const CONFIG = require('./constants')
const Storage = require('./storage')

function getToken() {
  return Storage.get('token')
}

function getRefreshToken() {
  return Storage.get('refreshToken')
}

// 是否正在刷新 token
let isRefreshing = false
// 等待刷新完成的队列
let refreshQueue = []

// 通用请求封装
const request = (options) => {
  return new Promise((resolve, reject) => {
    const token = getToken()

    wx.request({
      url: `${CONFIG.API_BASE}${options.url}`,
      method: options.method || 'GET',
      data: options.data,
      header: {
        'Content-Type': 'application/json',
        ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
        ...options.header
      },
      success(res) {
        if (res.statusCode === 401) {
          handle401(options, resolve, reject)
          return
        }
        if (res.statusCode >= 200 && res.statusCode < 300) {
          resolve(res.data)
        } else {
          reject({ status: res.statusCode, message: res.data?.message || res.data?.title || '请求失败', data: res.data })
        }
      },
      fail(err) {
        reject({ status: 0, message: '网络异常，请检查网络连接', detail: err })
      }
    })
  })
}

// 处理 401：尝试用 refresh_token 刷新
function handle401(originalOptions, resolve, reject) {
  const refreshToken = getRefreshToken()

  if (!refreshToken) {
    // 没有 refresh_token，直接跳登录
    goLogin()
    reject(new Error('登录已过期'))
    return
  }

  if (isRefreshing) {
    // 正在刷新中，排队等待
    refreshQueue.push({ options: originalOptions, resolve, reject })
    return
  }

  isRefreshing = true

  wx.request({
    url: `${CONFIG.API_BASE}/api/miniprogram/auth/refresh`,
    method: 'POST',
    data: { refreshToken },
    header: { 'Content-Type': 'application/json' },
    success(res) {
      if (res.statusCode === 200 && res.data.accessToken) {
        // 刷新成功，更新 token
        Storage.set('token', res.data.accessToken)
        if (res.data.refreshToken) {
          Storage.set('refreshToken', res.data.refreshToken)
        }

        // 重放原请求
        const newToken = res.data.accessToken
        wx.request({
          url: `${CONFIG.API_BASE}${originalOptions.url}`,
          method: originalOptions.method || 'GET',
          data: originalOptions.data,
          header: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${newToken}`,
            ...originalOptions.header
          },
          success(retryRes) {
            if (retryRes.statusCode >= 200 && retryRes.statusCode < 300) {
              resolve(retryRes.data)
            } else {
              reject({ status: retryRes.statusCode, message: retryRes.data?.message || '请求失败', data: retryRes.data })
            }
          },
          fail(err) {
            reject({ status: 0, message: '网络异常', detail: err })
          }
        })

        // 处理等待队列
        refreshQueue.forEach(item => {
          item.resolve(request(item.options))
        })
        refreshQueue = []
      } else {
        // 刷新失败，跳登录
        goLogin()
        reject(new Error('登录已过期'))
        refreshQueue.forEach(item => {
          item.reject(new Error('登录已过期'))
        })
        refreshQueue = []
      }
    },
    fail() {
      goLogin()
      reject(new Error('网络异常'))
      refreshQueue.forEach(item => {
        item.reject(new Error('网络异常'))
      })
      refreshQueue = []
    },
    complete() {
      isRefreshing = false
    }
  })
}

function goLogin() {
  Storage.remove('token')
  Storage.remove('refreshToken')
  Storage.remove('userInfo')
  Storage.remove('patients')
  wx.redirectTo({ url: '/pages/login/login' })
}

const api = {
  get(url, data) {
    return request({ url, data, method: 'GET' })
  },
  post(url, data) {
    return request({ url, data, method: 'POST' })
  },
  put(url, data) {
    return request({ url, data, method: 'PUT' })
  },
  patch(url, data) {
    return request({ url, data, method: 'PATCH' })
  },
  delete(url) {
    return request({ url, method: 'DELETE' })
  }
}

module.exports = { api, request }

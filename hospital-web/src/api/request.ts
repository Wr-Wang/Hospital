import axios from 'axios'
import type { AxiosResponse, InternalAxiosRequestConfig } from 'axios'
import { API_BASE_URL } from '../utils/constants'

const request = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' },
})

// 全局错误提示回调（由 App.vue 在 setup 中注册）
let showError: ((msg: string) => void) | null = null
export function setErrorHandler(handler: (msg: string) => void) {
  showError = handler
}

// 请求拦截器：自动注入 JWT Token
request.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = localStorage.getItem('token')
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// 响应拦截器：统一错误处理
request.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('userInfo')
      const currentPath = window.location.pathname
      if (currentPath !== '/login') {
        window.location.href = `/login?redirect=${encodeURIComponent(currentPath)}`
      }
    } else if (error.response?.status === 403) {
      showError?.('权限不足，无法执行此操作')
    } else if (error.response?.data?.error) {
      // 后端业务错误消息（如 "已停用的排班无法发布"）
      showError?.(error.response.data.error)
    } else if (error.response?.data?.message) {
      showError?.(error.response.data.message)
    } else if (error.message === 'Network Error') {
      showError?.('网络连接失败，请检查网络或联系管理员')
    } else if (error.code === 'ECONNABORTED') {
      showError?.('请求超时，请稍后重试')
    }
    return Promise.reject(error)
  },
)

export default request

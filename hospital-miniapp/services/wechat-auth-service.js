const { api } = require('../utils/api')

// 微信登录服务
const WeChatAuthService = {
  // code → 登录/创建患者
  login(code) {
    return api.post('/api/miniprogram/auth/login', { code })
  },

  // 创建新患者并绑定微信
  createPatient(tempToken, name, phone) {
    return api.post('/api/miniprogram/auth/create-patient', { tempToken, name, phone })
  },

  // 刷新 token
  refresh(refreshToken) {
    return api.post('/api/miniprogram/auth/refresh', { refreshToken })
  },

  // 注销
  logout(refreshToken) {
    return api.post('/api/miniprogram/auth/logout', { refreshToken })
  },

  // 获取当前登录患者的资料
  getCurrentPatient() {
    return api.get('/api/miniprogram/auth/me')
  }
}

module.exports = WeChatAuthService

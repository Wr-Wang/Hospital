const { api } = require('../utils/api')

// 登录服务
const AuthService = {
  // 账号密码登录
  login(username, password) {
    return api.post('/api/authentication/login', { username, password })
  },

  // 退出登录
  logout() {
    return api.post('/api/authentication/logout')
  },

  // 获取当前登录用户信息
  getCurrentUser() {
    return api.get('/api/user/me')
  }
}

module.exports = AuthService

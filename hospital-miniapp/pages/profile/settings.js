const Storage = require('../../utils/storage')
const WeChatAuthService = require('../../services/wechat-auth-service')

Page({
  data: {
    isLoggedIn: false,
    phone: '',
    cacheSize: '0KB'
  },

  onShow() {
    const token = Storage.get('token')
    const userInfo = Storage.get('userInfo')
    this.setData({
      isLoggedIn: !!token,
      phone: userInfo?.patientNo || ''
    })

    try {
      const info = wx.getStorageInfoSync()
      this.setData({ cacheSize: (info.currentSize / 1024).toFixed(1) + 'MB' })
    } catch {
      // ignore
    }
  },

  async handleLogout() {
    wx.showModal({
      title: '退出登录',
      content: '确定要退出登录吗？',
      success: async (res) => {
        if (res.confirm) {
          const app = getApp()
          // 先调后端撤销 refresh_token（失败不阻塞本地退出）
          const refreshToken = Storage.get('refreshToken')
          if (refreshToken) {
            try {
              await WeChatAuthService.logout(refreshToken)
            } catch (e) {
              // 忽略：网络/过期场景下本地退出兜底
            }
          }
          app.logout()
          wx.showToast({ title: '已退出', icon: 'success' })
          setTimeout(() => wx.redirectTo({ url: '/pages/login/login' }), 1000)
        }
      }
    })
  }
})

const Storage = require('../../utils/storage')

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

  handleLogout() {
    wx.showModal({
      title: '退出登录',
      content: '确定要退出登录吗？',
      success: (res) => {
        if (res.confirm) {
          const app = getApp()
          app.logout()
          wx.showToast({ title: '已退出', icon: 'success' })
          setTimeout(() => wx.redirectTo({ url: '/pages/login/login' }), 1000)
        }
      }
    })
  }
})

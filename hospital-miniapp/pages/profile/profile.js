const RegistrationService = require('../../services/registration-service')
const WeChatAuthService = require('../../services/wechat-auth-service')
const Storage = require('../../utils/storage')

Page({
  data: {
    isLoggedIn: false,
    displayName: '',
    userInitial: '👤',
    pendingCount: 0,
    visitedCount: 0,
    patientCount: 0
  },

  async onShow() {
    const token = Storage.get('token')
    const userInfo = Storage.get('userInfo')
    const patientName = userInfo?.name || ''

    this.setData({
      isLoggedIn: !!token,
      displayName: patientName || userInfo?.displayName || userInfo?.nickName || '未登录',
      userInitial: patientName ? patientName.charAt(0) : (userInfo?.displayName ? userInfo.displayName.charAt(0) : '👤'),
      patientCount: this.getPatientCount()
    })

    if (token) {
      await this.loadStats()
    }
  },

  getPatientCount() {
    const stored = Storage.get('patients')
    return (stored && stored.length) || 0
  },

  async loadStats() {
    try {
      const userInfo = Storage.get('userInfo')
      const patientId = userInfo?.patientId
      if (!patientId) return

      const list = await RegistrationService.getByPatient(patientId)
      const appointments = Array.isArray(list) ? list : []

      this.setData({
        pendingCount: appointments.filter(a => a.status === '已挂号').length,
        visitedCount: appointments.filter(a => a.status === '已就诊').length
      })
    } catch {
      // 静默
    }
  },

  goToLogin() {
    wx.navigateTo({ url: '/pages/login/login' })
  },

  goToUserDetail() {
    wx.navigateTo({ url: '/pages/profile/detail/index' })
  },

  goToPatients() {
    wx.navigateTo({ url: '/pages/profile/patients/index' })
  },

  goToRecords() {
    wx.redirectTo({ url: '/pages/appointment/records' })
  },

  goToPayments() {
    wx.showToast({ title: '功能开发中', icon: 'none' })
  },

  goToReports() {
    wx.showToast({ title: '功能开发中', icon: 'none' })
  },

  goToSettings() {
    wx.navigateTo({ url: '/pages/profile/settings' })
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
          this.setData({
            isLoggedIn: false,
            displayName: '',
            userInitial: '👤',
            pendingCount: 0,
            visitedCount: 0,
            patientCount: 0
          })
          wx.showToast({ title: '已退出', icon: 'success' })
        }
      }
    })
  }
})

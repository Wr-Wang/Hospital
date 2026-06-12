const RegistrationService = require('../../services/registration-service')
const CampusService = require('../../services/campus-service')
const Storage = require('../../utils/storage')

Page({
  data: {
    isLoggedIn: false,
    displayName: '',
    userAvatar: '👤',
    hotDepts: [
      { id: 0, name: '呼吸内科', icon: '🫁' },
      { id: 0, name: '消化内科', icon: '🫃' },
      { id: 0, name: '心血管内科', icon: '❤️' },
      { id: 0, name: '骨科', icon: '🦴' },
      { id: 0, name: '儿科', icon: '👶' },
      { id: 0, name: '妇产科', icon: '🤰' },
      { id: 0, name: '眼科', icon: '👁️' },
      { id: 0, name: '中医科', icon: '🌿' }
    ],
    serviceTags: [
      { name: '专家门诊', icon: '👨‍⚕️' },
      { name: '普通门诊', icon: '🏥' },
      { name: '急诊服务', icon: '🚑' },
      { name: '体检中心', icon: '📊' },
      { name: '疫苗接种', icon: '💉' },
      { name: '住院服务', icon: '🛏️' }
    ],
    notices: [
      '2026年端午节假期门诊安排通知',
      '我院已开通线上预约挂号服务，欢迎使用',
      '医保电子凭证已全面启用，就诊时请出示'
    ],
    upcomingAppointment: null
  },

  onShow() {
    this.loadUserInfo()
    this.loadUpcoming()
  },

  loadUserInfo() {
    const token = Storage.get('token')
    const userInfo = Storage.get('userInfo')
    this.setData({
      isLoggedIn: !!token,
      displayName: userInfo?.name || userInfo?.nickName || userInfo?.displayName || '未登录',
      userAvatar: userInfo?.name ? userInfo.name.charAt(0) : (userInfo?.nickName ? userInfo.nickName.charAt(0) : '👤')
    })
  },

  async loadUpcoming() {
    const token = Storage.get('token')
    if (!token) return

    try {
      // 获取当前患者的 id
      const userInfo = Storage.get('userInfo')
      const currentPatientId = userInfo?.patientId
      if (!currentPatientId) return

      const list = await RegistrationService.getByPatient(currentPatientId)
      const upcoming = Array.isArray(list)
        ? list.find(a => a.status === '已挂号')
        : null

      if (upcoming) {
        this.setData({
          upcomingAppointment: {
            id: upcoming.id,
            deptName: upcoming.deptName || '科室',
            doctorName: upcoming.doctorName || '医生',
            doctorTitle: upcoming.doctorTitle || '',
            date: upcoming.scheduleDate ? this.formatDate(upcoming.scheduleDate) : '',
            timeSlot: upcoming.slotName || '',
            queueNumber: upcoming.queueNumber || 0
          }
        })
      }
    } catch {
      // 静默失败，不影响首页展示
    }
  },

  formatDate(dateStr) {
    if (!dateStr) return ''
    const d = new Date(dateStr)
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
  },

  goToDeptList() {
    wx.navigateTo({ url: '/pages/dept/list' })
  },

  goToQueue() {
    wx.navigateTo({ url: '/pages/queue/status' })
  },

  goToRecords() {
    wx.redirectTo({ url: '/pages/appointment/records' })
  },

  goToReports() {
    wx.showToast({ title: '功能开发中', icon: 'none' })
  },

  goToProfile() {
    wx.redirectTo({ url: '/pages/profile/profile' })
  },

  goToDeptDetail(e) {
    const { name } = e.currentTarget.dataset
    wx.navigateTo({ url: `/pages/dept/list?keyword=${encodeURIComponent(name)}` })
  }
})

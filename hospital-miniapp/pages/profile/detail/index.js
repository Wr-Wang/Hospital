const Storage = require('../../../utils/storage')

Page({
  data: {
    patientInfo: {
      name: '',
      patientNo: '',
      phone: ''
    },
    userInitial: '👤',
    patientCount: 0
  },

  onShow() {
    this.loadPatientInfo()
  },

  loadPatientInfo() {
    const userInfo = Storage.get('userInfo')
    if (!userInfo) return

    // 尝试从就诊人列表中查找手机号
    const patients = Storage.get('patients') || []
    const self = patients.find(p => String(p.id) === String(userInfo.patientId))

    this.setData({
      patientInfo: {
        name: userInfo.name || '',
        patientNo: userInfo.patientNo || '',
        phone: self?.phone || userInfo.phone || ''
      },
      userInitial: userInfo.name ? userInfo.name.charAt(0) : '👤',
      patientCount: patients.length
    })
  },

  goToPatients() {
    wx.navigateTo({ url: '/pages/profile/patients/index' })
  }
})

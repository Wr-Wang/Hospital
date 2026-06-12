Page({
  data: {
    deptName: '',
    doctorName: '',
    doctorTitle: '',
    date: '',
    slotName: '',
    startTime: '',
    endTime: ''
  },

  onLoad(options) {
    this.setData({
      deptName: decodeURIComponent(options.deptName || ''),
      doctorName: decodeURIComponent(options.doctorName || ''),
      doctorTitle: decodeURIComponent(options.doctorTitle || ''),
      date: options.date || '',
      slotName: decodeURIComponent(options.slotName || ''),
      startTime: options.startTime || '',
      endTime: options.endTime || ''
    })
  },

  goToRecords() {
    wx.redirectTo({ url: '/pages/appointment/records' })
  },

  goToHome() {
    wx.redirectTo({ url: '/pages/index/index' })
  }
})

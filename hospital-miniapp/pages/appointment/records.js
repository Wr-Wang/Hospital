const RegistrationService = require('../../services/registration-service')
const Format = require('../../utils/format')
const Storage = require('../../utils/storage')

Page({
  data: {
    currentTab: 0,
    tabs: [
      { name: '待就诊', count: 0, filter: '已挂号' },
      { name: '已就诊', count: 0, filter: '已就诊' },
      { name: '已取消', count: 0, filter: '已退号' }
    ],
    list: [],
    loading: true
  },

  onShow() {
    this.loadAppointments()
  },

  async loadAppointments() {
    const token = Storage.get('token')
    if (!token) {
      this.setData({ list: [], loading: false })
      return
    }

    this.setData({ loading: true })

    try {
      const userInfo = Storage.get('userInfo')
      const currentPatientId = userInfo?.patientId
      if (!currentPatientId) {
        this.setData({ list: [], loading: false })
        return
      }

      let list = await RegistrationService.getByPatient(currentPatientId)
      list = Array.isArray(list) ? list : []

      const mapped = list.map(a => ({
        id: a.id,
        patientId: a.patientId,
        patientName: a.patientName || '',
        doctorName: a.doctorName || '',
        doctorTitle: a.doctorTitle || '',
        deptName: a.deptName || '',
        campusName: a.campusName || '',
        date: a.scheduleDate ? Format.date(a.scheduleDate) : '',
        timeSlot: a.slotName || '',
        queueNumber: a.queueNumber || 0,
        status: a.status,
        statusText: a.status,
        statusObj: Format.appointmentStatus(a.status),
        createTime: a.registerTime ? Format.datetime(a.registerTime) : ''
      }))

      // 按状态计数
      const tabs = this.data.tabs.map(t => ({
        ...t,
        count: mapped.filter(a => a.status === t.filter).length
      }))

      this.setData({ allAppointments: mapped, tabs, loading: false })
      this.filterList()
    } catch (err) {
      this.setData({ loading: false })
      wx.showToast({ title: '加载预约记录失败', icon: 'none' })
    }
  },

  filterList() {
    const filterStatus = this.data.tabs[this.data.currentTab].filter
    const list = (this.data.allAppointments || [])
      .filter(a => a.status === filterStatus)

    this.setData({ list })
  },

  switchTab(e) {
    this.setData({ currentTab: e.currentTarget.dataset.index }, () => this.filterList())
  },

  async cancelAppointment(e) {
    const id = e.currentTarget.dataset.id
    wx.showModal({
      title: '取消预约',
      content: '确定要取消该预约吗？',
      success: async (res) => {
        if (res.confirm) {
          try {
            await RegistrationService.cancel(id)
            wx.showToast({ title: '已取消', icon: 'success' })
            this.loadAppointments()
          } catch (err) {
            wx.showToast({ title: err.message || '取消失败', icon: 'none' })
          }
        }
      }
    })
  },

  viewQueue() {
    wx.navigateTo({ url: '/pages/queue/status' })
  }
})

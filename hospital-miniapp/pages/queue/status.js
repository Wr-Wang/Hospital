const EncounterService = require('../../services/encounter-service')
const Storage = require('../../utils/storage')

Page({
  data: {
    queueInfo: {
      currentNumber: '-',
      myNumber: '-',
      waitingCount: 0,
      estimatedWait: '加载中...',
      status: 'waiting',
      statusText: '等待中',
      department: '',
      doctor: '',
      date: ''
    }
  },

  onShow() {
    this.refreshQueue()
  },

  async refreshQueue() {
    const token = Storage.get('token')
    if (!token) {
      this.setData({
        queueInfo: { ...this.data.queueInfo, estimatedWait: '请先登录', statusText: '未登录' }
      })
      return
    }

    wx.showLoading({ title: '刷新中...' })
    try {
      const userInfo = Storage.get('userInfo')
      const currentPatientId = userInfo?.patientId
      // 获取最近的预约以找到医生 ID
      const regService = require('../../services/registration-service')
      const list = await regService.getByPatient(currentPatientId)
      const appointments = Array.isArray(list) ? list : []
      const upcoming = appointments.find(a => a.status === '已挂号')

      if (!upcoming) {
        this.setData({
          queueInfo: {
            currentNumber: '-',
            myNumber: '-',
            waitingCount: 0,
            estimatedWait: '暂无就诊信息',
            status: 'completed',
            statusText: '暂无',
            department: '',
            doctor: '',
            date: ''
          }
        })
        wx.hideLoading()
        return
      }

      const today = new Date()
      const dateStr = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}`

      // 获取该医生的门诊队列
      const queue = await EncounterService.getQueue(upcoming.doctorId, dateStr)
      const queueList = Array.isArray(queue) ? queue : []

      // 找出当前患者的位置
      const myEntry = queueList.find(q => q.patientId === currentPatientId)
      const myIndex = queueList.findIndex(q => q.patientId === currentPatientId)
      const currentIndex = queueList.findIndex(q => q.status === '就诊中')

      const waitingCount = myIndex > currentIndex ? myIndex - currentIndex : 0

      const statusMap = { 'waiting': '等待中', 'consulting': '就诊中', 'completed': '已完成' }

      this.setData({
        queueInfo: {
          currentNumber: currentIndex >= 0 ? queueList[currentIndex].queueNumber : '-',
          myNumber: myEntry?.queueNumber || upcoming.queueNumber || '-',
          waitingCount,
          estimatedWait: waitingCount <= 0 ? '即将就诊' : `约 ${waitingCount * 5} 分钟`,
          status: currentIndex < 0 ? 'waiting' : (myIndex <= currentIndex ? 'consulting' : 'waiting'),
          statusText: statusMap[currentIndex < 0 ? 'waiting' : (myIndex <= currentIndex ? 'consulting' : 'waiting')],
          department: upcoming.deptName || '',
          doctor: upcoming.doctorName || '',
          date: (upcoming.scheduleDate ? this.formatDate(upcoming.scheduleDate) : dateStr) + ' ' + (upcoming.slotName || '')
        }
      })
    } catch {
      this.setData({
        queueInfo: { ...this.data.queueInfo, estimatedWait: '刷新失败' }
      })
    } finally {
      wx.hideLoading()
    }
  },

  formatDate(dateStr) {
    if (!dateStr) return ''
    const d = new Date(dateStr)
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
  }
})

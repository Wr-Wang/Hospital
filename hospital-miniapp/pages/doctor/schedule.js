const StaffService = require('../../services/staff-service')
const ScheduleService = require('../../services/schedule-service')
const Format = require('../../utils/format')

const DOCTOR_AVATARS = ['👨‍⚕️', '👩‍⚕️', '👨‍⚕️', '👩‍⚕️', '👨‍⚕️', '👩‍⚕️']

Page({
  data: {
    deptId: '',
    deptName: '',
    weekDays: [],
    selectedDate: '',
    selectedDateInfo: '',
    doctors: [],
    loading: true
  },

  onLoad(options) {
    const deptId = parseInt(options.deptId) || 0
    const deptName = decodeURIComponent(options.deptName || '')

    const weekDays = Format.getWeekDays(7)

    this.setData({
      deptId,
      deptName,
      weekDays,
      selectedDate: weekDays[0].date,
      selectedDateInfo: weekDays[0].full + ' ' + weekDays[0].dayOfWeek
    })

    this.loadData()
  },

  async loadData() {
    if (!this.data.deptId) {
      this.setData({ loading: false })
      return
    }

    this.setData({ loading: true })

    try {
      // 并行获取医生列表和排班
      const staffList = await StaffService.getByDept(this.data.deptId)
      const staff = Array.isArray(staffList) ? staffList : []

      // 对每个医生获取当天的排班
      const doctorsWithSchedule = []
      for (let i = 0; i < staff.length; i++) {
        const s = staff[i]
        let schedules = []
        try {
          const result = await ScheduleService.getAvailable(this.data.deptId, this.data.selectedDate, s.id)
          schedules = Array.isArray(result) ? result : []
        } catch {
          // 该医生当天无排班
        }

        // 计算可用号数
        const totalAvailable = schedules.reduce((sum, sch) =>
          sum + (sch.slots || []).reduce((ss, sl) => ss + (sl.availableQuota || 0), 0), 0)

        // 无排班的医生不展示
        if (schedules.length === 0 || totalAvailable === 0) continue

        // 拍平时段列表
        const allSlots = schedules.flatMap(sch =>
          (sch.slots || []).map(sl => ({
            id: sl.id,
            scheduleId: sch.id,
            slotName: sl.slotName,
            startTime: sl.startTime || '',
            endTime: sl.endTime || '',
            totalQuota: sl.totalQuota || 0,
            bookedQuota: sl.bookedQuota || 0,
            availableQuota: sl.availableQuota || 0
          }))
        )

        // 按起始小时分上/下午（12:00 为界）
        const morningSlots = allSlots.filter(s => {
          const h = parseInt(s.startTime)
          return !isNaN(h) && h < 12
        })
        const afternoonSlots = allSlots.filter(s => {
          const h = parseInt(s.startTime)
          return !isNaN(h) && h >= 12
        })

        doctorsWithSchedule.push({
          id: s.id,
          name: s.name,
          title: s.licenseType || '医生',
          deptName: this.data.deptName,
          avatar: DOCTOR_AVATARS[i % DOCTOR_AVATARS.length],
          expanded: i === 0,
          flatSlots: allSlots,
          morningSlots,
          afternoonSlots,
          availableSlots: totalAvailable
        })
      }

      this.setData({ doctors: doctorsWithSchedule, loading: false })
    } catch (err) {
      this.setData({ loading: false })
      wx.showToast({ title: '加载排班失败', icon: 'none' })
    }
  },

  selectDate(e) {
    const date = e.currentTarget.dataset.date
    const dayInfo = this.data.weekDays.find(d => d.date === date)
    this.setData({
      selectedDate: date,
      selectedDateInfo: dayInfo ? dayInfo.full + ' ' + dayInfo.dayOfWeek : date
    })
    this.loadData()
  },

  toggleExpand(e) {
    const doctorId = parseInt(e.currentTarget.dataset.id)
    const doctors = this.data.doctors.map(d => ({
      ...d,
      expanded: d.id === doctorId ? !d.expanded : false
    }))
    this.setData({ doctors })
  },

  bookSlot(e) {
    const token = wx.getStorageSync('token')
    if (!token) {
      wx.showModal({
        title: '提示',
        content: '请先登录后再预约挂号',
        success: (res) => {
          if (res.confirm) wx.navigateTo({ url: '/pages/login/login' })
        }
      })
      return
    }

    const { doctor: doctorId, slot: slotId, scheduleId, slotName, startTime, endTime } = e.currentTarget.dataset
    const doctor = this.data.doctors.find(d => d.id === parseInt(doctorId))

    wx.navigateTo({
      url: `/pages/appointment/confirm?doctorId=${doctorId}&doctorName=${encodeURIComponent(doctor.name)}&doctorTitle=${encodeURIComponent(doctor.title)}&deptId=${this.data.deptId}&deptName=${encodeURIComponent(this.data.deptName)}&date=${this.data.selectedDate}&slotId=${slotId}&scheduleId=${scheduleId}&slotName=${encodeURIComponent(slotName)}&startTime=${startTime}&endTime=${endTime}`
    })
  }
})

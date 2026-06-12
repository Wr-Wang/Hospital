const RegistrationService = require('../../services/registration-service')
const PatientService = require('../../services/patient-service')
const Format = require('../../utils/format')
const Storage = require('../../utils/storage')

Page({
  data: {
    deptId: '',
    deptName: '',
    doctorId: '',
    doctorName: '',
    doctorTitle: '',
    date: '',
    slotId: '',
    scheduleId: 0,
    slotName: '',
    startTime: '',
    endTime: '',
    patients: [],
    selectedPatientId: '',
    submitting: false,
    errorMsg: ''
  },

  onLoad(options) {
    this.setData({
      deptId: parseInt(options.deptId) || '',
      deptName: decodeURIComponent(options.deptName || ''),
      doctorId: parseInt(options.doctorId) || '',
      doctorName: decodeURIComponent(options.doctorName || ''),
      doctorTitle: decodeURIComponent(options.doctorTitle || ''),
      date: options.date || '',
      slotId: options.slotId || '',
      scheduleId: parseInt(options.scheduleId) || 0,
      slotName: decodeURIComponent(options.slotName || ''),
      startTime: options.startTime || '',
      endTime: options.endTime || ''
    })
  },

  onShow() {
    this.loadPatients()
  },

  async loadPatients() {
    try {
      const storeKey = 'patients'
      let rawPatients = Storage.get(storeKey)

      // 本地没有，搜索当前患者关联的就诊人
      if (!rawPatients || rawPatients.length === 0) {
        const userInfo = Storage.get('userInfo')
        if (userInfo?.patientId) {
          const result = await PatientService.search(String(userInfo.patientId), 1, 10)
          const items = result.items || result.data || []
          rawPatients = items
        }
      }

      if (rawPatients && rawPatients.length > 0) {
        // 统一映射字段，确保 idCard/phone/gender 等字段存在
        const patients = rawPatients.map(p => {
          const idCard = p.idCard || ''
          const phone = p.phone || ''
          console.log('[confirm] patient:', p.id, p.name, 'idCard:', idCard, 'phone:', phone)
          return {
            id: p.id,
            name: p.name || '',
            idCard,
            phone,
            idCardDisplay: Format.maskIdCard(idCard),
            phoneDisplay: Format.maskPhone(phone),
            gender: p.gender === 'Male' || p.gender === '男' || p.gender === 1 ? 1 : (p.gender === 'Female' || p.gender === '女' || p.gender === 2 ? 2 : 0),
            isDefault: p.isDefault === true
          }
        })
        this.setData({
          patients,
          selectedPatientId: patients.find(p => p.isDefault)?.id || (patients[0]?.id || '')
        })
      }
    } catch {
      // 静默处理
    }
  },

  selectPatient(e) {
    this.setData({ selectedPatientId: parseInt(e.currentTarget.dataset.id) || e.currentTarget.dataset.id })
  },

  deletePatient(e) {
    const id = String(e.currentTarget.dataset.id)
    if (!id) return

    const patient = this.data.patients.find(p => String(p.id) === id)
    wx.showModal({
      title: '删除就诊人',
      content: `确定要删除 ${patient?.name || ''} 吗？`,
      success: (res) => {
        if (res.confirm) {
          // 直接从当前页面数据中删除，确保界面立即更新
          const remaining = this.data.patients.filter(p => String(p.id) !== id)
          Storage.set('patients', remaining)
          this.setData({
            patients: remaining,
            selectedPatientId: remaining.length > 0 && String(this.data.selectedPatientId) === id
              ? remaining[0]?.id || ''
              : this.data.selectedPatientId
          })
        }
      }
    })
  },

  addPatient() {
    wx.navigateTo({ url: '/pages/profile/patients/edit' })
  },

  async submitAppointment() {
    if (!this.data.selectedPatientId) {
      wx.showToast({ title: '请选择就诊人', icon: 'none' })
      return
    }

    const patientId = parseInt(this.data.selectedPatientId)
    if (!patientId) {
      wx.showToast({ title: '该就诊人未同步到服务器，请重新添加', icon: 'none' })
      return
    }

    this.setData({ submitting: true, errorMsg: '' })

    try {
      const payload = {
        patientId,
        scheduleId: this.data.scheduleId,
        doctorId: parseInt(this.data.doctorId) || 0,
        deptId: parseInt(this.data.deptId) || 0,
        campusId: 1,
        slotName: this.data.slotName
      }
      console.log('[submit] payload:', JSON.stringify(payload))

      await RegistrationService.create(payload)

      wx.redirectTo({
        url: `/pages/appointment/success?deptName=${encodeURIComponent(this.data.deptName)}&doctorName=${encodeURIComponent(this.data.doctorName)}&doctorTitle=${encodeURIComponent(this.data.doctorTitle)}&date=${this.data.date}&startTime=${this.data.startTime}&endTime=${this.data.endTime}&slotName=${encodeURIComponent(this.data.slotName)}`
      })
    } catch (err) {
      this.setData({ errorMsg: err.message || '预约失败，请重试' })
      wx.showToast({ title: '预约失败', icon: 'none' })
    } finally {
      this.setData({ submitting: false })
    }
  }
})

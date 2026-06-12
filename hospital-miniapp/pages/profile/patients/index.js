const PatientService = require('../../../services/patient-service')
const Format = require('../../../utils/format')
const Storage = require('../../../utils/storage')

function calculateAge(idCard) {
  if (!idCard || idCard.length < 18) return 0
  const birthYear = parseInt(idCard.substring(6, 10))
  return new Date().getFullYear() - birthYear
}

Page({
  data: {
    patients: []
  },

  onShow() {
    this.loadPatients()
  },

  async loadPatients() {
    // 先从本地读取
    const stored = Storage.get('patients')
    if (stored && stored.length > 0) {
      this.setData({
        patients: stored.map(p => ({
          ...p,
          age: calculateAge(p.idCard),
          idCardDisplay: Format.maskIdCard(p.idCard || ''),
          phoneDisplay: Format.maskPhone(p.phone || '')
        }))
      })
      return
    }

    // 本地没有，尝试从后端搜索当前患者
    try {
      const userInfo = Storage.get('userInfo')
      if (userInfo?.patientId) {
        const result = await PatientService.search(String(userInfo.patientId))
        const items = result.items || result.data || []

        const mapped = items.map(p => ({
          id: p.id,
          name: p.name,
          patientNo: p.patientNo || '',
          idCard: p.idCard || '',
          phone: p.phone || '',
          idCardDisplay: Format.maskIdCard(p.idCard || ''),
          phoneDisplay: Format.maskPhone(p.phone || ''),
          gender: p.gender === '男' || p.gender === 'Male' ? 1 : (p.gender === '女' || p.gender === 'Female' ? 2 : 0),
          isDefault: false,
          age: calculateAge(p.idCard || '')
        }))

        if (mapped.length > 0) mapped[0].isDefault = true
        this.setData({ patients: mapped })
        Storage.set('patients', mapped)
      }
    } catch {
      // 静默
    }
  },

  maskIdCard(idCard) {
    return Format.maskIdCard(idCard)
  },

  maskPhone(phone) {
    return Format.maskPhone(phone)
  },

  editPatient(e) {
    const id = e.currentTarget.dataset.id
    wx.navigateTo({ url: `/pages/profile/patients/edit?id=${id}` })
  },

  deletePatient(e) {
    const id = e.currentTarget.dataset.id
    wx.showModal({
      title: '确认删除',
      content: '确定要删除该就诊人吗？',
      success: (res) => {
        if (res.confirm) {
          const patients = Storage.get('patients') || []
          const updated = patients.filter(p => String(p.id) !== String(id))
          Storage.set('patients', updated)

          const app = getApp()
          if (app.globalData.patients) {
            app.globalData.patients = updated
          }
          this.loadPatients()
          wx.showToast({ title: '已删除', icon: 'success' })
        }
      }
    })
  },

  setDefault(e) {
    const id = e.currentTarget.dataset.id
    const patients = (Storage.get('patients') || []).map(p => ({
      ...p,
      isDefault: p.id === id
    }))
    Storage.set('patients', patients)
    this.loadPatients()
    wx.showToast({ title: '已设为默认', icon: 'success' })
  },

  addPatient() {
    wx.navigateTo({ url: '/pages/profile/patients/edit' })
  }
})

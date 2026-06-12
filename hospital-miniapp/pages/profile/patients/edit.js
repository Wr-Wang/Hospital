const PatientService = require('../../../services/patient-service')
const Storage = require('../../../utils/storage')

function generateId() {
  return 'pat_' + Date.now() + '_' + Math.random().toString(36).slice(2, 6)
}

function isValidIdCardDate(idCard) {
  if (!idCard || idCard.length < 18) return false
  const year = parseInt(idCard.substring(6, 10), 10)
  const month = parseInt(idCard.substring(10, 12), 10)
  const day = parseInt(idCard.substring(12, 14), 10)
  if (month < 1 || month > 12 || day < 1 || day > 31) return false
  // 检查闰年 2 月等边界
  const d = new Date(year, month - 1, day)
  return d.getFullYear() === year && d.getMonth() === month - 1 && d.getDate() === day
}

Page({
  data: {
    isEdit: false,
    editId: '',
    form: {
      name: '',
      gender: 1,
      idCard: '',
      phone: '',
      relation: '',
      relationIndex: 0
    },
    relationOptions: ['本人', '配偶', '子女', '父母', '亲属', '朋友', '其他']
  },

  onLoad(options) {
    const editId = options.id
    if (editId) {
      const patients = Storage.get('patients') || []
      const patient = patients.find(p => String(p.id) === editId)
      if (patient) {
        const relIndex = this.data.relationOptions.indexOf(patient.relation || '')
        this.setData({
          isEdit: true,
          editId,
          form: {
            name: patient.name || '',
            gender: patient.gender || 1,
            idCard: patient.idCard || '',
            phone: patient.phone || '',
            relation: patient.relation || '',
            relationIndex: relIndex >= 0 ? relIndex : 0
          }
        })
      }
    }
  },

  onInput(e) {
    this.setData({ [`form.${e.currentTarget.dataset.field}`]: e.detail.value })
  },

  setGender(e) {
    this.setData({ 'form.gender': parseInt(e.currentTarget.dataset.value) })
  },

  onRelationChange(e) {
    const index = e.detail.value
    this.setData({
      'form.relation': this.data.relationOptions[index],
      'form.relationIndex': index
    })
  },

  async savePatient() {
    const { name, gender, idCard, phone } = this.data.form
    if (!name.trim()) { wx.showToast({ title: '请输入姓名', icon: 'none' }); return }
    if (!idCard.trim() || idCard.length < 18) { wx.showToast({ title: '请输入正确的身份证号', icon: 'none' }); return }
    if (!phone.trim() || phone.length < 11) { wx.showToast({ title: '请输入正确的手机号', icon: 'none' }); return }
    if (!isValidIdCardDate(idCard)) { wx.showToast({ title: '身份证号码有误，请检查', icon: 'none' }); return }

    // 从身份证提取出生日期
    const birthDate = `${idCard.substring(6, 10)}-${idCard.substring(10, 12)}-${idCard.substring(12, 14)}`

    if (this.data.isEdit) {
      // 本地更新
      const patients = (Storage.get('patients') || []).map(p => {
        if (String(p.id) === this.data.editId) {
          return { ...p, name, gender, idCard, phone, relation: this.data.form.relation }
        }
        return p
      })
      Storage.set('patients', patients)
      wx.showToast({ title: '已更新', icon: 'success' })
    } else {
      // 准备本地患者数据
      const newPatient = {
        id: generateId(), // 临时 ID，API 成功后被替换
        name,
        gender,
        idCard,
        phone,
        relation: this.data.form.relation,
        isDefault: (Storage.get('patients') || []).length === 0
      }

      // 尝试在后端创建患者
      try {
        const payload = {
          patientNo: '',
          name,
          gender: gender === 1 ? 'Male' : 'Female',
          idCard,
          phone,
          birthDate
        }
        const result = await PatientService.create(payload)
        // 使用后端返回的真实 ID 替换临时 ID
        if (result && result.id) {
          newPatient.id = result.id
        }
      } catch (err) {
        console.warn('后端创建患者失败，已存至本地', err?.message || err)
        wx.showToast({ title: '已保存到本地，云端同步失败', icon: 'none' })
      }

      // 本地保存
      const patients = [...(Storage.get('patients') || []), newPatient]
      Storage.set('patients', patients)

      wx.showToast({ title: '添加成功', icon: 'success' })
    }

    setTimeout(() => wx.navigateBack(), 1500)
  },

  deletePatient() {
    if (!this.data.isEdit || !this.data.editId) return
    wx.showModal({
      title: '删除就诊人',
      content: `确定要删除 ${this.data.form.name || ''} 吗？`,
      success: (res) => {
        if (res.confirm) {
          const patients = (Storage.get('patients') || []).filter(p => String(p.id) !== this.data.editId)
          Storage.set('patients', patients)
          wx.showToast({ title: '已删除', icon: 'success' })
          setTimeout(() => wx.navigateBack(), 1500)
        }
      }
    })
  }
})

const { api } = require('../utils/api')

// 患者服务
const PatientService = {
  // 搜索患者
  search(keyword, page = 1, size = 20) {
    return api.get('/api/patient/search', { keyword, page, size })
  },

  // 按身份证号查询
  getByIdCard(idCard) {
    return api.get(`/api/patient/by-idcard/${idCard}`)
  },

  // 按病历号查询
  getByPatientNo(patientNo) {
    return api.get(`/api/patient/by-patient-no/${patientNo}`)
  },

  // 获取患者详细档案
  getProfile(id) {
    return api.get(`/api/patient/${id}/profile`)
  },

  // 获取患者详情
  getById(id) {
    return api.get(`/api/patient/${id}`)
  },

  // 新建患者
  create(data) {
    return api.post('/api/patient', data)
  }
}

module.exports = PatientService

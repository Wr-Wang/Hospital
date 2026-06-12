const { api } = require('../utils/api')

// 挂号（预约）服务
const RegistrationService = {
  // 创建挂号
  create(data) {
    return api.post('/api/registration', data)
  },

  // 按患者获取挂号记录
  getByPatient(patientId) {
    return api.get(`/api/registration/by-patient/${patientId}`)
  },

  // 按医生和日期获取挂号（队列）
  getByDoctor(doctorId, date) {
    return api.get(`/api/registration/by-doctor/${doctorId}`, { date })
  },

  // 获取挂号详情
  getById(id) {
    return api.get(`/api/registration/${id}`)
  },

  // 取消挂号
  cancel(id) {
    return api.patch(`/api/registration/${id}/void`)
  }
}

module.exports = RegistrationService

const { api } = require('../utils/api')

// 排班服务
const ScheduleService = {
  // 获取可用号源
  getAvailable(deptId, date, doctorId) {
    const params = { deptId, date }
    if (doctorId) params.doctorId = doctorId
    return api.get('/api/schedule/available', params)
  },

  // 按科室和日期获取排班
  getByDept(deptId, date) {
    return api.get(`/api/schedule/by-dept/${deptId}`, { date })
  },

  // 按医生获取排班
  getByDoctor(doctorId) {
    return api.get(`/api/schedule/by-doctor/${doctorId}`)
  }
}

module.exports = ScheduleService

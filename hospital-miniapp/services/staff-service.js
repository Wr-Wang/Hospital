const { api } = require('../utils/api')

// 人员（医生）服务
const StaffService = {
  // 获取所有人员
  getAll() {
    return api.get('/api/staff')
  },

  // 按科室获取人员
  getByDept(deptId) {
    return api.get(`/api/staff/by-dept/${deptId}`)
  },

  // 按院区获取人员
  getByCampus(campusId) {
    return api.get(`/api/staff/by-campus/${campusId}`)
  }
}

module.exports = StaffService

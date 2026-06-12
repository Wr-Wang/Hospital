const { api } = require('../utils/api')

// 科室服务
const DeptService = {
  // 获取所有科室（平面列表）
  getAll() {
    return api.get('/api/department')
  },

  // 获取指定院区的科室树
  getTree(campusId) {
    return api.get(`/api/department/tree/${campusId}`)
  }
}

module.exports = DeptService

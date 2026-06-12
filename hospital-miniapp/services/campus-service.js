const { api } = require('../utils/api')

// 院区服务
const CampusService = {
  // 获取所有院区
  getAll() {
    return api.get('/api/campus')
  },

  // 获取活跃院区
  getActive() {
    return api.get('/api/campus/active')
  }
}

module.exports = CampusService

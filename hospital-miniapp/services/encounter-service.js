const { api } = require('../utils/api')

// 就诊（接诊队列）服务
const EncounterService = {
  // 获取门诊队列
  getQueue(doctorId, date) {
    return api.get('/api/encounter/queue', { doctorId, date })
  }
}

module.exports = EncounterService

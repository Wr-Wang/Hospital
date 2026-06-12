// API 基础配置
const CONFIG = {
  // 后端 API 基础地址
  API_BASE: 'http://192.168.31.20:8080',

  // 小程序 AppID
  APP_ID: 'wx71380c520e3e0777',

  // 预约相关
  MAX_PATIENTS: 5,
  ADVANCE_DAYS: 7,
  CANCEL_HOURS: 2,

  // 后端返回的挂号状态 → 前端显示映射
  REG_STATUS: {
    '已挂号': { text: '待就诊', type: 'tag--primary' },
    '已就诊': { text: '已就诊', type: 'tag--success' },
    '已退号': { text: '已取消', type: 'tag--danger' }
  }
}

module.exports = CONFIG

const CONFIG = require('./constants')

const Format = {
  // 日期格式化：yyyy-MM-dd
  date(date) {
    if (!date) return ''
    const d = new Date(date)
    const year = d.getFullYear()
    const month = String(d.getMonth() + 1).padStart(2, '0')
    const day = String(d.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  },

  // 日期时间格式化：yyyy-MM-dd HH:mm
  datetime(date) {
    if (!date) return ''
    const d = new Date(date)
    return `${Format.date(d)} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
  },

  // 从 ISO 日期提取 HH:mm
  time(date) {
    if (!date) return ''
    const d = new Date(date)
    return `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
  },

  // 手机号脱敏
  maskPhone(phone) {
    if (!phone) return ''
    return phone.replace(/(\d{3})\d{4}(\d{4})/, '$1****$2')
  },

  // 身份证脱敏
  maskIdCard(idCard) {
    if (!idCard || idCard.length < 10) return idCard
    return idCard.slice(0, 3) + '***********' + idCard.slice(-4)
  },

  // 金额格式化
  money(amount) {
    if (amount == null) return '¥0.00'
    return `¥${Number(amount).toFixed(2)}`
  },

  // 性别映射（后端可能返回 1/2 或 "男"/"女"）
  gender(val) {
    if (val === 1 || val === '男') return '男'
    if (val === 2 || val === '女') return '女'
    return '未知'
  },

  // 后端中文挂号状态 → 前端显示
  appointmentStatus(statusText) {
    return CONFIG.REG_STATUS[statusText] || { text: statusText || '未知', type: '' }
  },

  // 获取本周日期列表
  getWeekDays(count = 7) {
    const days = []
    const today = new Date()
    const weekText = ['日', '一', '二', '三', '四', '五', '六']
    for (let i = 0; i < count; i++) {
      const d = new Date(today)
      d.setDate(today.getDate() + i)
      days.push({
        date: Format.date(d),
        dayOfMonth: d.getDate(),
        dayOfWeek: weekText[d.getDay()],
        isToday: i === 0,
        full: d.getMonth() + 1 + '月' + d.getDate() + '日'
      })
    }
    return days
  }
}

module.exports = Format

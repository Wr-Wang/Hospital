// 本地存储封装
const Storage = {
  get(key) {
    try {
      return wx.getStorageSync(key)
    } catch {
      return null
    }
  },

  set(key, value) {
    try {
      wx.setStorageSync(key, value)
    } catch (e) {
      console.error('Storage set error:', e)
    }
  },

  remove(key) {
    try {
      wx.removeStorageSync(key)
    } catch {
      // ignore
    }
  },

  clear() {
    try {
      wx.clearStorageSync()
    } catch {
      // ignore
    }
  }
}

module.exports = Storage

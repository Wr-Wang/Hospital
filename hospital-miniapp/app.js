const Storage = require('./utils/storage')

App({
  globalData: {
    userInfo: null,
    token: '',
    refreshToken: '',
    patients: [],
    currentPatient: null
  },

  onLaunch() {
    const token = Storage.get('token')
    const refreshToken = Storage.get('refreshToken')
    const userInfo = Storage.get('userInfo')
    if (token) {
      this.globalData.token = token
      this.globalData.refreshToken = refreshToken || ''
      this.globalData.userInfo = userInfo
    }
  },

  setUserInfo(userInfo, token, refreshToken) {
    this.globalData.userInfo = userInfo
    this.globalData.token = token
    this.globalData.refreshToken = refreshToken || ''
    Storage.set('userInfo', userInfo)
    Storage.set('token', token)
    if (refreshToken) {
      Storage.set('refreshToken', refreshToken)
    }
  },

  logout() {
    this.globalData.userInfo = null
    this.globalData.token = ''
    this.globalData.refreshToken = ''
    this.globalData.currentPatient = null
    Storage.remove('userInfo')
    Storage.remove('token')
    Storage.remove('refreshToken')
    Storage.remove('patients')
  }
})

const WeChatAuthService = require('../../services/wechat-auth-service')
const Storage = require('../../utils/storage')

Page({
  data: {
    loading: false,
    errorMsg: '',
    // 新患者姓名/手机号输入
    showNameInput: false,
    newName: '',
    newPhone: '',
    // 流程中暂存的 tempToken
    tempToken: ''
  },

  onLoad() {
    const token = wx.getStorageSync('token')
    if (token) {
      wx.redirectTo({ url: '/pages/index/index' })
    }
  },

  // 微信一键登录
  async handleWeChatLogin() {
    this.setData({ loading: true, errorMsg: '' })

    try {
      // Step 1: wx.login 获取 code
      const loginRes = await new Promise((resolve, reject) => {
        wx.login({ success: resolve, fail: reject })
      })
      if (!loginRes.code) {
        throw new Error('微信登录失败，请重试')
      }

      // Step 2: code → 后端登录（已有绑定则直接返回 JWT）
      const result = await WeChatAuthService.login(loginRes.code)

      if (result.isNew) {
        // 未绑定微信账号，需要创建新患者
        this.setData({
          loading: false,
          showNameInput: true,
          tempToken: result.tempToken
        })
      } else {
        // 已有绑定，直接登录成功
        this.completeLogin(result)
      }

    } catch (err) {
      this.setData({ errorMsg: err.message || '登录失败，请重试', loading: false })
    }
  },

  // 姓名输入
  onNameInput(e) {
    this.setData({ newName: e.detail.value })
  },

  // 手机号输入
  onPhoneInput(e) {
    this.setData({ newPhone: e.detail.value })
  },

  async onConfirmName() {
    const name = this.data.newName.trim()
    if (!name) {
      this.setData({ errorMsg: '请输入姓名' })
      return
    }

    const phone = this.data.newPhone.trim()
    if (!phone || phone.length < 11) {
      this.setData({ errorMsg: '请输入正确的手机号' })
      return
    }

    this.setData({ loading: true, errorMsg: '' })

    try {
      const result = await WeChatAuthService.createPatient(this.data.tempToken, name, phone)
      this.setData({ showNameInput: false })
      this.completeLogin(result)
    } catch (err) {
      this.setData({ errorMsg: err.message || '创建失败', loading: false })
    }
  },

  // 登录成功处理
  completeLogin(result) {
    const app = getApp()
    const userInfo = {
      name: result.name || '',
      patientNo: result.patientNo || '',
      patientId: result.patientId,
      phone: result.phone || ''
    }
    app.setUserInfo(userInfo, result.accessToken, result.refreshToken)

    // 同步到就诊人列表
    if (result.patientId) {
      const patients = Storage.get('patients') || []
      if (!patients.find(p => String(p.id) === String(result.patientId))) {
        patients.push({
          id: result.patientId,
          name: userInfo.name,
          patientNo: userInfo.patientNo,
          phone: userInfo.phone,
          isDefault: patients.length === 0
        })
        Storage.set('patients', patients)
      }
    }

    wx.showToast({ title: '登录成功', icon: 'success' })

    if (result.isNew) {
      wx.redirectTo({ url: '/pages/profile/patients/edit?isNew=true' })
    } else {
      wx.redirectTo({ url: '/pages/index/index' })
    }
  },

  // 暂不登录
  onSkipLogin() {
    wx.redirectTo({ url: '/pages/index/index' })
  }
})

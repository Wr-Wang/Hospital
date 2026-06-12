const CampusService = require('../../services/campus-service')
const DeptService = require('../../services/dept-service')

const DEPT_ICONS = {
  '呼吸内科': '🫁', '消化内科': '🫃', '心血管内科': '❤️',
  '普外科': '🔪', '骨科': '🦴',
  '儿科': '👶', '妇产科': '🤰', '眼科': '👁️',
  '耳鼻喉科': '👂', '皮肤科': '🧴', '口腔科': '🦷',
  '中医科': '🌿'
}
const DEFAULT_ICON = '🏥'

Page({
  data: {
    campuses: [],
    currentCampusIndex: 0,
    keyword: '',
    allDepts: [],
    filteredDepts: [],
    loading: true
  },

  onLoad(options) {
    const keyword = decodeURIComponent(options.keyword || '')
    this.setData({ keyword })
    this.loadData()
  },

  async loadData() {
    this.setData({ loading: true })
    try {
      const [campuses, depts] = await Promise.all([
        CampusService.getActive(),
        DeptService.getAll()
      ])

      const campusList = Array.isArray(campuses) ? campuses : []
      let deptList = Array.isArray(depts) ? depts : []

      // 只保留叶子科室（有 parentId 的）
      deptList = deptList.filter(d => d.parentId != null && d.isActive !== false)
        .map(d => ({
          id: d.id,
          name: d.name,
          campusId: d.campusId,
          icon: DEPT_ICONS[d.name] || DEFAULT_ICON,
          desc: d.name + (d.type ? ' · ' + d.type : '')
        }))

      this.setData({
        campuses: campusList,
        allDepts: deptList,
        loading: false
      })
      this.filterData()
    } catch (err) {
      this.setData({ loading: false })
      wx.showToast({ title: '加载科室失败', icon: 'none' })
    }
  },

  filterData() {
    const { campuses, currentCampusIndex, allDepts, keyword } = this.data
    const campusId = campuses[currentCampusIndex]?.id

    let result = allDepts
    if (campusId) {
      result = result.filter(d => d.campusId === campusId)
    }
    if (keyword) {
      result = result.filter(d => d.name.includes(keyword))
    }

    this.setData({ filteredDepts: result })
  },

  switchCampus(e) {
    this.setData({ currentCampusIndex: e.currentTarget.dataset.index }, () => this.filterData())
  },

  onSearch(e) {
    this.setData({ keyword: e.detail.value.trim() }, () => this.filterData())
  },

  goToSchedule(e) {
    const { id, name } = e.currentTarget.dataset
    wx.navigateTo({ url: `/pages/doctor/schedule?deptId=${id}&deptName=${encodeURIComponent(name)}` })
  }
})

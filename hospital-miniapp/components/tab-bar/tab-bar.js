Component({
  properties: {
    current: {
      type: Number,
      value: 0
    }
  },

  data: {
    list: [
      { icon: '🏠', activeIcon: '🏠', text: '首页', page: '/pages/index/index' },
      { icon: '📋', activeIcon: '📋', text: '预约', page: '/pages/appointment/records' },
      { icon: '👤', activeIcon: '👤', text: '我的', page: '/pages/profile/profile' }
    ]
  },

  methods: {
    onTabTap(e) {
      const index = e.currentTarget.dataset.index
      const page = this.data.list[index].page
      if (index !== this.properties.current) {
        wx.redirectTo({
          url: page
        })
      }
    }
  }
})

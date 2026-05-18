<template>
  <n-layout position="absolute" style="height: 100vh">
    <n-layout-header bordered style="height: 48px; display: flex; align-items: center; padding: 0 16px; justify-content: space-between">
      <div style="display: flex; align-items: center; gap: 12px">
        <span style="font-size: 16px; font-weight: 600">医院信息管理系统</span>
      </div>
      <div style="display: flex; align-items: center; gap: 12px">
        <n-tooltip trigger="hover">
          <template #trigger>
            <n-tag>{{ auth.campusName }}</n-tag>
          </template>
          <span>{{ auth.displayName }}</span>
        </n-tooltip>
        <n-button quaternary size="small" @click="handleLogout">退出</n-button>
      </div>
    </n-layout-header>

    <n-layout has-sider position="absolute" style="top: 48px; bottom: 0">
      <n-layout-sider
        bordered
        :width="200"
        :native-scrollbar="false"
        style="padding-top: 4px"
      >
        <n-menu
          :options="menuOptions"
          :collapsed-width="64"
          :value="activeKey"
          @update:value="handleMenuUpdate"
        />
      </n-layout-sider>

      <n-layout-content style="padding: 16px; overflow-y: auto">
        <router-view />
      </n-layout-content>
    </n-layout>
  </n-layout>
</template>

<script setup lang="ts">
import { computed, h } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { NIcon } from 'naive-ui'
import {
  DashboardOutlined,
  DatabaseOutlined,
  UserOutlined,
  CalendarOutlined,
  SettingOutlined,
} from '@vicons/antd'
import type { MenuOption } from 'naive-ui'
import { useAuthStore } from '../stores/auth'
import type { MenuItem } from '../types'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()

const iconMap: Record<string, any> = {
  dashboard: DashboardOutlined,
  database: DatabaseOutlined,
  user: UserOutlined,
  calendar: CalendarOutlined,
  settings: SettingOutlined,
}

function renderIcon(icon: string) {
  const IconComp = iconMap[icon]
  if (!IconComp) return undefined
  return () => h(NIcon, null, { default: () => h(IconComp) })
}

function convertToMenuOptions(items: MenuItem[]): MenuOption[] {
  return items.map((item) => {
    const option: MenuOption = {
      key: item.key,
      label: item.label,
      icon: item.icon ? renderIcon(item.icon) : undefined,
    }
    if (item.children && item.children.length > 0) {
      option.children = convertToMenuOptions(item.children)
    }
    return option
  })
}

const menuOptions = computed(() => convertToMenuOptions(auth.menus))

const activeKey = computed(() => {
  const path = route.path
  // 尝试匹配菜单 key
  for (const item of auth.menus) {
    if (item.path === path) return item.key
    if (item.children) {
      for (const child of item.children) {
        if (child.path === path) return child.key
      }
    }
  }
  return 'shell.home'
})

function handleMenuUpdate(key: string, _item: MenuOption) {
  // 从菜单配置中找到对应路径
  function findPath(items: MenuItem[], targetKey: string): string | undefined {
    for (const item of items) {
      if (item.key === targetKey) return item.path
      if (item.children) {
        const found = findPath(item.children, targetKey)
        if (found) return found
      }
    }
    return undefined
  }
  const path = findPath(auth.menus, key)
  if (path) {
    router.push(path)
  }
}

function handleLogout() {
  auth.logout()
  router.push('/login')
}
</script>

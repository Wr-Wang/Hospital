import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { UserInfo, MenuItem } from '../types'
import { loginApi } from '../api/auth'
import { MenuConfig } from '../utils/constants'

/** 解码 JWT payload，提取用户信息 */
function decodeToken(token: string): { id: number; permissions: string[] } {
  try {
    const payload = token.split('.')[1]
    const decoded = JSON.parse(atob(payload))
    // permissions 在后端 JwtTokenService 中以逗号分隔存储
    const permissions: string[] = decoded.permissions
      ? decoded.permissions.split(',').filter(Boolean)
      : []
    const id = parseInt(decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || '0', 10)
    return { id, permissions }
  } catch {
    return { id: 0, permissions: [] }
  }
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const userInfo = ref<UserInfo | null>(
    JSON.parse(localStorage.getItem('userInfo') || 'null'),
  )

  const isLoggedIn = computed(() => !!token.value && !!userInfo.value)
  const displayName = computed(() => userInfo.value?.displayName ?? '')
  const campusName = computed(() => userInfo.value?.campusName ?? '')
  const userPermissions = computed(() => userInfo.value?.permissions ?? [])

  /** 判断是否拥有某权限 */
  function hasPermission(permission?: string): boolean {
    if (!permission) return true
    if (!userInfo.value?.permissions) return false
    return userInfo.value.permissions.includes(permission)
  }

  /** 根据用户权限过滤菜单 */
  const menus = computed<MenuItem[]>(() => {
    function filterMenus(items: typeof MenuConfig): MenuItem[] {
      return items
        .filter((item) => hasPermission(item.permission))
        .map((item) => ({
          key: item.key,
          label: item.label,
          icon: item.icon,
          path: item.path,
          permission: item.permission,
          children: item.children ? filterMenus(item.children) : undefined,
        }))
    }
    return filterMenus(MenuConfig)
  })

  /** 登录 */
  async function login(username: string, password: string) {
    const res = await loginApi({ username, password })
    const data = res.data

    if (!data.token) {
      throw new Error('登录失败：未获取到 Token')
    }

    // 从 JWT 中解码用户 ID 和权限
    const { id, permissions } = decodeToken(data.token)

    const info: UserInfo = {
      id,
      displayName: data.displayName,
      campusName: data.campusName,
      roles: data.roles,
      permissions,
    }

    token.value = data.token
    userInfo.value = info
    localStorage.setItem('token', data.token)
    localStorage.setItem('userInfo', JSON.stringify(info))
  }

  /** 登出 */
  function logout() {
    token.value = null
    userInfo.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('userInfo')
  }

  return {
    token,
    userInfo,
    isLoggedIn,
    displayName,
    campusName,
    userPermissions,
    menus,
    hasPermission,
    login,
    logout,
  }
})

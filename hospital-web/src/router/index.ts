import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '../stores/auth'

// 路由级权限映射
const routePermissions: Record<string, string> = {
  Dashboard: 'sys.shell.use',
  Campus: 'mdm.campus.manage',
  Department: 'mdm.dept.manage',
  Staff: 'mdm.staff.manage',
  Dictionary: 'mdm.dict.manage',
  PatientCreate: 'pat.register',
  PatientSearch: 'pat.search',
  PatientDetail: 'pat.search',
  Schedule: 'opd.schedule',
  Users: 'sys.security.manage',
  Roles: 'sys.security.manage',
}

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../pages/login/index.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/',
    component: () => import('../components/AppLayout.vue'),
    meta: { requiresAuth: true },
    redirect: '/dashboard',
    children: [
      { path: 'dashboard', name: 'Dashboard', component: () => import('../pages/dashboard/index.vue') },
      // 主数据
      { path: 'campus', name: 'Campus', component: () => import('../pages/campus/index.vue') },
      { path: 'department', name: 'Department', component: () => import('../pages/department/index.vue') },
      { path: 'staff', name: 'Staff', component: () => import('../pages/staff/index.vue') },
      { path: 'dictionary', name: 'Dictionary', component: () => import('../pages/dictionary/index.vue') },
      // 患者
      { path: 'patient/create', name: 'PatientCreate', component: () => import('../pages/patient/create.vue') },
      { path: 'patient/search', name: 'PatientSearch', component: () => import('../pages/patient/search.vue') },
      { path: 'patient/detail/:id', name: 'PatientDetail', component: () => import('../pages/patient/detail.vue') },
      // 排班
      { path: 'schedule', name: 'Schedule', component: () => import('../pages/schedule/index.vue') },
      // 系统
      { path: 'users', name: 'Users', component: () => import('../pages/user/index.vue') },
      { path: 'roles', name: 'Roles', component: () => import('../pages/role/index.vue') },
    ],
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('../pages/dashboard/index.vue'),
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// 导航守卫：未登录跳转登录页 + 无权限跳转 403
router.beforeEach((to) => {
  if (to.meta.requiresAuth === false) return true

  const auth = useAuthStore()
  if (!auth.isLoggedIn) {
    return `/login?redirect=${encodeURIComponent(to.fullPath)}`
  }

  // 权限检查
  const name = to.name as string | undefined
  if (name) {
    const requiredPerm = routePermissions[name]
    if (requiredPerm && !auth.hasPermission(requiredPerm)) {
      return '/dashboard'
    }
  }

  return true
})

export default router

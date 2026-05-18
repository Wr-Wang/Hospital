import type { MenuItem } from '../types'

/** 路由键常量，与后端 RouteKeys 一一对应 */
export const RouteKeys = {
  Home: 'shell.home',

  // M1 主数据
  Campus: 'mdm.campus',
  Department: 'mdm.dept',
  Staff: 'mdm.staff',
  Dictionary: 'mdm.dict',

  // M2 患者
  PatientRegister: 'pat.register',
  PatientSearch: 'pat.search',
  Patient360: 'pat.360',

  // M3 排班
  Schedule: 'opd.schedule',

  // M13 系统
  UserRole: 'sys.userrole',
} as const

/** 权限标识常量，与后端 Permissions.cs 一一对应 */
export const Permissions = {
  ShellUse: 'sys.shell.use',
  SecurityManage: 'sys.security.manage',

  CampusManage: 'mdm.campus.manage',
  DeptManage: 'mdm.dept.manage',
  StaffManage: 'mdm.staff.manage',
  DictManage: 'mdm.dict.manage',

  PatientRegister: 'pat.register',
  PatientSearch: 'pat.search',

  Schedule: 'opd.schedule',
} as const

/** 菜单定义：路径 → 权限映射（仅包含管理端功能） */
export const MenuConfig: MenuItem[] = [
  {
    key: RouteKeys.Home,
    label: '首页',
    icon: 'dashboard',
    path: '/dashboard',
    permission: Permissions.ShellUse,
  },
  {
    key: 'mdm',
    label: '主数据',
    icon: 'database',
    permission: Permissions.ShellUse,
    children: [
      { key: RouteKeys.Campus, label: '院区管理', path: '/campus', permission: Permissions.CampusManage },
      { key: RouteKeys.Department, label: '科室管理', path: '/department', permission: Permissions.DeptManage },
      { key: RouteKeys.Staff, label: '人员管理', path: '/staff', permission: Permissions.StaffManage },
      { key: RouteKeys.Dictionary, label: '字典管理', path: '/dictionary', permission: Permissions.DictManage },
    ],
  },
  {
    key: 'pat',
    label: '患者管理',
    icon: 'user',
    permission: Permissions.ShellUse,
    children: [
      { key: RouteKeys.PatientRegister, label: '患者建档', path: '/patient/create', permission: Permissions.PatientRegister },
      { key: RouteKeys.PatientSearch, label: '患者检索', path: '/patient/search', permission: Permissions.PatientSearch },
    ],
  },
  {
    key: RouteKeys.Schedule,
    label: '排班管理',
    icon: 'calendar',
    path: '/schedule',
    permission: Permissions.Schedule,
  },
  {
    key: 'sys',
    label: '系统管理',
    icon: 'settings',
    permission: Permissions.SecurityManage,
    children: [
      { key: 'sys.users', label: '用户管理', path: '/users', permission: Permissions.SecurityManage },
      { key: 'sys.roles', label: '角色管理', path: '/roles', permission: Permissions.SecurityManage },
    ],
  },
]

/** API 基础路径 */
export const API_BASE_URL = '/api'

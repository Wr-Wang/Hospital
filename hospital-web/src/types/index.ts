// ===== 认证相关（与后端 API 实际返回格式对齐） =====
export interface AuthenticationRequest {
  username: string
  password: string
}

/** 后端登录接口实际返回格式 */
export interface LoginResponse {
  token: string
  displayName: string
  campusName: string
  roles: string[]
}

/** 前端解码 JWT 后提取的用户信息 */
export interface UserInfo {
  id: number
  displayName: string
  campusName: string
  roles: string[]
  permissions: string[]
}

// ===== 菜单项 =====
export interface MenuItem {
  key: string
  label: string
  icon?: string
  path?: string
  children?: MenuItem[]
  permission?: string
}

// ===== 院区 =====
export interface CampusDto {
  id: number
  code: string
  name: string
  address?: string
  phone?: string
  isActive: boolean
}

export interface CreateCampusDto {
  code: string
  name: string
  address?: string
  phone?: string
}

// ===== 科室 =====
export interface DepartmentDto {
  id: number
  code: string
  name: string
  parentId?: number
  campusId: number
  type: string
  isActive: boolean
  children: DepartmentDto[]
}

export interface CreateDepartmentDto {
  code: string
  name: string
  campusId: number
  type: string
  parentId?: number
}

// ===== 人员 =====
export interface StaffDto {
  id: number
  code: string
  name: string
  gender: string
  phone?: string
  campusId: number
  deptId: number
  licenseType: string
  licenseNo: string
  licenseExpiry?: string
  isActive: boolean
  isLicenseExpired: boolean
}

export interface CreateStaffDto {
  code: string
  name: string
  gender: string
  phone?: string
  campusId: number
  deptId: number
  licenseType: string
  licenseNo: string
  licenseExpiry?: string
}

// ===== 字典 =====
export interface DictionaryTypeDto {
  id: number
  code: string
  name: string
  description?: string
  isActive: boolean
}

export interface DictionaryItemDto {
  id: number
  typeId: number
  code: string
  name: string
  parentId?: number
  sortOrder: number
  isActive: boolean
}

// ===== 患者 =====
export interface PatientDto {
  id: number
  patientNo: string
  name: string
  gender?: string
  birthDate?: string
  phone?: string
  allergiesText?: string
  idCard?: string
}

export interface CreatePatientDto {
  patientNo: string
  name: string
  gender?: string
  birthDate?: string
  phone?: string
  allergiesText?: string
  idCard?: string
}

export interface PatientSearchResultDto {
  items: PatientDto[]
  totalCount: number
  page: number
  size: number
}

// ===== 排班 =====
export interface ScheduleDto {
  id: number
  doctorId: number
  deptId: number
  campusId: number
  scheduleDate: string
  status: string
  slots: ScheduleSlotDto[]
}

export interface ScheduleSlotDto {
  id: number
  slotName: string
  startTime: string
  endTime: string
  totalQuota: number
  bookedQuota: number
  availableQuota: number
}

export interface CreateScheduleDto {
  doctorId: number
  deptId: number
  campusId: number
  scheduleDate: string
  slots: CreateScheduleSlotDto[]
}

export interface CreateScheduleSlotDto {
  slotName: string
  startTime: string
  endTime: string
  totalQuota: number
}

// ===== 挂号（用于患者就诊历史展示） =====
export interface RegistrationDto {
  id: number
  patientId: number
  scheduleId: number
  doctorId: number
  deptId: number
  campusId: number
  registerTime: string
  queueNumber: number
  slotName: string
  status: string
}

// ===== 用户角色 =====
export interface UserDto {
  id: number
  loginName: string
  displayName: string
  campusName: string
  isActive: boolean
  roles: string[]
}

export interface CreateUserDto {
  loginName: string
  password: string
  displayName: string
  campusName: string
  roles: string[]
}

export interface RoleDto {
  id: number
  name: string
  description: string
  permissions: string[]
}

export interface CreateRoleDto {
  name: string
  description: string
  permissions: string[]
}

// ===== 通用 =====
export interface PageResult<T> {
  items: T[]
  totalCount: number
  page: number
  size: number
}

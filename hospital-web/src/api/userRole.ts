import request from './request'
import type { UserDto, RoleDto, CreateUserDto, UpdateUserDto, CreateRoleDto, UpdateRoleDto } from '../types'

// ===== 用户 =====
export function getUsers() {
  return request.get<UserDto[]>('/User')
}

export function createUser(data: CreateUserDto) {
  return request.post<{ id: number }>('/User', data)
}

export function updateUser(id: number, data: UpdateUserDto) {
  return request.put(`/User/${id}`, data)
}

// ===== 角色 =====
export function getRoles() {
  return request.get<RoleDto[]>('/Role')
}

export function createRole(data: CreateRoleDto) {
  return request.post<{ id: number }>('/Role', data)
}

export function updateRole(id: number, data: UpdateRoleDto) {
  return request.put(`/Role/${id}`, data)
}

export function deleteRole(id: number) {
  return request.delete(`/Role/${id}`)
}

import request from './request'
import type { DepartmentDto, CreateDepartmentDto } from '../types'

export function getDepartmentList() {
  return request.get<DepartmentDto[]>('/Department')
}

export function getDepartmentTree(campusId: number) {
  return request.get<DepartmentDto[]>(`/Department/tree/${campusId}`)
}

export function getDepartmentById(id: number) {
  return request.get<DepartmentDto>(`/Department/${id}`)
}

export function createDepartment(data: CreateDepartmentDto) {
  return request.post<{ id: number }>('/Department', data)
}

export function updateDepartment(id: number, data: { name: string; type: string; parentId?: number | null }) {
  return request.put(`/Department/${id}`, data)
}

export function activateDepartment(id: number) {
  return request.patch(`/Department/${id}/activate`)
}

export function deactivateDepartment(id: number) {
  return request.patch(`/Department/${id}/deactivate`)
}

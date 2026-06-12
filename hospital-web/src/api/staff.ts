import request from './request'
import type { StaffDto, CreateStaffDto } from '../types'

export function getStaffList() {
  return request.get<StaffDto[]>('/Staff')
}

export function getStaffByCampus(campusId: number) {
  return request.get<StaffDto[]>(`/Staff/by-campus/${campusId}`)
}

export function getStaffByDept(deptId: number) {
  return request.get<StaffDto[]>(`/Staff/by-dept/${deptId}`)
}

export function getStaffById(id: number) {
  return request.get<StaffDto>(`/Staff/${id}`)
}

export function createStaff(data: CreateStaffDto) {
  return request.post<{ id: number }>('/Staff', data)
}

export function updateStaff(id: number, data: { name: string; gender: string; phone?: string; deptId: number }) {
  return request.put(`/Staff/${id}`, data)
}

export function updateStaffLicense(id: number, data: { licenseType: string; licenseNo: string; licenseExpiry?: string }) {
  return request.patch(`/Staff/${id}/license`, data)
}

export function activateStaff(id: number) {
  return request.patch(`/Staff/${id}/activate`)
}

export function deactivateStaff(id: number) {
  return request.patch(`/Staff/${id}/deactivate`)
}

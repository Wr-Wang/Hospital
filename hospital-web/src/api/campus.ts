import request from './request'
import type { CampusDto, CreateCampusDto } from '../types'

export function getCampusList() {
  return request.get<CampusDto[]>('/Campus')
}

export function getActiveCampuses() {
  return request.get<CampusDto[]>('/Campus/active')
}

export function getCampusById(id: number) {
  return request.get<CampusDto>(`/Campus/${id}`)
}

export function createCampus(data: CreateCampusDto) {
  return request.post<{ id: number }>('/Campus', data)
}

export function updateCampus(id: number, data: { name: string; address?: string; phone?: string }) {
  return request.put(`/Campus/${id}`, data)
}

export function activateCampus(id: number) {
  return request.patch(`/Campus/${id}/activate`)
}

export function deactivateCampus(id: number) {
  return request.patch(`/Campus/${id}/deactivate`)
}

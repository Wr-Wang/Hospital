import request from './request'
import type { ScheduleDto } from '../types'

export function getScheduleById(id: number) {
  return request.get<ScheduleDto>(`/Schedule/${id}`)
}

export function getScheduleByDoctor(doctorId: number) {
  return request.get<ScheduleDto[]>(`/Schedule/by-doctor/${doctorId}`)
}

export function getScheduleByDept(deptId: number, date?: string) {
  return request.get<ScheduleDto[]>('/Schedule/by-dept/' + deptId, {
    params: { date },
  })
}

export function createSchedule(data: {
  doctorId: number
  deptId: number
  campusId: number
  scheduleDate: string
  slots: { slotName: string; startTime: string; endTime: string; totalQuota: number }[]
}) {
  return request.post<{ id: number }>('/Schedule', data)
}

export function publishSchedule(id: number) {
  return request.patch(`/Schedule/${id}/publish`)
}

export function deactivateSchedule(id: number) {
  return request.patch(`/Schedule/${id}/deactivate`)
}

export function updateSlotQuota(id: number, slotName: string, totalQuota: number) {
  return request.put(`/Schedule/${id}/slot-quota`, { slotName, totalQuota })
}

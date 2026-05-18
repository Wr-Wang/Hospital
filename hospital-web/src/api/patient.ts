import request from './request'
import type { PatientDto, PatientProfileDto, PatientSearchResultDto } from '../types'

export function getPatientById(id: number) {
  return request.get<PatientDto>(`/Patient/${id}`)
}

export function getPatientByPatientNo(patientNo: string) {
  return request.get<PatientDto>(`/Patient/by-patient-no/${encodeURIComponent(patientNo)}`)
}

export function getPatientByIdCard(idCard: string) {
  return request.get<PatientDto>(`/Patient/by-idcard/${encodeURIComponent(idCard)}`)
}

export function getSuspectDuplicates(name: string, phone?: string) {
  return request.post<PatientDto[]>('/Patient/suspect-duplicates', { name, phone })
}

export function searchPatients(keyword?: string, page = 1, size = 20) {
  return request.get<PatientSearchResultDto>('/Patient/search', {
    params: { keyword, page, size },
  })
}

export function getPatientProfile(id: number) {
  return request.get<PatientProfileDto>(`/Patient/${id}/profile`)
}

export function createPatient(data: {
  patientNo: string
  name: string
  gender?: string
  birthDate?: string
  phone?: string
  allergiesText?: string
  idCard?: string
}) {
  return request.post<{ id: number }>('/Patient', data)
}

import request from './request'
import type { DictionaryTypeDto, DictionaryItemDto } from '../types'

// ===== 字典类型 =====
export function getDictionaryTypes() {
  return request.get<DictionaryTypeDto[]>('/Dictionary/types')
}

export function getDictionaryTypeById(id: number) {
  return request.get<DictionaryTypeDto>(`/Dictionary/types/${id}`)
}

export function createDictionaryType(data: { code: string; name: string; description?: string }) {
  return request.post<{ id: number }>('/Dictionary/types', data)
}

export function updateDictionaryType(id: number, data: { name: string; description?: string }) {
  return request.put(`/Dictionary/types/${id}`, data)
}

export function activateDictionaryType(id: number) {
  return request.patch(`/Dictionary/types/${id}/activate`)
}

export function deactivateDictionaryType(id: number) {
  return request.patch(`/Dictionary/types/${id}/deactivate`)
}

// ===== 字典项 =====
export function getDictionaryItems(typeId: number) {
  return request.get<DictionaryItemDto[]>(`/Dictionary/types/${typeId}/items`)
}

export function getDictionaryItemsByCode(typeCode: string) {
  return request.get<DictionaryItemDto[]>(`/Dictionary/types/by-code/${typeCode}/items`)
}

export function createDictionaryItem(data: { typeId: number; code: string; name: string; parentId?: number | null; sortOrder: number }) {
  return request.post<{ id: number }>('/Dictionary/items', data)
}

export function updateDictionaryItem(id: number, data: { name: string; parentId?: number | null; sortOrder: number }) {
  return request.put(`/Dictionary/items/${id}`, data)
}

export function activateDictionaryItem(id: number) {
  return request.patch(`/Dictionary/items/${id}/activate`)
}

export function deactivateDictionaryItem(id: number) {
  return request.patch(`/Dictionary/items/${id}/deactivate`)
}

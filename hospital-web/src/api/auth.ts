import request from './request'
import type { AuthenticationRequest, LoginResponse } from '../types'

/** 登录 */
export function loginApi(data: AuthenticationRequest) {
  return request.post<LoginResponse>('/Authentication/login', data)
}

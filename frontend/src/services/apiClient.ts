import type { ApiResponse, LoginResponse } from '../types/api'
import {
  clearSession,
  getAccessToken,
  getRefreshToken,
  saveSession,
} from './session'

export const API_BASE_URL = (
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5196'
).replace(/\/$/, '')

export class ApiError<T = unknown> extends Error {
  response: ApiResponse<T> | null

  constructor(message: string, response: ApiResponse<T> | null = null) {
    super(message)
    this.name = 'ApiError'
    this.response = response
  }
}

interface RequestOptions extends RequestInit {
  skipAuth?: boolean
  skipRefresh?: boolean
}

let refreshPromise: Promise<void> | null = null

export async function apiRequest<T>(
  path: string,
  options: RequestOptions = {},
): Promise<ApiResponse<T>> {
  const response = await rawRequest<T>(path, options)

  if (
    response.statusCode === 401 &&
    !options.skipAuth &&
    !options.skipRefresh &&
    getRefreshToken()
  ) {
    await refreshAccessToken()
    return rawRequest<T>(path, { ...options, skipRefresh: true })
  }

  if (!response.status) {
    throw new ApiError(response.mensagem || 'Erro ao executar requisicao.', response)
  }

  return response
}

async function rawRequest<T>(
  path: string,
  options: RequestOptions,
): Promise<ApiResponse<T>> {
  const headers = new Headers(options.headers)

  if (options.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }

  if (!options.skipAuth) {
    const token = getAccessToken()
    if (token) {
      headers.set('Authorization', `Bearer ${token}`)
    }
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  })

  const text = await response.text()
  const parsed = text ? (JSON.parse(text) as ApiResponse<T>) : null

  if (parsed && typeof parsed.status === 'boolean') {
    return parsed
  }

  return {
    dados: (parsed as T | null) ?? null,
    mensagem: response.ok ? 'Requisicao concluida.' : response.statusText,
    status: response.ok,
    statusCode: response.status,
  }
}

async function refreshAccessToken(): Promise<void> {
  if (!refreshPromise) {
    refreshPromise = doRefreshAccessToken().finally(() => {
      refreshPromise = null
    })
  }

  return refreshPromise
}

async function doRefreshAccessToken(): Promise<void> {
  const refreshToken = getRefreshToken()
  if (!refreshToken) {
    clearSession()
    throw new ApiError('Sessao expirada.')
  }

  const response = await rawRequest<LoginResponse>('/api/auth/refresh-token', {
    method: 'POST',
    body: JSON.stringify({ refreshToken }),
    skipAuth: true,
    skipRefresh: true,
  })

  if (!response.status || !response.dados) {
    clearSession()
    throw new ApiError(response.mensagem || 'Sessao expirada.', response)
  }

  saveSession(response.dados)
}

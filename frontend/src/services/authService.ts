import type {
  LoginPayload,
  LoginResponse,
  RegisterPayload,
  UpdateUserPayload,
  Usuario,
} from '../types/api'
import { apiRequest } from './apiClient'
import { clearSession } from './session'

export async function login(payload: LoginPayload): Promise<LoginResponse> {
  const response = await apiRequest<LoginResponse>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(payload),
    skipAuth: true,
  })

  if (!response.dados) {
    throw new Error('Login sem dados de sessao.')
  }

  return response.dados
}

export async function register(payload: RegisterPayload): Promise<Usuario> {
  const response = await apiRequest<Usuario>('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify(payload),
    skipAuth: true,
  })

  if (!response.dados) {
    throw new Error('Cadastro sem dados de usuario.')
  }

  return response.dados
}

export async function getMe(): Promise<Usuario> {
  const response = await apiRequest<Usuario>('/api/users/me')

  if (!response.dados) {
    throw new Error('Usuario autenticado nao retornado.')
  }

  return response.dados
}

export async function updateMe(payload: UpdateUserPayload): Promise<Usuario> {
  const response = await apiRequest<Usuario>('/api/users/me', {
    method: 'PUT',
    body: JSON.stringify(payload),
  })

  if (!response.dados) {
    throw new Error('Usuario atualizado nao retornado.')
  }

  return response.dados
}

export async function deleteMe(): Promise<void> {
  await apiRequest<Usuario>('/api/users/me', {
    method: 'DELETE',
  })
  clearSession()
}

export async function listUsers(): Promise<Usuario[]> {
  const response = await apiRequest<Usuario[]>('/api/users')
  return response.dados ?? []
}

export async function logout(): Promise<void> {
  try {
    await apiRequest<string>('/api/auth/logout', {
      method: 'POST',
    })
  } finally {
    clearSession()
  }
}

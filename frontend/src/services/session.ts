import type { LoginResponse, Usuario } from '../types/api'

const ACCESS_TOKEN_KEY = 'apiusuarios.accessToken'
const REFRESH_TOKEN_KEY = 'apiusuarios.refreshToken'
const USER_KEY = 'apiusuarios.user'

export interface SessionState {
  accessToken: string
  refreshToken: string
  user: Usuario
}

export function getSession(): SessionState | null {
  const accessToken = localStorage.getItem(ACCESS_TOKEN_KEY)
  const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY)
  const user = localStorage.getItem(USER_KEY)

  if (!accessToken || !refreshToken || !user) {
    return null
  }

  try {
    return {
      accessToken,
      refreshToken,
      user: JSON.parse(user) as Usuario,
    }
  } catch {
    clearSession()
    return null
  }
}

export function saveSession(login: LoginResponse): SessionState {
  localStorage.setItem(ACCESS_TOKEN_KEY, login.token)
  localStorage.setItem(REFRESH_TOKEN_KEY, login.refreshToken)
  localStorage.setItem(USER_KEY, JSON.stringify(login.usuario))

  return {
    accessToken: login.token,
    refreshToken: login.refreshToken,
    user: login.usuario,
  }
}

export function saveUser(user: Usuario): void {
  localStorage.setItem(USER_KEY, JSON.stringify(user))
}

export function clearSession(): void {
  localStorage.removeItem(ACCESS_TOKEN_KEY)
  localStorage.removeItem(REFRESH_TOKEN_KEY)
  localStorage.removeItem(USER_KEY)
}

export function getAccessToken(): string | null {
  return localStorage.getItem(ACCESS_TOKEN_KEY)
}

export function getRefreshToken(): string | null {
  return localStorage.getItem(REFRESH_TOKEN_KEY)
}

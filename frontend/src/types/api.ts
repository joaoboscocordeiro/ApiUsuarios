export type Role = 'User' | 'Admin' | string

export interface ApiResponse<T> {
  dados: T | null
  mensagem: string
  status: boolean
  statusCode: number
}

export interface Usuario {
  id: number
  usuario: string
  nome: string
  sobrenome: string
  email: string
  role: Role
  dataCriacao: string
  dataAlteracao: string
}

export interface LoginResponse {
  usuario: Usuario
  token: string
  tokenExpiracao: string
  refreshToken: string
  refreshTokenExpiracao: string
}

export interface LoginPayload {
  email: string
  senha: string
}

export interface RegisterPayload {
  usuario: string
  nome: string
  sobrenome: string
  email: string
  senha: string
  confirmaSenha: string
}

export interface UpdateUserPayload {
  usuario: string
  nome: string
  sobrenome: string
  email: string
}

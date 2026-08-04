import { useEffect, useMemo, useState } from 'react'
import type { FormEvent, ReactNode } from 'react'
import {
  ApiError,
  API_BASE_URL,
} from './services/apiClient'
import {
  deleteMe,
  getMe,
  listUsers,
  login,
  logout,
  register,
  updateMe,
} from './services/authService'
import {
  clearSession,
  getSession,
  saveSession,
  saveUser,
  type SessionState,
} from './services/session'
import type {
  LoginPayload,
  RegisterPayload,
  UpdateUserPayload,
  Usuario,
} from './types/api'

type View = 'login' | 'register' | 'account' | 'admin'
type Notice = { type: 'success' | 'error' | 'info'; text: string } | null

const initialLogin: LoginPayload = {
  email: '',
  senha: '',
}

const initialRegister: RegisterPayload = {
  usuario: '',
  nome: '',
  sobrenome: '',
  email: '',
  senha: '',
  confirmaSenha: '',
}

function App() {
  const [session, setSession] = useState<SessionState | null>(() => getSession())
  const [view, setView] = useState<View>(() => getInitialView(getSession()))
  const [notice, setNotice] = useState<Notice>(null)
  const [loading, setLoading] = useState(false)
  const [loginForm, setLoginForm] = useState<LoginPayload>(initialLogin)
  const [registerForm, setRegisterForm] =
    useState<RegisterPayload>(initialRegister)
  const [accountForm, setAccountForm] = useState<UpdateUserPayload>({
    usuario: '',
    nome: '',
    sobrenome: '',
    email: '',
  })
  const [users, setUsers] = useState<Usuario[]>([])

  const currentUser = session?.user ?? null
  const isAdmin = currentUser?.role === 'Admin'

  useEffect(() => {
    if (!getSession()) {
      return
    }

    setLoading(true)
    getMe()
      .then((user) => {
        saveUser(user)
        setSession((current) => (current ? { ...current, user } : null))
        setNotice(null)
      })
      .catch((error: unknown) => {
        clearSession()
        setSession(null)
        setView('login')
        setNotice({ type: 'error', text: getErrorMessage(error) })
      })
      .finally(() => {
        setLoading(false)
      })
  }, [])

  useEffect(() => {
    if (!currentUser) {
      setAccountForm({ usuario: '', nome: '', sobrenome: '', email: '' })
      return
    }

    setAccountForm({
      usuario: currentUser.usuario,
      nome: currentUser.nome,
      sobrenome: currentUser.sobrenome,
      email: currentUser.email,
    })
  }, [currentUser])

  useEffect(() => {
    if (view === 'admin' && isAdmin) {
      void loadUsers()
    }
  }, [view, isAdmin])

  const subtitle = useMemo(() => {
    if (!currentUser) {
      return 'Entre ou crie uma conta para acessar o Auth Service.'
    }

    return `${currentUser.nome} ${currentUser.sobrenome} - ${currentUser.role}`
  }, [currentUser])

  async function loadUsers() {
    setLoading(true)
    try {
      setUsers(await listUsers())
      setNotice(null)
    } catch (error) {
      setUsers([])
      setNotice({ type: 'error', text: getErrorMessage(error) })
    } finally {
      setLoading(false)
    }
  }

  async function handleLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setLoading(true)
    try {
      const response = await login(loginForm)
      const nextSession = saveSession(response)
      setSession(nextSession)
      setLoginForm(initialLogin)
      setView('account')
      setNotice({ type: 'success', text: 'Login realizado com sucesso.' })
    } catch (error) {
      setNotice({ type: 'error', text: getErrorMessage(error) })
    } finally {
      setLoading(false)
    }
  }

  async function handleRegister(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setLoading(true)
    try {
      await register(registerForm)
      setRegisterForm(initialRegister)
      setView('login')
      setNotice({
        type: 'success',
        text: 'Cadastro criado. Faca login para continuar.',
      })
    } catch (error) {
      setNotice({ type: 'error', text: getErrorMessage(error) })
    } finally {
      setLoading(false)
    }
  }

  async function handleUpdateAccount(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setLoading(true)
    try {
      const user = await updateMe(accountForm)
      saveUser(user)
      setSession((current) => (current ? { ...current, user } : null))
      setNotice({ type: 'success', text: 'Conta atualizada com sucesso.' })
    } catch (error) {
      setNotice({ type: 'error', text: getErrorMessage(error) })
    } finally {
      setLoading(false)
    }
  }

  async function handleLogout() {
    setLoading(true)
    try {
      await logout()
      setSession(null)
      setView('login')
      setUsers([])
      setNotice({ type: 'info', text: 'Sessao encerrada.' })
    } finally {
      setLoading(false)
    }
  }

  async function handleDeleteAccount() {
    const confirmed = window.confirm('Remover sua conta permanentemente?')
    if (!confirmed) {
      return
    }

    setLoading(true)
    try {
      await deleteMe()
      setSession(null)
      setView('register')
      setNotice({ type: 'info', text: 'Conta removida.' })
    } catch (error) {
      setNotice({ type: 'error', text: getErrorMessage(error) })
    } finally {
      setLoading(false)
    }
  }

  function changeView(nextView: View) {
    setView(nextView)
    setNotice(null)
  }

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">AU</span>
          <div>
            <h1>ApiUsuarios</h1>
            <p>Auth Service</p>
          </div>
        </div>

        <nav className="nav-list" aria-label="Navegacao principal">
          {!currentUser && (
            <>
              <button
                type="button"
                className={view === 'login' ? 'active' : ''}
                onClick={() => changeView('login')}
              >
                Login
              </button>
              <button
                type="button"
                className={view === 'register' ? 'active' : ''}
                onClick={() => changeView('register')}
              >
                Cadastro
              </button>
            </>
          )}

          {currentUser && (
            <>
              <button
                type="button"
                className={view === 'account' ? 'active' : ''}
                onClick={() => changeView('account')}
              >
                Minha conta
              </button>
              {isAdmin && (
                <button
                  type="button"
                  className={view === 'admin' ? 'active' : ''}
                  onClick={() => changeView('admin')}
                >
                  Usuarios
                </button>
              )}
            </>
          )}
        </nav>

        <div className="sidebar-footer">
          <span>API</span>
          <code>{API_BASE_URL}</code>
        </div>
      </aside>

      <section className="workspace">
        <header className="topbar">
          <div>
            <p className="eyebrow">Identidade e acesso</p>
            <h2>{viewTitle(view, currentUser)}</h2>
            <p>{subtitle}</p>
          </div>
          {currentUser && (
            <button type="button" className="ghost-button" onClick={handleLogout}>
              Sair
            </button>
          )}
        </header>

        {notice && <div className={`notice ${notice.type}`}>{notice.text}</div>}

        {view === 'login' && (
          <AuthPanel title="Login" aside="Use as credenciais cadastradas na API.">
            <form className="form-grid" onSubmit={handleLogin}>
              <Field
                label="Email"
                name="email"
                type="email"
                value={loginForm.email}
                onChange={(email) => setLoginForm({ ...loginForm, email })}
              />
              <Field
                label="Senha"
                name="senha"
                type="password"
                value={loginForm.senha}
                onChange={(senha) => setLoginForm({ ...loginForm, senha })}
              />
              <button type="submit" disabled={loading}>
                {loading ? 'Entrando...' : 'Entrar'}
              </button>
            </form>
          </AuthPanel>
        )}

        {view === 'register' && (
          <AuthPanel title="Cadastro" aside="Novos usuarios entram com role User.">
            <form className="form-grid two-columns" onSubmit={handleRegister}>
              <Field
                label="Usuario"
                name="usuario"
                value={registerForm.usuario}
                onChange={(usuario) =>
                  setRegisterForm({ ...registerForm, usuario })
                }
              />
              <Field
                label="Email"
                name="email"
                type="email"
                value={registerForm.email}
                onChange={(email) => setRegisterForm({ ...registerForm, email })}
              />
              <Field
                label="Nome"
                name="nome"
                value={registerForm.nome}
                onChange={(nome) => setRegisterForm({ ...registerForm, nome })}
              />
              <Field
                label="Sobrenome"
                name="sobrenome"
                value={registerForm.sobrenome}
                onChange={(sobrenome) =>
                  setRegisterForm({ ...registerForm, sobrenome })
                }
              />
              <Field
                label="Senha"
                name="senha"
                type="password"
                value={registerForm.senha}
                onChange={(senha) => setRegisterForm({ ...registerForm, senha })}
              />
              <Field
                label="Confirmar senha"
                name="confirmaSenha"
                type="password"
                value={registerForm.confirmaSenha}
                onChange={(confirmaSenha) =>
                  setRegisterForm({ ...registerForm, confirmaSenha })
                }
              />
              <button type="submit" disabled={loading}>
                {loading ? 'Cadastrando...' : 'Cadastrar'}
              </button>
            </form>
          </AuthPanel>
        )}

        {view === 'account' && currentUser && (
          <section className="content-grid">
            <div className="panel">
              <div className="panel-header">
                <h3>Dados da conta</h3>
                <span className="role-badge">{currentUser.role}</span>
              </div>
              <form className="form-grid two-columns" onSubmit={handleUpdateAccount}>
                <Field
                  label="Usuario"
                  name="usuario"
                  value={accountForm.usuario}
                  onChange={(usuario) =>
                    setAccountForm({ ...accountForm, usuario })
                  }
                />
                <Field
                  label="Email"
                  name="email"
                  type="email"
                  value={accountForm.email}
                  onChange={(email) => setAccountForm({ ...accountForm, email })}
                />
                <Field
                  label="Nome"
                  name="nome"
                  value={accountForm.nome}
                  onChange={(nome) => setAccountForm({ ...accountForm, nome })}
                />
                <Field
                  label="Sobrenome"
                  name="sobrenome"
                  value={accountForm.sobrenome}
                  onChange={(sobrenome) =>
                    setAccountForm({ ...accountForm, sobrenome })
                  }
                />
                <button type="submit" disabled={loading}>
                  {loading ? 'Salvando...' : 'Salvar alteracoes'}
                </button>
              </form>
            </div>

            <div className="panel danger-panel">
              <h3>Sessao</h3>
              <dl className="facts">
                <div>
                  <dt>ID</dt>
                  <dd>{currentUser.id}</dd>
                </div>
                <div>
                  <dt>Criado em</dt>
                  <dd>{formatDate(currentUser.dataCriacao)}</dd>
                </div>
                <div>
                  <dt>Alterado em</dt>
                  <dd>{formatDate(currentUser.dataAlteracao)}</dd>
                </div>
              </dl>
              <div className="button-row">
                <button type="button" className="ghost-button" onClick={handleLogout}>
                  Encerrar sessao
                </button>
                <button
                  type="button"
                  className="danger-button"
                  onClick={handleDeleteAccount}
                >
                  Remover conta
                </button>
              </div>
            </div>
          </section>
        )}

        {view === 'admin' && currentUser && (
          <section className="panel">
            <div className="panel-header">
              <h3>Usuarios</h3>
              <button type="button" className="ghost-button" onClick={loadUsers}>
                Atualizar
              </button>
            </div>
            {!isAdmin ? (
              <p className="empty-state">Apenas usuarios Admin podem listar contas.</p>
            ) : (
              <div className="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Usuario</th>
                      <th>Email</th>
                      <th>Role</th>
                      <th>Criado em</th>
                    </tr>
                  </thead>
                  <tbody>
                    {users.map((user) => (
                      <tr key={user.id}>
                        <td>{user.id}</td>
                        <td>{user.usuario}</td>
                        <td>{user.email}</td>
                        <td>{user.role}</td>
                        <td>{formatDate(user.dataCriacao)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
                {users.length === 0 && (
                  <p className="empty-state">Nenhum usuario carregado.</p>
                )}
              </div>
            )}
          </section>
        )}
      </section>
    </main>
  )
}

interface AuthPanelProps {
  title: string
  aside: string
  children: ReactNode
}

function AuthPanel({ title, aside, children }: AuthPanelProps) {
  return (
    <section className="auth-layout">
      <div className="panel auth-panel">
        <h3>{title}</h3>
        {children}
      </div>
      <aside className="auth-aside">
        <h3>Fluxo atual</h3>
        <p>{aside}</p>
        <ul>
          <li>JWT Bearer Token</li>
          <li>Refresh token rotacionado</li>
          <li>Roles User e Admin</li>
        </ul>
      </aside>
    </section>
  )
}

interface FieldProps {
  label: string
  name: string
  value: string
  type?: string
  onChange: (value: string) => void
}

function Field({ label, name, value, type = 'text', onChange }: FieldProps) {
  return (
    <label className="field">
      <span>{label}</span>
      <input
        name={name}
        type={type}
        value={value}
        autoComplete={type === 'password' ? 'current-password' : 'on'}
        onChange={(event) => onChange(event.target.value)}
        required
      />
    </label>
  )
}

function viewTitle(view: View, user: Usuario | null) {
  if (!user && view === 'register') {
    return 'Criar conta'
  }

  if (!user) {
    return 'Acessar conta'
  }

  if (view === 'admin') {
    return 'Administracao'
  }

  return 'Minha conta'
}

function getInitialView(session: SessionState | null): View {
  return session ? 'account' : 'login'
}

function getErrorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    return error.response?.mensagem || error.message
  }

  if (error instanceof Error) {
    return error.message
  }

  return 'Nao foi possivel concluir a acao.'
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(new Date(value))
}

export default App

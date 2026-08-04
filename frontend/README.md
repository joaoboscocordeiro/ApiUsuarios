# ApiUsuarios Frontend

Frontend React para consumir o Auth Service `ApiUsuarios`.

## Stack

- React
- TypeScript
- Vite
- CSS

## Funcionalidades

- Login.
- Cadastro.
- Minha conta.
- Edicao do usuario autenticado.
- Logout.
- Remocao da propria conta.
- Listagem administrativa de usuarios para role `Admin`.
- Refresh token automatico quando a API retornar `401`.

## Configuracao

Copie `frontend/.env.example` para `frontend/.env` quando precisar mudar a URL da API:

```env
VITE_API_BASE_URL=http://localhost:5196
```

No futuro, quando existir BFF, essa variavel deve apontar para o BFF.

## Executar

```powershell
npm install
npm run dev
```

URL local:

```text
http://localhost:5173
```

## Validar

```powershell
npm run build
npm run lint
npm audit --audit-level=moderate
```

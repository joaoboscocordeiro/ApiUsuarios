using ApiUsuarios.Data;
using ApiUsuarios.Dtos.Login;
using ApiUsuarios.Dtos.Usuario;
using ApiUsuarios.Models;
using ApiUsuarios.Services.Senha;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuarios.Services.Usuario
{
    public class UsuarioService : IUsuarioInterface
    {
        private readonly AppDbContext _context;
        private readonly ISenhaInterface _senhaInterface;

        public UsuarioService(AppDbContext context, ISenhaInterface senhaInterface)
        {
            _context = context;
            _senhaInterface = senhaInterface;
        }

        public async Task<ResponseModel<UsuarioRespostaDto>> BuscarUsuarioPorId(int id)
        {
            ResponseModel<UsuarioRespostaDto> response = new();

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    response.Mensagem = "Usuario nao localizado!";
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                response.Dados = MapearUsuarioResposta(usuario);
                response.Mensagem = "Usuario localizado!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioRespostaDto>> EditarUsuario(UsuarioEdicaoDto usuarioEdicaoDto)
        {
            ResponseModel<UsuarioRespostaDto> response = new();

            try
            {
                var usuarioDB = await _context.Usuarios.FindAsync(usuarioEdicaoDto.Id);

                if (usuarioDB == null)
                {
                    response.Mensagem = "Usuario nao localizado!";
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                usuarioDB.Usuario = usuarioEdicaoDto.Usuario;
                usuarioDB.Nome = usuarioEdicaoDto.Nome;
                usuarioDB.Sobrenome = usuarioEdicaoDto.Sobrenome;
                usuarioDB.Email = usuarioEdicaoDto.Email;
                usuarioDB.DataAlteracao = DateTime.Now;

                _context.Update(usuarioDB);
                await _context.SaveChangesAsync();

                response.Mensagem = "Usuario editado com sucesso!";
                response.Dados = MapearUsuarioResposta(usuarioDB);
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        public async Task<ResponseModel<List<UsuarioRespostaDto>>> ListarUsuarios()
        {
            ResponseModel<List<UsuarioRespostaDto>> response = new();

            try
            {
                var usuarios = await _context.Usuarios
                    .AsNoTracking()
                    .Select(usuario => new UsuarioRespostaDto
                    {
                        Id = usuario.Id,
                        Usuario = usuario.Usuario,
                        Nome = usuario.Nome,
                        Sobrenome = usuario.Sobrenome,
                        Email = usuario.Email,
                        Role = usuario.Role,
                        DataCriacao = usuario.DataCriacao,
                        DataAlteracao = usuario.DataAlteracao
                    })
                    .ToListAsync();

                response.Dados = usuarios;
                response.Mensagem = "Usuarios localizados!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        public async Task<ResponseModel<LoginRespostaDto>> Login(UsuarioLoginDto usuarioLoginDto)
        {
            ResponseModel<LoginRespostaDto> response = new();

            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuarioLoginDto.Email);

                if (usuario == null)
                {
                    response.Mensagem = "Credenciais invalidas!";
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                if (!_senhaInterface.VerificaSenhaHash(usuarioLoginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    response.Mensagem = "Credenciais invalidas!";
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                response.Dados = await CriarLoginResposta(usuario);
                response.Mensagem = "Usuario logado com sucesso!";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioRespostaDto>> RegistrarUsuario(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            ResponseModel<UsuarioRespostaDto> response = new();

            try
            {
                if (await ExisteEmailOuUsuario(usuarioCriacaoDto))
                {
                    response.Mensagem = "Email/Usuario ja cadastrado!";
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status409Conflict;
                    return response;
                }

                _senhaInterface.CriarSenhaHash(usuarioCriacaoDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                UsuarioModel usuario = new()
                {
                    Usuario = usuarioCriacaoDto.Usuario,
                    Email = usuarioCriacaoDto.Email,
                    Nome = usuarioCriacaoDto.Nome,
                    Sobrenome = usuarioCriacaoDto.Sobrenome,
                    Role = UsuarioRoles.User,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                response.Mensagem = "Usuario cadastrado com sucesso!";
                response.Dados = MapearUsuarioResposta(usuario);
                response.StatusCode = StatusCodes.Status201Created;
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        public async Task<ResponseModel<LoginRespostaDto>> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            ResponseModel<LoginRespostaDto> response = new();

            try
            {
                var refreshTokenHash = _senhaInterface.CriarHashToken(refreshTokenDto.RefreshToken);

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                    u.RefreshTokenHash == refreshTokenHash);

                if (usuario == null ||
                    usuario.RefreshTokenRevogadoEm != null ||
                    usuario.RefreshTokenExpiracao == null ||
                    usuario.RefreshTokenExpiracao <= DateTime.UtcNow)
                {
                    response.Mensagem = "Refresh token invalido ou expirado!";
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                response.Dados = await CriarLoginResposta(usuario);
                response.Mensagem = "Token renovado com sucesso!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        public async Task<ResponseModel<string>> Logout(int usuarioId)
        {
            ResponseModel<string> response = new();

            try
            {
                var usuario = await _context.Usuarios.FindAsync(usuarioId);

                if (usuario == null)
                {
                    response.Mensagem = "Usuario nao localizado!";
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                usuario.RefreshTokenHash = null;
                usuario.RefreshTokenRevogadoEm = DateTime.UtcNow;

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                response.Dados = "Logout realizado com sucesso!";
                response.Mensagem = "Logout realizado com sucesso!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioRespostaDto>> RemoverUsuario(int id)
        {
            ResponseModel<UsuarioRespostaDto> response = new();

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    response.Mensagem = "Usuario nao localizado!";
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                _context.Remove(usuario);
                await _context.SaveChangesAsync();

                response.Dados = MapearUsuarioResposta(usuario);
                response.Mensagem = "Usuario removido com sucesso!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        private async Task<bool> ExisteEmailOuUsuario(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            return await _context.Usuarios.AnyAsync(item =>
                item.Email == usuarioCriacaoDto.Email || item.Usuario == usuarioCriacaoDto.Usuario);
        }

        private async Task<LoginRespostaDto> CriarLoginResposta(UsuarioModel usuario)
        {
            var tokenExpiracao = _senhaInterface.ObterDataExpiracaoTokenAcesso();
            var refreshToken = _senhaInterface.CriarRefreshToken();
            var refreshTokenExpiracao = _senhaInterface.ObterDataExpiracaoRefreshToken();

            usuario.RefreshTokenHash = _senhaInterface.CriarHashToken(refreshToken);
            usuario.RefreshTokenCriadoEm = DateTime.UtcNow;
            usuario.RefreshTokenExpiracao = refreshTokenExpiracao;
            usuario.RefreshTokenRevogadoEm = null;

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return new LoginRespostaDto
            {
                Usuario = MapearUsuarioResposta(usuario),
                Token = _senhaInterface.CriarToken(usuario, tokenExpiracao),
                TokenExpiracao = tokenExpiracao,
                RefreshToken = refreshToken,
                RefreshTokenExpiracao = refreshTokenExpiracao
            };
        }

        private static UsuarioRespostaDto MapearUsuarioResposta(UsuarioModel usuario)
        {
            return new UsuarioRespostaDto
            {
                Id = usuario.Id,
                Usuario = usuario.Usuario,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Email = usuario.Email,
                Role = usuario.Role,
                DataCriacao = usuario.DataCriacao,
                DataAlteracao = usuario.DataAlteracao
            };
        }
    }
}

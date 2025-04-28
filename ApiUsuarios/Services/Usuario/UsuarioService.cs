using ApiUsuarios.Data;
using ApiUsuarios.Dtos.Usuario;
using ApiUsuarios.Models;
using ApiUsuarios.Services.Senha;
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

        public async Task<ResponseModel<UsuarioModel>> BuscarUsuarioPorId(int id)
        {
            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    response.Mensagem = "Usuário não localizado!";
                    return response;
                }

                response.Dados = usuario;
                response.Mensagem = "Usuários localizado!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioModel>> EditarUsuario(UsuarioEdicaoDto usuarioEdicaoDto)
        {
            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
                var usuarioDB = await _context.Usuarios.FindAsync(usuarioEdicaoDto.Id);

                if (usuarioDB == null)
                {
                    response.Mensagem = "Usuário não localizado!";
                    return response;
                }

                usuarioDB.Usuario = usuarioEdicaoDto.Usuario;
                usuarioDB.Nome = usuarioEdicaoDto.Nome;
                usuarioDB.Sobrenome = usuarioEdicaoDto.Sobrenome;
                usuarioDB.Email = usuarioEdicaoDto.Email;
                usuarioDB.DataAlteracao = DateTime.Now;

                _context.Update(usuarioDB);
                await _context.SaveChangesAsync();

                response.Mensagem = "Usuário editado com sucesso!";
                response.Dados = usuarioDB;
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<List<UsuarioModel>>> ListarUsuarios()
        {
            ResponseModel<List<UsuarioModel>> response = new ResponseModel<List<UsuarioModel>>();

            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();

                response.Dados = usuarios;
                response.Mensagem = "Usuários localizados!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioModel>> RegistrarUsuario(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
                if (!VerificaSeExisteEmailUsuarioRepetidos(usuarioCriacaoDto))
                {
                    response.Mensagem = "Email/Usuário já cadastrado!";
                    response.Status = false;
                    return response;
                }

                _senhaInterface.CriarSenhaHash(usuarioCriacaoDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                UsuarioModel usuario = new UsuarioModel()
                {
                    Usuario = usuarioCriacaoDto.Usuario,
                    Email = usuarioCriacaoDto.Email,
                    Nome = usuarioCriacaoDto.Nome,
                    Sobrenome = usuarioCriacaoDto.Sobrenome,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                response.Mensagem = "Usuário cadastrado com sucesso!";
                response.Dados = usuario;
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioModel>> RemoverUsuario(int id)
        {
            ResponseModel<UsuarioModel> response = new ResponseModel<UsuarioModel>();

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    response.Mensagem = "Usuário não localizado!";
                    return response;
                }

                _context.Remove(usuario);
                await _context.SaveChangesAsync();

                response.Dados = usuario;
                response.Mensagem = "Usuários Removido com Sucesso!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        private bool VerificaSeExisteEmailUsuarioRepetidos(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(item => item.Email == usuarioCriacaoDto.Email || item.Usuario == usuarioCriacaoDto.Usuario);

            if (usuario != null)
            {
                return false;
            }

            return true;
        }
    }
}

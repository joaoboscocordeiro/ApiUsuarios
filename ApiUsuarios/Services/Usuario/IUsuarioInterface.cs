using ApiUsuarios.Dtos.Usuario;
using ApiUsuarios.Models;

namespace ApiUsuarios.Services.Usuario
{
    public interface IUsuarioInterface
    {
        Task<ResponseModel<UsuarioModel>> RegistrarUsuario(UsuarioCriacaoDto criacaoCriacaoDto);
        Task<ResponseModel<List<UsuarioModel>>> ListarUsuarios();
        Task<ResponseModel<UsuarioModel>> BuscarUsuarioPorId(int id);
        Task<ResponseModel<UsuarioModel>> EditarUsuario(UsuarioEdicaoDto usuarioEdicaoDto);
        Task<ResponseModel<UsuarioModel>> RemoverUsuario(int id);
    }
}

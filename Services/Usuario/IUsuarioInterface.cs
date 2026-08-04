using ApiUsuarios.Dtos.Login;
using ApiUsuarios.Dtos.Usuario;
using ApiUsuarios.Models;

namespace ApiUsuarios.Services.Usuario
{
    public interface IUsuarioInterface
    {
        Task<ResponseModel<UsuarioRespostaDto>> RegistrarUsuario(UsuarioCriacaoDto criacaoCriacaoDto);
        Task<ResponseModel<List<UsuarioRespostaDto>>> ListarUsuarios();
        Task<ResponseModel<UsuarioRespostaDto>> BuscarUsuarioPorId(int id);
        Task<ResponseModel<UsuarioRespostaDto>> EditarUsuario(UsuarioEdicaoDto usuarioEdicaoDto);
        Task<ResponseModel<UsuarioRespostaDto>> RemoverUsuario(int id);
        Task<ResponseModel<LoginRespostaDto>> Login(UsuarioLoginDto usuarioLoginDto);
        Task<ResponseModel<LoginRespostaDto>> RefreshToken(RefreshTokenDto refreshTokenDto);
        Task<ResponseModel<string>> Logout(int usuarioId);
    }
}

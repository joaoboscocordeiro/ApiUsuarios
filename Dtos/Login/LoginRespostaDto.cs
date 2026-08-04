using ApiUsuarios.Dtos.Usuario;

namespace ApiUsuarios.Dtos.Login
{
    public class LoginRespostaDto
    {
        public UsuarioRespostaDto Usuario { get; set; } = new();
        public string Token { get; set; } = string.Empty;
    }
}

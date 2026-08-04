using ApiUsuarios.Dtos.Usuario;

namespace ApiUsuarios.Dtos.Login
{
    public class LoginRespostaDto
    {
        public UsuarioRespostaDto Usuario { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiracao { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiracao { get; set; }
    }
}

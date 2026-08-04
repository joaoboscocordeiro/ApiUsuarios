using System.ComponentModel.DataAnnotations;

namespace ApiUsuarios.Dtos.Login
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Informe o refresh token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

using ApiUsuarios.Dtos.Login;
using ApiUsuarios.Dtos.Usuario;
using ApiUsuarios.Models;
using ApiUsuarios.Services.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiUsuarios.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarioInterface _usuarioInterface;

        public LoginController(IUsuarioInterface usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegistrarUsuario(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            var usuario = await _usuarioInterface.RegistrarUsuario(usuarioCriacaoDto);
            return Responder(usuario);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var usuario = await _usuarioInterface.Login(usuarioLoginDto);
            return Responder(usuario);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var usuario = await _usuarioInterface.RefreshToken(refreshTokenDto);
            return Responder(usuario);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var usuarioId = ObterUsuarioIdAutenticado();
            if (usuarioId is null)
            {
                return ResponderTokenInvalido();
            }

            var usuario = await _usuarioInterface.Logout(usuarioId.Value);
            return Responder(usuario);
        }

        private IActionResult Responder<T>(ResponseModel<T> response)
        {
            return StatusCode(response.StatusCode, response);
        }

        private int? ObterUsuarioIdAutenticado()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(usuarioId, out var id) ? id : null;
        }

        private IActionResult ResponderTokenInvalido()
        {
            return StatusCode(StatusCodes.Status401Unauthorized, new ResponseModel<string>
            {
                Mensagem = "Token invalido!",
                Status = false,
                StatusCode = StatusCodes.Status401Unauthorized
            });
        }
    }
}

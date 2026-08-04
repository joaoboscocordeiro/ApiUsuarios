using ApiUsuarios.Dtos.Usuario;
using ApiUsuarios.Models;
using ApiUsuarios.Services.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiUsuarios.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioInterface _usuarioInterface;

        public UsuarioController(IUsuarioInterface usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }

        [HttpGet]
        [Authorize(Roles = UsuarioRoles.Admin)]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioInterface.ListarUsuarios();
            return Responder(usuarios);
        }

        [HttpGet("me")]
        public async Task<IActionResult> BuscarMeuUsuario()
        {
            var usuarioId = ObterUsuarioIdAutenticado();
            if (usuarioId is null)
            {
                return ResponderTokenInvalido();
            }

            var usuario = await _usuarioInterface.BuscarUsuarioPorId(usuarioId.Value);
            return Responder(usuario);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarUsuarioPorId(int id)
        {
            var acessoNegado = ValidarAcessoAoUsuario(id);
            if (acessoNegado is not null)
            {
                return acessoNegado;
            }

            var usuario = await _usuarioInterface.BuscarUsuarioPorId(id);
            return Responder(usuario);
        }

        [HttpPut("me")]
        public async Task<IActionResult> EditarMeuUsuario(UsuarioEdicaoDto usuarioEdicaoDto)
        {
            var usuarioId = ObterUsuarioIdAutenticado();
            if (usuarioId is null)
            {
                return ResponderTokenInvalido();
            }

            usuarioEdicaoDto.Id = usuarioId.Value;

            var usuario = await _usuarioInterface.EditarUsuario(usuarioEdicaoDto);
            return Responder(usuario);
        }

        [HttpPut]
        public async Task<IActionResult> EditarUsuario(UsuarioEdicaoDto usuarioEdicaoDto)
        {
            var acessoNegado = ValidarAcessoAoUsuario(usuarioEdicaoDto.Id);
            if (acessoNegado is not null)
            {
                return acessoNegado;
            }

            var usuario = await _usuarioInterface.EditarUsuario(usuarioEdicaoDto);
            return Responder(usuario);
        }

        [HttpDelete("me")]
        public async Task<IActionResult> RemoverMeuUsuario()
        {
            var usuarioId = ObterUsuarioIdAutenticado();
            if (usuarioId is null)
            {
                return ResponderTokenInvalido();
            }

            var usuario = await _usuarioInterface.RemoverUsuario(usuarioId.Value);
            return Responder(usuario);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> RemoverUsuario(int id)
        {
            var acessoNegado = ValidarAcessoAoUsuario(id);
            if (acessoNegado is not null)
            {
                return acessoNegado;
            }

            var usuario = await _usuarioInterface.RemoverUsuario(id);
            return Responder(usuario);
        }

        private IActionResult Responder<T>(ResponseModel<T> response)
        {
            return StatusCode(response.StatusCode, response);
        }

        private IActionResult? ValidarAcessoAoUsuario(int id)
        {
            var usuarioId = ObterUsuarioIdAutenticado();
            if (usuarioId is null)
            {
                return ResponderTokenInvalido();
            }

            if (usuarioId.Value != id)
            {
                return ResponderAcessoNegado();
            }

            return null;
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

        private IActionResult ResponderAcessoNegado()
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel<string>
            {
                Mensagem = "Voce nao tem permissao para acessar este usuario!",
                Status = false,
                StatusCode = StatusCodes.Status403Forbidden
            });
        }
    }
}

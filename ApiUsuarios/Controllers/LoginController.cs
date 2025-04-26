using ApiUsuarios.Dtos.Usuario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiUsuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult RegistrarUsuario(UsuarioCriacaoDto criacaoDto)
        {
            return Ok();
        }
    }
}

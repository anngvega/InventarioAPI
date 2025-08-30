using InventarioAPI.Data.Contrato;
using InventarioAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuario usuarioDB;

        public UsuarioController(IUsuario usuarioRepo)
        {
            usuarioDB = usuarioRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await usuarioDB.LoginAsync(request.NombreUsuario, request.Clave);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Nombre de usuario o clave incorrectos." });

            var resp = new
            {
                usuario.Id,
                usuario.Nombres,
                usuario.Apellidos,
                usuario.NombreUsuario,
                Rol = usuario.Rol?.Nombre ?? ""  
            };

            return Ok(resp);
        }
    }
}

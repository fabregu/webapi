using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Custom;
using WebApi.Models;
using WebApi.Models.DTOs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly DbpruebaContext _dbPruebaContext;
        private readonly Utilidades _utilidades;

        public AccesoController(DbpruebaContext dbPruebaContext, Utilidades utilidades)
        {
            _dbPruebaContext = dbPruebaContext;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO usuarioDTO)
        {
            try
            {
                var usuario = new Usuario
                {
                    Nombre = usuarioDTO.Nombre,
                    Correo = usuarioDTO.Correo,
                    Clave = _utilidades.encriptarSHA256(usuarioDTO.Clave)
                };
                await _dbPruebaContext.Usuarios.AddAsync(usuario);
                await _dbPruebaContext.SaveChangesAsync();

                if (usuario.IdUsuario == 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var usuarioEncontrado = await _dbPruebaContext.Usuarios
                .Where(x => 
                x.Correo == loginDTO.Correo && 
                x.Clave == _utilidades.encriptarSHA256(loginDTO.Clave))
                .FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token="" });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilidades.generarToken(usuarioEncontrado) });
        }
    }
}

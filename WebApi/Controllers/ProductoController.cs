using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly DbpruebaContext _dbPruebaContext;

        public ProductoController(DbpruebaContext dbPruebaContext)
        {
            _dbPruebaContext = dbPruebaContext;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            try
            {
                var productos = await _dbPruebaContext.Productos.ToListAsync();
                return StatusCode(StatusCodes.Status200OK, new {value = productos});
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}

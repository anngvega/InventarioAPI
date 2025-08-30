using InventarioAPI.Data.Contrato;
using InventarioAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProducto productoDB;
        public ProductoController(IProducto productoRepo)
        {
            productoDB = productoRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await Task.Run(() => productoDB.Listado()));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            return Ok(await Task.Run(() => productoDB.ObtenerPorID(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Producto producto)
        {
            return Ok(await Task.Run(() => productoDB.Registrar(producto)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Producto producto)
        {
            producto.Id = id;
            var actualizado = await Task.Run(() => productoDB.Actualizar(producto));
            if (actualizado == null) return NotFound();
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var ok = productoDB.Eliminar(id);
                if (!ok) return NotFound(new { mensaje = "Producto no encontrado." });
                return NoContent(); 
            }
            catch (SqlException ex) 
            {
                if (ex.Message.Contains("tiene movimientos de stock"))
                    return Conflict(new { mensaje = ex.Message });

                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }   

} 

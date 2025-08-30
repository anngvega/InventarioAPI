using InventarioAPI.Data.Contrato;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoController : ControllerBase
    {
        private readonly IMovimientos _svc;            

        public MovimientoController(IMovimientos svc)     
        {
            _svc = svc;
        }

        public class VentaRapidaRequest
        {
            public int Cantidad { get; set; }
            public int UsuarioId { get; set; }
            public string? Motivo { get; set; } = "Venta";  
        }

        
        [HttpPost("{productoId}/venta")]
        public IActionResult VentaRapida(int productoId, [FromBody] VentaRapidaRequest req)
        {
            if (req == null) return BadRequest(new { mensaje = "Body requerido." });
            if (req.Cantidad <= 0) return BadRequest(new { mensaje = "La cantidad debe ser mayor que cero." });

            try
            {
                _svc.RegistrarSalidaVenta(productoId, req.Cantidad, req.UsuarioId, req.Motivo ?? "Venta");
                return NoContent(); 
            }
            catch (SqlException ex) when (ex.Message.Contains("Stock insuficiente"))
            {
                return Conflict(new { mensaje = ex.Message }); 
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message }); 
            }
        }

        [HttpPost("{productoId}/entrada")]
        public IActionResult EntradaRapida(int productoId, [FromBody] VentaRapidaRequest req)
        {
            if (req == null) return BadRequest(new { mensaje = "Body requerido." });
            if (req.Cantidad <= 0) return BadRequest(new { mensaje = "La cantidad debe ser mayor que cero." });

            try
            {
                _svc.RegistrarEntrada(productoId, req.Cantidad, req.UsuarioId, req.Motivo ?? "Compra");
                return NoContent();
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}

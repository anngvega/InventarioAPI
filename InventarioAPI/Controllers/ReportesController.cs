using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly string _cs;
        public ReportesController(IConfiguration cfg)
        {
            _cs = cfg.GetConnectionString("DB")
                ?? throw new InvalidOperationException("Falta ConnectionStrings:DB");
        }

      
        [HttpGet("ventas")]
        public IActionResult Ventas([FromQuery] DateTime desde, [FromQuery] DateTime hasta, [FromQuery] string? texto)
        {
            if (desde == default || hasta == default)
                return BadRequest(new { mensaje = "Parámetros desde/hasta requeridos (yyyy-MM-dd)." });

            var filas = new List<object>();

            using var cn = new SqlConnection(_cs);
            cn.Open();
            using var cmd = new SqlCommand("ReporteVentasDetalle", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Desde", desde.Date);
            cmd.Parameters.AddWithValue("@Hasta", hasta.Date);
            cmd.Parameters.AddWithValue("@TextoBuscar", (object?)texto ?? DBNull.Value);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                filas.Add(new
                {
                    Fecha = rd.GetDateTime(rd.GetOrdinal("Fecha")),
                    Codigo = rd.GetString(rd.GetOrdinal("Codigo")),
                    Producto = rd.GetString(rd.GetOrdinal("Producto")),
                    Cantidad = rd.GetInt32(rd.GetOrdinal("Cantidad")),
                    PrecioVenta = rd.GetDecimal(rd.GetOrdinal("PrecioVenta")),
                    PrecioCosto = rd.GetDecimal(rd.GetOrdinal("PrecioCosto")),
                    Importe = rd.GetDecimal(rd.GetOrdinal("Importe")),
                    Costo = rd.GetDecimal(rd.GetOrdinal("Costo")),
                    Ganancia = rd.GetDecimal(rd.GetOrdinal("Ganancia")),
                    Usuario = rd.GetString(rd.GetOrdinal("Usuario"))
                });
            }

            return Ok(filas);
        }

       
        [HttpGet("ventas/resumen")]
        public IActionResult VentasResumen([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        {
            if (desde == default || hasta == default)
                return BadRequest(new { mensaje = "Parámetros desde/hasta requeridos (yyyy-MM-dd)." });

            var filas = new List<object>();

            using var cn = new SqlConnection(_cs);
            cn.Open();
            using var cmd = new SqlCommand("ReporteVentasResumen", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Desde", desde.Date);
            cmd.Parameters.AddWithValue("@Hasta", hasta.Date);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                filas.Add(new
                {
                    Dia = rd.GetDateTime(rd.GetOrdinal("Dia")),
                    CantidadTotal = rd.GetInt32(rd.GetOrdinal("CantidadTotal")),
                    ImporteTotal = rd.GetDecimal(rd.GetOrdinal("ImporteTotal")),
                    CostoTotal = rd.GetDecimal(rd.GetOrdinal("CostoTotal")),
                    GananciaTotal = rd.GetDecimal(rd.GetOrdinal("GananciaTotal"))
                });
            }

            return Ok(filas);
        }
    }
}

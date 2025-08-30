using InventarioWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InventarioWebApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IConfiguration _config;
        public ReportesController(IConfiguration config) { _config = config; }

        public IActionResult Ventas(DateTime? desde, DateTime? hasta, string? texto)
        {
            var fDesde = (desde ?? DateTime.Today.AddDays(-6)).Date;
            var fHasta = (hasta ?? DateTime.Today).Date;

            var detalle = new List<VentaDetalleVM>();
            var resumen = new List<VentaResumenDiaVM>();

            try
            {
                var baseUrl = _config["Services:URL"]!.TrimEnd('/') + "/";

                using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };

                // Detalle
                var urlDetalle = $"Reportes/ventas?desde={fDesde:yyyy-MM-dd}&hasta={fHasta:yyyy-MM-dd}&texto={Uri.EscapeDataString(texto ?? "")}";
                var r1 = http.GetAsync(urlDetalle).Result;
                if (r1.IsSuccessStatusCode)
                {
                    var json = r1.Content.ReadAsStringAsync().Result;
                    detalle = JsonConvert.DeserializeObject<List<VentaDetalleVM>>(json) ?? new();
                }

                // Resumen
                var urlResumen = $"Reportes/ventas/resumen?desde={fDesde:yyyy-MM-dd}&hasta={fHasta:yyyy-MM-dd}";
                var r2 = http.GetAsync(urlResumen).Result;
                if (r2.IsSuccessStatusCode)
                {
                    var json = r2.Content.ReadAsStringAsync().Result;
                    resumen = JsonConvert.DeserializeObject<List<VentaResumenDiaVM>>(json) ?? new();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Desde = fDesde.ToString("yyyy-MM-dd");
            ViewBag.Hasta = fHasta.ToString("yyyy-MM-dd");
            ViewBag.Texto = texto ?? "";
            ViewBag.Resumen = resumen;

            return View(detalle);
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using InventarioWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InventarioWebApp.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IConfiguration _config;
        public ProductosController(IConfiguration config) => _config = config;

      
        private string GetBaseUrl()
        {
            var baseUrl = _config["Services:URL"] ?? "";
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            return baseUrl;
        }

        private List<Producto> obtenerProductos()
        {
            var listado = new List<Producto>();
            using (var http = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) })
            {
                var resp = http.GetAsync("Producto").Result;
                var data = resp.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Producto>>(data) ?? new List<Producto>();
            }
            return listado;
        }

        private Producto ObtenerPorId(int id)
        {
            using var http = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) };
            var resp = http.GetAsync($"Producto/{id}").Result;
            if (!resp.IsSuccessStatusCode) return null;
            var data = resp.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Producto>(data);
        }

        private Producto registrarProducto(Producto producto)
        {
            using var http = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) };
            var contenido = new StringContent(JsonConvert.SerializeObject(producto), Encoding.UTF8, "application/json");
            var resp = http.PostAsync("Producto", contenido).Result;
            var data = resp.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Producto>(data);
        }

        private Producto actualizarProducto(Producto producto)
        {
            using var http = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) };

            var payload = new
            {
                nombre = producto.Nombre,
                stock = producto.Stock,
                precioCosto = producto.PrecioCosto,
                precioVenta = producto.PrecioVenta,
                fechaVencimiento = producto.FechaVencimiento
            };

            var contenido = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var resp = http.PutAsync($"Producto/{producto.Id}", contenido).Result;
            if (!resp.IsSuccessStatusCode) throw new Exception("No se pudo actualizar el producto.");

            var data = resp.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Producto>(data);
        }

        private bool eliminarProducto(int id)
        {
            using var http = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) };
            var resp = http.DeleteAsync($"Producto/{id}").Result;
            return resp.IsSuccessStatusCode;
        }

        public class VentaRapidaVM
        {
            [Required]
            public int ProductoId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
            public int Cantidad { get; set; } = 1;

            public string? Motivo { get; set; } = "Venta";
        }

        public IActionResult Index(int page = 1, int cantidad = 10, string textoBuscar = "", string estado = "")
        {
            var listado = obtenerProductos();

            var estados = listado
                .Select(p => p.Estado ?? "")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            if (!string.IsNullOrWhiteSpace(textoBuscar))
            {
                var q = textoBuscar.Trim();
                listado = listado
                    .Where(p =>
                        (!string.IsNullOrEmpty(p.Nombre) && p.Nombre.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(p.Codigo) && p.Codigo.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                listado = listado
                    .Where(p => string.Equals(p.Estado, estado, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalRegistros = listado.Count;
            if (cantidad <= 0) cantidad = 10;

            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)cantidad);
            if (totalPaginas == 0) totalPaginas = 1;
            page = Math.Max(1, Math.Min(page, totalPaginas));

            var omitir = (page - 1) * cantidad;
            var pagina = listado.Skip(omitir).Take(cantidad).ToList();

            
            ViewBag.totalPaginas = totalPaginas;
            ViewBag.page = page;
            ViewBag.cantidad = cantidad;
            ViewBag.textoBuscar = textoBuscar;
            ViewBag.estado = estado;
            ViewBag.Estados = estados; 

            return View(pagina);
        }

        public IActionResult Create() => View(new Producto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            try
            {
                registrarProducto(producto);
                TempData["msg"] = "Producto creado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(producto);
            }
        }

        
        public IActionResult Edit(int id)
        {
            var producto = ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Producto model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                var actualizado = actualizarProducto(model);
                TempData["msg"] = "Producto actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var ok = eliminarProducto(id);
                TempData[ok ? "msg" : "error"] = ok
                    ? "Producto eliminado correctamente."
                    : "No se pudo eliminar (no encontrado o error).";
            }
            catch (Exception ex)
            {
                TempData["error"] = "No se pudo eliminar: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

 
        public IActionResult Entrar(int id)
        {
            var p = ObtenerPorId(id);
            if (p == null) return NotFound();
            ViewBag.Producto = p;
            return View(new VentaRapidaVM { ProductoId = id, Cantidad = 1, Motivo = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Entrar(VentaRapidaVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Producto = ObtenerPorId(vm.ProductoId);
                return View(vm);
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                using var http = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) };
                var body = new { cantidad = vm.Cantidad, usuarioId = userId, motivo = vm.Motivo ?? "" };
                var resp = http.PostAsync($"Movimiento/{vm.ProductoId}/entrada",
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")).Result;

                if (resp.IsSuccessStatusCode) TempData["msg"] = "Entrada registrada.";
                else TempData["error"] = resp.Content.ReadAsStringAsync().Result;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Vender(int id)
        {
            var p = ObtenerPorId(id);
            if (p == null) return NotFound();
            ViewBag.Producto = p;
            return View(new VentaRapidaVM { ProductoId = id, Cantidad = 1, Motivo = "Venta" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Vender(VentaRapidaVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Producto = ObtenerPorId(vm.ProductoId);
                return View(vm);
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                using var http = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) };
                var body = new { cantidad = vm.Cantidad, usuarioId = userId, motivo = vm.Motivo ?? "Venta" };

                var resp = http.PostAsync($"Movimiento/{vm.ProductoId}/venta",
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")).Result;

                if (resp.IsSuccessStatusCode)
                {
                    TempData["msg"] = "Venta registrada.";
                }
                else
                {
                    var error = resp.Content.ReadAsStringAsync().Result;
                    TempData["error"] = string.IsNullOrWhiteSpace(error) ? "No se pudo registrar la venta." : error;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}

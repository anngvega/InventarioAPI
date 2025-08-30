using System.Net.Http;
using System.Text;
using InventarioWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InventarioWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration config) { _config = config; }

        #region . METODOS PRIVADOS .
        private Usuario obtenerUsuario(LoginRequest model)
        {
            var baseUrl = _config["Services:URL"];
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            using (var http = new HttpClient { BaseAddress = new Uri(baseUrl) })
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = http.PostAsync("Usuario/login", content).Result;

                if (!resp.IsSuccessStatusCode) return null;

                var data = resp.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<Usuario>(data);
                return user;
            }
        }
        #endregion

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginRequest model)
        {
            var user = obtenerUsuario(model);
            if (user == null)
            {
                ViewBag.Error = "Usuario o clave incorrectos.";
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.NombreUsuario ?? "");
            HttpContext.Session.SetString("Rol", user.Rol ?? "");

            return RedirectToAction("Index", "Productos");
        }

        // GET /Account/Logout (por si escribes la URL o entras con link)
        [HttpGet, ActionName("Logout")]
        public IActionResult LogoutGet()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // POST /Account/Logout (recomendado desde el botón del layout)
        [HttpPost, ValidateAntiForgeryToken, ActionName("Logout")]
        public IActionResult LogoutPost()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}

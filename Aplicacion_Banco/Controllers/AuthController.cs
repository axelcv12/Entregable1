using Aplicacion_Banco.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aplicacion_Banco.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<PrestatarioController> _logger;
        private readonly APLICACION_BANCOContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(ILogger<PrestatarioController> logger, APLICACION_BANCOContext db, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Login()
        {   
            return View();
        }
        //git prueba pull

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ValidarUsuario(Usuario model)
        {
            Usuario usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == model.Correo && u.Contrasena == model.Contrasena);
            if(usuario == null)
            {
                return View("Login", model);
            }
            _httpContextAccessor.HttpContext.Session.SetInt32("UserId", usuario.Id);
                        
            if (usuario.IdRol == 1)
            {
                return RedirectToAction("Index", "Inversionista");
            }

            if (usuario.IdRol == 2)
            {
                return RedirectToAction("Index", "JefePrestamista");
            }

            if (usuario.IdRol == 3)
            {
                return RedirectToAction("Index", "Prestamista");
            }

            if (usuario.IdRol == 4)
            {
                return RedirectToAction("Index", "Prestatario");
            }

            return NotFound();

        }

        [HttpPost]
        public IActionResult Register(Usuario model)
        {
            model.IdRol = 4;
            _db.Usuarios.Add(model);
            _db.SaveChanges();

            _httpContextAccessor.HttpContext.Session.SetInt32("UserId", model.Id);
            return View("Registrar", model);
        }

    }
}

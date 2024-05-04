using Aplicacion_Banco.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aplicacion_Banco.Controllers
{
    public class JefePrestamistaController : Controller
    {
        private readonly APLICACION_BANCOContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JefePrestamistaController(APLICACION_BANCOContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CrearPrestamista(Usuario model)
        {
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            model.IdRol = 3;
            model.IdUsuarioCreador = userId;
            _db.Usuarios.Add(model);
            _db.SaveChanges();

            return View("Index", model);
        }
    }
}

using Aplicacion_Banco.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aplicacion_Banco.Controllers
{
    public class InversionistaController : Controller
    {
        private readonly APLICACION_BANCOContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public InversionistaController(APLICACION_BANCOContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CrearJefePrestamista(Usuario model)
        {
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            model.IdRol = 2;
            model.IdUsuarioCreador = userId;
            _db.Usuarios.Add(model);
            _db.SaveChanges();

            return View("Index", model);
        }
    }
}

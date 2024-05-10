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

        public IActionResult Listado()
        {

            int? jefeprestamistaId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");


            var jefeprestamistas = _db.Usuarios.Where(u => u.IdUsuarioCreador == jefeprestamistaId).ToList();


            return View(jefeprestamistas);
        }

        public IActionResult ActualizarJefePrestamista(int id)
        {
            var jefeprestamista = _db.Usuarios.Find(id);
            return View(jefeprestamista);
        }

        [HttpPost]
        public IActionResult ActualizarJefePrestamista(Usuario model)
        {
            if (ModelState.IsValid)
            {

                var jefeprestamistaExistente = _db.Usuarios.Find(model.Id);


                if (jefeprestamistaExistente != null)
                {

                    model.IdRol = jefeprestamistaExistente.IdRol;
                    model.IdUsuarioCreador = jefeprestamistaExistente.IdUsuarioCreador;

                    jefeprestamistaExistente.Nombre = model.Nombre;
                    jefeprestamistaExistente.Edad = model.Edad;
                    jefeprestamistaExistente.Direccion = model.Direccion;
                    jefeprestamistaExistente.Correo = model.Correo;
                    jefeprestamistaExistente.Contrasena = model.Contrasena;


                    _db.SaveChanges();


                    ViewBag.Exito = "El Jefe de Prestamista se ha actualizado correctamente.";

                    return RedirectToAction("Listado");
                }
                else
                {

                    return NotFound();
                }
            }


            return View(model);
        }
        public IActionResult EliminarJefePrestamista(int id)
        {
            var jefeprestamista = _db.Usuarios.Find(id);


            var usuariosAsociados = _db.Usuarios.Any(u => u.IdUsuarioCreador == id);


            if (usuariosAsociados)
            {
                TempData["ErrorMessage"] = "No puedes eliminar este jefe de prestamista porque tiene otros usuarios asociados.";
                return RedirectToAction("Listado");
            }


            _db.Usuarios.Remove(jefeprestamista);
            _db.SaveChanges();

            return RedirectToAction("Listado");
        }
    }
}

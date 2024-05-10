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

        public IActionResult Listado()
        {
            int? jefeprestamistaId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            // Filtra los prestatarios asociados con el prestamista actual
            var prestamistas = _db.Usuarios.Where(u => u.IdUsuarioCreador == jefeprestamistaId).ToList();


            return View(prestamistas);
        }

        public IActionResult ActualizarPrestamista(int id)
        {
            var prestamista = _db.Usuarios.Find(id); //  ID
            return View(prestamista);
        }

        [HttpPost]
        public IActionResult ActualizarPrestamista(Usuario model)
        {
            if (ModelState.IsValid)
            {

                var prestamistaExistente = _db.Usuarios.Find(model.Id);

                // Verificar si se ha encontrado el prestatario
                if (prestamistaExistente != null)
                {
                    // Mantener los valores de idRol e idUsuarioCreador del prestatario existente
                    model.IdRol = prestamistaExistente.IdRol;
                    model.IdUsuarioCreador = prestamistaExistente.IdUsuarioCreador;

                    prestamistaExistente.Nombre = model.Nombre;
                    prestamistaExistente.Edad = model.Edad;
                    prestamistaExistente.Direccion = model.Direccion;
                    prestamistaExistente.Correo = model.Correo;
                    prestamistaExistente.Contrasena = model.Contrasena;

                    _db.SaveChanges();

                    ViewBag.Exito = "El prestamista se ha actualizado correctamente.";


                    return RedirectToAction("Listado");
                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }



        public IActionResult EliminarPrestamista(int id)
        {
            var prestamista = _db.Usuarios.Find(id); // ID


            var usuariosAsociados = _db.Usuarios.Any(u => u.IdUsuarioCreador == id);


            if (usuariosAsociados)
            {
                TempData["ErrorMessage"] = "No puedes eliminar este prestamista porque tiene otros usuarios asociados.";
                return RedirectToAction("Listado");
            }


            _db.Usuarios.Remove(prestamista);
            _db.SaveChanges();

            return RedirectToAction("Listado");
        }
    }
}

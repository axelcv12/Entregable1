using Aplicacion_Banco.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion_Banco.Controllers
{
    public class PrestamistaController : Controller
    {
        private readonly APLICACION_BANCOContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PrestamistaController(APLICACION_BANCOContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CrearPrestatario(Usuario model)
        {
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            model.IdRol = 4;
            model.IdUsuarioCreador = userId;
            _db.Usuarios.Add(model);
            _db.SaveChanges();

            return View("Index", model);
        }

        public IActionResult Listado()
        {

            int? prestamistaId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");


            var prestatarios = _db.Usuarios.Where(u => u.IdUsuarioCreador == prestamistaId).ToList();


            return View(prestatarios);
        }

        public IActionResult ActualizarPrestatario(int id)
        {
            var prestatario = _db.Usuarios.Find(id); //  ID
            return View(prestatario);
        }

        [HttpPost]
        public IActionResult ActualizarPrestatario(Usuario model)
        {
            if (ModelState.IsValid)
            {
                var prestatarioExistente = _db.Usuarios.Find(model.Id);

                if (prestatarioExistente != null)
                {
                    model.IdRol = prestatarioExistente.IdRol;
                    model.IdUsuarioCreador = prestatarioExistente.IdUsuarioCreador;

                    prestatarioExistente.Nombre = model.Nombre;
                    prestatarioExistente.Edad = model.Edad;
                    prestatarioExistente.Direccion = model.Direccion;
                    prestatarioExistente.Correo = model.Correo;
                    prestatarioExistente.Contrasena = model.Contrasena;

                    _db.SaveChanges();

                    ViewBag.Exito = "El prestatario se ha actualizado correctamente.";

                    return RedirectToAction("Listado");
                }
                else
                {
                    return NotFound();
                }
            }

            // Si el modelo no es válido, vuelve a mostrar la vista de actualización con el modelo actual
            return View(model);
        }
        public IActionResult EliminarPrestatario(int id)
        {
            var prestatario = _db.Usuarios.Find(id); // Encuentra el prestatario por su ID
            var prestamosAsociados = _db.Prestamos.Any(p => p.IdPrestatario == id);

            // Si existen préstamos asociados, mostrar un mensaje de error y redirigir
            if (prestamosAsociados)
            {
                TempData["ErrorMessage"] = "No puedes eliminar este prestatario porque tiene préstamos asociados.";
                return RedirectToAction("Listado");
            }

            // Si no hay préstamos asociados, procedemos a eliminar el prestatario
            _db.Usuarios.Remove(prestatario); // Elimina el prestatario de la base de datos

            // Guardar los cambios en la base de datos
            _db.SaveChanges();

            return RedirectToAction("Listado"); // Redirige al listado de prestatarios
        }

        public IActionResult SolicitudesRecibidas(string searchString, DateTime? startDate, DateTime? endDate)
        {
            int? prestamistaId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            var query = _db.Prestamos
                .Include(p => p.IdPrestatarioNavigation)
                .Include(p => p.IdMontoNavigation)
                .Include(p => p.IdDuracionNavigation)
                .Where(p => p.IdPrestamista == prestamistaId && (p.Estado == "Pendiente" || p.Estado == "Rechazado" || p.Estado == "Aprobado"));

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.IdPrestatarioNavigation.Nombre.Contains(searchString));
            }

            if (startDate != null)
            {
                query = query.Where(p => p.FechaInicio >= startDate);
            }

            if (endDate != null)
            {
                query = query.Where(p => p.FechaInicio <= endDate);
            }

            var solicitudesRecibidas = query.ToList();

            foreach (var solicitud in solicitudesRecibidas)
            {
                if (solicitud.FechaInicio != null && solicitud.IdDuracionNavigation != null && solicitud.IdDuracionNavigation.Duracion1 != null)
                {
                    solicitud.FechaFin = solicitud.FechaInicio.Value.AddDays((int)solicitud.IdDuracionNavigation.Duracion1);
                }
                else
                {
                    // Manejar el caso en que no se pueda calcular la fecha de finalización
                    // Puedes lanzar una excepción, registrar un error o manejarlo de otra manera según sea necesario
                }
            }

            return View(solicitudesRecibidas);
        }

        public IActionResult AprobarSolicitud(int id)
        {
            var solicitud = _db.Prestamos.Find(id);

            if (solicitud != null)
            {
                solicitud.Estado = "Aprobado";
                _db.SaveChanges();
                ViewBag.Exito = "La solicitud ha sido aprobada correctamente.";
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("SolicitudesRecibidas");
        }

        public IActionResult DesaprobarSolicitud(int id)
        {
            // Busca la solicitud de préstamo por su ID
            var solicitud = _db.Prestamos.Find(id);

            // Verifica si se encontró la solicitud
            if (solicitud != null)
            {
                // Cambia el estado de la solicitud a "Desaprobado"
                solicitud.Estado = "Rechazado";

                // Guarda los cambios en la base de datos
                _db.SaveChanges();

                // Establece un mensaje de éxito
                ViewBag.Exito = "La solicitud ha sido rechazada.";
            }
            else
            {
                // Si no se encuentra la solicitud, devuelve un error 404
                return NotFound();
            }

            // Redirige de vuelta a la página de solicitudes recibidas
            return RedirectToAction("SolicitudesRecibidas");
        }

    }
}

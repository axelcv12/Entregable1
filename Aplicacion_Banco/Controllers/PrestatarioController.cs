using Aplicacion_Banco.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Aplicacion_Banco.Controllers
{
    public class PrestatarioController : Controller
    {
        private readonly ILogger<PrestatarioController> _logger;
        private readonly APLICACION_BANCOContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PrestatarioController(ILogger<PrestatarioController> logger, APLICACION_BANCOContext db, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            VistaPrestamo model = new VistaPrestamo();
            model.Prestamos = _db.Prestamos
                .Where(p => p.IdPrestatario == userId)
                .Include(p => p.IdMontoNavigation)
                .Include(p => p.IdPrestamistaNavigation)
                .Include(p => p.IdDuracionNavigation)
                .Include(p => p.IdMetodoPagoNavigation)
                .ToList();

            model.Montos = _db.Montos.ToList();
            model.Duraciones = _db.Duracions.ToList();
            model.Metodopagos = _db.Metodopagos.ToList();

            model.Prestamistas = _db.Usuarios.Where(u => u.IdRol == 3).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult CrearPrestamo(VistaPrestamo vistaPrestamo)
        {
            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            Prestamo prestamo = vistaPrestamo.Prestamo;
            prestamo.Estado = "Pendiente";
            prestamo.FechaFin = SumarDiasLaborales(prestamo.FechaInicio.GetValueOrDefault(), 30);
            prestamo.IdPrestatario = userId;
            prestamo.PagoDiario = ObtenerPagoDiario(prestamo);

            _db.Prestamos.Add(prestamo);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Detalle(int id)
        {
            Prestamo prestamo = _db.Prestamos.Where(p => p.Id == id)
                .Include(p => p.IdPrestatarioNavigation)
                .Include(p => p.IdMontoNavigation)
                .Include(p => p.IdDuracionNavigation)
                .FirstOrDefault();

            return View(prestamo);
        }

        public decimal? ObtenerPagoDiario(Prestamo prestamo)
        {
            decimal? montoDiario;

            decimal? monto = _db.Montos.First(m => m.Id == prestamo.IdMonto).Valor;
            decimal? interes = _db.Duracions.First(d => d.Id == prestamo.IdDuracion).Interes;
            int? diasDuracion = _db.Duracions.First(d => d.Id == prestamo.IdDuracion).Duracion1;

            decimal? montoConInteres = monto + (monto * interes);

            montoDiario = montoConInteres / diasDuracion;
                        
            return Math.Round(montoDiario.GetValueOrDefault(), 3);
        }

        public DateTime SumarDiasLaborales(DateTime fechaInicio, int dias)
        {
            DateTime fechaFin = fechaInicio;
            int diasLaborables = 0;
            int diasAgregados = 0;

            while (diasAgregados < dias)
            {
                fechaFin = fechaFin.AddDays(1);
                if (fechaFin.DayOfWeek != DayOfWeek.Saturday && fechaFin.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasLaborables++;
                    diasAgregados++;
                }
            }

            return fechaFin;
        }

    }
}
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

            // Obtener el ID del prestamista registrador del prestatario actual desde la base de datos
            int? prestamistaRegistradorId = _db.Usuarios
                .Where(u => u.Id == userId)
                .Select(u => u.IdUsuarioCreador)
                .FirstOrDefault();

            // Si se pudo obtener el ID del prestamista registrador, proceder con la creación del préstamo
            if (prestamistaRegistradorId != null)
            {
                Prestamo prestamo = vistaPrestamo.Prestamo;
                prestamo.Estado = "Pendiente";

                // Sumar la duración del préstamo en días a la fecha de inicio
                prestamo.FechaFin = prestamo.FechaInicio.GetValueOrDefault().AddDays(prestamo.IdDuracionNavigation?.Duracion1 ?? 0);

                prestamo.IdPrestatario = userId;
                prestamo.IdPrestamista = prestamistaRegistradorId; // Asignar el ID del prestamista registrador al préstamo

                prestamo.PagoDiario = ObtenerPagoDiario(prestamo);

                _db.Prestamos.Add(prestamo);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");// Redireccionar a alguna vista adecuada
        }


        public IActionResult Detalle(int id)
        {
            Prestamo prestamo = _db.Prestamos
               .Include(p => p.IdPrestatarioNavigation)
               .Include(p => p.IdMontoNavigation)
               .Include(p => p.IdDuracionNavigation)
               .FirstOrDefault(p => p.Id == id);

            if (prestamo != null)
            {
                // Calcular el monto total sumando el interés al monto inicial
                decimal montoTotal = Math.Round((prestamo.IdMontoNavigation?.Valor ?? 0) * (1 + (prestamo.IdDuracionNavigation?.Interes ?? 0)), 2);

                // Calcular los días laborables en el rango del préstamo
                int diasLaborables = CalcularDiasLaborables(prestamo.FechaInicio.GetValueOrDefault(), prestamo.IdDuracionNavigation?.Duracion1 ?? 0);

                // Calcular el pago diario basado en los días laborables
                decimal pagoDiario = diasLaborables != 0 ? Math.Round(montoTotal / diasLaborables, 3) : 0;

                // Asignar el pago diario al modelo
                prestamo.PagoDiario = pagoDiario;

                // Sumar la duración del préstamo a la fecha de inicio
                prestamo.FechaFin = SumarDias(prestamo.FechaInicio.GetValueOrDefault(), prestamo.IdDuracionNavigation?.Duracion1 ?? 0);

                // Devolver la vista con el préstamo y sus detalles calculados
                return View(prestamo);
            }
            else
            {
                return NotFound();
            }
        }

        public decimal? ObtenerPagoDiario(Prestamo prestamo)
        {
            decimal? montoDiario;

            decimal? monto = _db.Montos.FirstOrDefault(m => m.Id == prestamo.IdMonto)?.Valor;
            decimal? interes = _db.Duracions.FirstOrDefault(d => d.Id == prestamo.IdDuracion)?.Interes;
            int? diasDuracion = _db.Duracions.FirstOrDefault(d => d.Id == prestamo.IdDuracion)?.Duracion1;

            if (monto != null && interes != null && diasDuracion != null && diasDuracion != 0)
            {
                decimal montoConInteres = monto.Value * (1 + interes.Value);

                // Calcular la fecha de finalización basada en los días laborables
                DateTime fechaInicio = prestamo.FechaInicio.GetValueOrDefault();
                DateTime fechaFin = SumarDiasLaborales(fechaInicio, diasDuracion.Value);

                // Contar los días laborables en el rango del préstamo
                int diasLaborables = 0;
                for (DateTime fecha = fechaInicio; fecha <= fechaFin; fecha = fecha.AddDays(1))
                {
                    if (fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday)
                    {
                        diasLaborables++;
                    }
                }

                // Calcular el pago diario basado en los días laborables
                montoDiario = Math.Round(montoConInteres / diasLaborables, 3);
            }
            else
            {
                montoDiario = null;
            }

            return montoDiario;
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

        public DateTime SumarDias(DateTime fechaInicio, int dias)
        {
            return fechaInicio.AddDays(dias);
        }


        private int CalcularDiasLaborables(DateTime fechaInicio, int dias)
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

            return diasLaborables;
        }

    }
}
using System;
using System.Collections.Generic;

namespace Aplicacion_Banco.Models
{
    public partial class Prestamo
    {
        public int Id { get; set; }
        public string? Zona { get; set; }
        public string? NumeroCuenta { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? PagoDiario { get; set; }
        public int? IdMonto { get; set; }
        public int? IdDuracion { get; set; }
        public int? IdMetodoPago { get; set; }
        public int? IdPrestamista { get; set; }
        public int? IdPrestatario { get; set; }

        public virtual Duracion? IdDuracionNavigation { get; set; }
        public virtual Metodopago? IdMetodoPagoNavigation { get; set; }
        public virtual Monto? IdMontoNavigation { get; set; }
        public virtual Usuario? IdPrestamistaNavigation { get; set; }
        public virtual Usuario? IdPrestatarioNavigation { get; set; }
    }
}

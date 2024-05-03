using System;
using System.Collections.Generic;

namespace Aplicacion_Banco.Models
{
    public partial class Monto
    {
        public Monto()
        {
            Prestamos = new HashSet<Prestamo>();
        }

        public int Id { get; set; }
        public decimal? Valor { get; set; }

        public virtual ICollection<Prestamo> Prestamos { get; set; }
    }
}

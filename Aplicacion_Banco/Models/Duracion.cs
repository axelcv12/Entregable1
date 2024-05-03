using System;
using System.Collections.Generic;

namespace Aplicacion_Banco.Models
{
    public partial class Duracion
    {
        public Duracion()
        {
            Prestamos = new HashSet<Prestamo>();
        }

        public int Id { get; set; }
        public int? Duracion1 { get; set; }
        public decimal? Interes { get; set; }

        public virtual ICollection<Prestamo> Prestamos { get; set; }
    }
}

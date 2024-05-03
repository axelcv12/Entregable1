using System;
using System.Collections.Generic;

namespace Aplicacion_Banco.Models
{
    public partial class Metodopago
    {
        public Metodopago()
        {
            Prestamos = new HashSet<Prestamo>();
        }

        public int Id { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<Prestamo> Prestamos { get; set; }
    }
}

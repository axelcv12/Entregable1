using System;
using System.Collections.Generic;

namespace Aplicacion_Banco.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            PrestamoIdPrestamistaNavigations = new HashSet<Prestamo>();
            PrestamoIdPrestatarioNavigations = new HashSet<Prestamo>();
        }

        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Edad { get; set; }
        public string? Direccion { get; set; }
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
        public string? ConfContrasena { get; set; }
        public int? IdRol { get; set; }
        public int? IdUsuarioCreador { get; set; }

        public virtual Rol? IdRolNavigation { get; set; }
        public virtual ICollection<Prestamo> PrestamoIdPrestamistaNavigations { get; set; }
        public virtual ICollection<Prestamo> PrestamoIdPrestatarioNavigations { get; set; }
    }
}

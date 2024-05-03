namespace Aplicacion_Banco.Models
{
    public class VistaPrestamo
    {
        public Prestamo Prestamo { get; set; }
        public List<Prestamo> Prestamos { get; set; }
        public List<Monto> Montos { get; set; }
        public List<Duracion> Duraciones { get; set; }
        public List<Metodopago> Metodopagos { get; set; }
        public List<Usuario> Prestamistas { get; set; }

    }
}

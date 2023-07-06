using Invextil.Entity;

namespace Invextil.App.Models.ViewModels
{
    public class VMCita
    {
        public int IdCita { get; set; }

        public int? IdEmpleado { get; set; }
        public string? nombre { get; set; }

        public string? Cliente { get; set; }

        public string? Fecha { get; set; }

        public string? Hora { get; set; }

        public string? Direccion { get; set; }

    }
}

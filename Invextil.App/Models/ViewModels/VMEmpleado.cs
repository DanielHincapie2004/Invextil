using Invextil.Entity;

namespace Invextil.App.Models.ViewModels
{
    public class VMEmpleado
    {
        public int IdEmpleado { get; set; }

        public string? Nombre { get; set; }

        public string? Correo { get; set; }

        public int? IdRol { get; set; }

        public string? NombreRol { get; set; }
        public string? UrlFoto { get; set; }
        public int? EsActivo { get; set; }

    }
}

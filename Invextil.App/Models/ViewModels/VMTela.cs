using Invextil.Entity;

namespace Invextil.App.Models.ViewModels
{
    public class VMTela
    {
        public int IdProducto { get; set; }

        public string? CodigoBarra { get; set; }

        public string? Descripcion { get; set; }

        public int? IdCategoria { get; set; }
        public string? nombreCategoria { get; set; }

        public int? Stock { get; set; }

        public string? UrlImagen { get; set; }

        public string? Precio { get; set; }

        public int? EsActivo { get; set; }


    }
}

using System;
using System.Collections.Generic;

namespace Invextil.Entity;

public partial class Tela
{
    public int IdProducto { get; set; }

    public string? CodigoBarra { get; set; }

    public string? Marca { get; set; }

    public string? Descripcion { get; set; }

    public int? IdCategoria { get; set; }

    public int? Stock { get; set; }

    public string? UrlImagen { get; set; }

    public string? NombreImagen { get; set; }

    public decimal? Precio { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Familia? IdCategoriaNavigation { get; set; }

    public virtual ICollection<Muestra> Muestras { get; set; } = new List<Muestra>();
}

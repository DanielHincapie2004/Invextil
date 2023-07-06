using System;
using System.Collections.Generic;

namespace Invextil.Entity;

public partial class Venta
{
    public int IdVenta { get; set; }

    public string? NumeroVenta { get; set; }

    public int? IdTipoDocumentoVenta { get; set; }

    public int? IdEmpleado { get; set; }

    public string? DocumentoCliente { get; set; }

    public string? NombreCliente { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? ImpuestoTotal { get; set; }

    public decimal? Total { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Empleado? IdEmpleadoNavigation { get; set; }

    public virtual TipoDocumentoVenta? IdTipoDocumentoVentaNavigation { get; set; }
}

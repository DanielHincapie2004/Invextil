using System;
using System.Collections.Generic;

namespace Invextil.Entity;

public partial class Muestra
{
    public int IdMuestra { get; set; }

    public int? IdTela { get; set; }

    public string? DocumentoCliente { get; set; }

    public string? NombreCliente { get; set; }

    public string? Direccion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Tela? IdTelaNavigation { get; set; }
}

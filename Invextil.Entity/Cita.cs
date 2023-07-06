using System;
using System.Collections.Generic;

namespace Invextil.Entity;

public partial class Cita
{
    public int IdCita { get; set; }

    public int? IdEmpleado { get; set; }

    public string? Cliente { get; set; }

    public string? Fecha { get; set; }

    public string? Hora { get; set; }

    public string? Direccion { get; set; }

    public virtual Empleado? IdEmpleadoNavigation { get; set; }
}

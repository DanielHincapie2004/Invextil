using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invextil.Entity;

namespace Invextil.DAL.Interfaces
{
    public interface IVentaRepository : IGenericRepository<Venta>
    {
        //interfaz con los metodos necesarios para crear las cotizaciones
        Task<Venta> Registrar(Venta entidad);
        Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin);
    }
}

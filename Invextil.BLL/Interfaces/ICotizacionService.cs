using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invextil.Entity;

namespace Invextil.BLL.Interfaces
{
    public interface ICotizacionService
    {

        Task<List<Tela>> ObtenerTelas(string busqueda);
        Task<Venta> Registrar(Venta entidad);
        Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin);
        Task<Venta> Detalle(string numeroVenta);
        Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin);

    }
}

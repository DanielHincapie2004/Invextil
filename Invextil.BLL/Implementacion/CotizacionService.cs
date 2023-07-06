using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Invextil.BLL.Interfaces;
using Invextil.DAL.Interfaces;
using Invextil.Entity;
using System.Globalization;

namespace Invextil.BLL.Implementacion
{
    public class CotizacionService : ICotizacionService
    {

        private readonly IGenericRepository<Tela> _telaRepositorio;
        private readonly IVentaRepository _repositorioVenta;

        public CotizacionService(IGenericRepository<Tela> telaRepositorio, IVentaRepository repositorioVenta)
        {
            _telaRepositorio = telaRepositorio;
            _repositorioVenta = repositorioVenta;
        }

        //Metodo para obtener las telas que estan activas y se encuentre en stock
        public async Task<List<Tela>> ObtenerTelas(string busqueda)
        {
            IQueryable<Tela> query = await _telaRepositorio.Consultar(p => p.EsActivo == true && p.Stock > 0 && string.Concat(p.CodigoBarra, p.Descripcion).Contains(busqueda));
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        //Metodo para guardar una cotizacion
        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _repositorioVenta.Registrar(entidad);
            }
            catch
            {
                throw;
            }
        }

        //metodo que nos permite visualizar las cotizacones creadas por fecha o numero de cotizacion
        public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar();
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;

            if (fechaInicio != "" && fechaFin != "")
            {
                DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
                DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                return query.Where(v =>
                v.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                v.FechaRegistro.Value.Date <= fecha_fin.Date
                )
                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdEmpleadoNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }
            else
            {
                return query.Where(v =>
                v.NumeroVenta == numeroVenta
                )
                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdEmpleadoNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }

        }
        //Metodo para ver a detalle la cotizacion
        public async Task<Venta> Detalle(string numeroVenta)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.NumeroVenta == numeroVenta);
            return query.Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdEmpleadoNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .First();
        }
        public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
        {
            DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
            DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

            List<DetalleVenta> lista = await _repositorioVenta.Reporte(fecha_inicio, fecha_fin);

           return lista;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Invextil.DAL.DBContext;
using Invextil.DAL.Interfaces;
using Invextil.Entity;

namespace Invextil.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        //Implementacion de cada uno de los metodos anteriormente creados en la intefaz 
        //Asignandole las solicitudes que se le haran a la base de datos  

        private readonly InvextilContext _dbContext;

        public VentaRepository(InvextilContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    //Generar numero para cada una de las cotizaciones
                    //Guardar el numero generado en la bd 
                    await _dbContext.SaveChangesAsync();
                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion == "Cotizacion").First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeroVenta;

                    await _dbContext.Venta.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            List<DetalleVenta> listaResumen = await _dbContext.DetalleVenta
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(u => u.IdEmpleadoNavigation)
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)

                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date &&
                    dv.IdVentaNavigation.FechaRegistro.Value.Date <= FechaFin.Date
                ).ToListAsync();
            return listaResumen;
        }
    }
}

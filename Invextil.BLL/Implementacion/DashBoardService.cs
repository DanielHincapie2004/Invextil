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
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _repositoryDetalleVenta;
        private readonly IGenericRepository<Familia> _repositoryFamilia;
        private readonly IGenericRepository<Tela> _repositoryTela;
        private DateTime FechaInicio = DateTime.Now;

        public DashBoardService(IVentaRepository ventaRepository, 
            IGenericRepository<DetalleVenta> repositoryDetalleVenta,
            IGenericRepository<Familia> repositoryFamilia,
            IGenericRepository<Tela> repositoryTela
            )
        {
            _ventaRepository = ventaRepository;
            _repositoryDetalleVenta = repositoryDetalleVenta;
            _repositoryFamilia = repositoryFamilia;
            _repositoryTela = repositoryTela;

            FechaInicio = FechaInicio.AddDays(-7);
        }

        //Metodos para calcular las telas, categorias y cotizaciones realizadas en la ultima semana
        public async Task<int> TotalCotizacionesUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalFamilias()
        {
            try
            {
                IQueryable<Familia> query = await _repositoryFamilia.Consultar();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalTelas()
        {
            try
            {
                IQueryable<Tela> query = await _repositoryTela.Consultar();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Dictionary<string, int>> CotizacionesUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository
                    .Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);

                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key)
                    .Select(dv => new {fecha = dv.Key.ToString("dd//MM//yy"), total = dv.Count()})
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> TelasTopUltimaSemana()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _repositoryDetalleVenta.Consultar();

                Dictionary<string, int> resultado = query
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date)
                    .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count())
                    .Select(dv => new { tela = dv.Key, total = dv.Count() }).Take(4)
                    .ToDictionary(keySelector: r => r.tela, elementSelector: r => r.total);

                return resultado; 
            }
            catch
            {
                throw;
            }
        }

       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invextil.BLL.Implementacion;
using Invextil.BLL.Interfaces;
using Invextil.DAL.Interfaces;
using Invextil.Entity;

namespace Invextil.BLL.Implementacion
{
    public class TipoDocumentoService : ITipoDocumentoService
    {
        private readonly IGenericRepository<TipoDocumentoVenta> _repositorio;
        public TipoDocumentoService(IGenericRepository<TipoDocumentoVenta> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<TipoDocumentoVenta>> Lista()
        {
            IQueryable<TipoDocumentoVenta> query = await _repositorio.Consultar();
            return query.ToList();
        }
    }
}

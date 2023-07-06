using Invextil.BLL.Interfaces;
using Invextil.DAL.Interfaces;
using Invextil.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invextil.BLL.Implementacion
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _repositorio;
        public RolService(IGenericRepository<Rol> repositorio)
        {
            _repositorio = repositorio;
        }
        //Metodo para listra roles
        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await _repositorio.Consultar();
            return query.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invextil.BLL.Interfaces;
using Invextil.DAL.Interfaces;
using Invextil.Entity;


namespace Invextil.BLL.Implementacion
{
    public class FamiliaService : IFamiliaService
    {

        private readonly IGenericRepository<Familia> _repository;

        public FamiliaService(IGenericRepository<Familia> repository)
        {
            _repository = repository;
        }

        //Metodos para crear, editar, listar y eliminar familias de tela 
        //Usando los metodosgenericos 
        public async Task<List<Familia>> Lista()
        {
            IQueryable<Familia> query = await _repository.Consultar();
            return query.ToList();
        }
        public async Task<Familia> Crear(Familia entidad)
        {
            try
            {
                Familia familia_creada = await _repository.Crear(entidad);
                if(familia_creada.IdCategoria == 0)
                {
                    throw new TaskCanceledException("No se creo la familia de tela");
                }
                return familia_creada;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<Familia> Editar(Familia entidad)
        {
            try
            {
                Familia familia_encontrada = await _repository.Obtener(c => c.IdCategoria == entidad.IdCategoria);
                familia_encontrada.Descripcion = entidad.Descripcion;
                familia_encontrada.EsActivo = entidad.EsActivo;

                bool respuesta = await _repository.Editar(familia_encontrada);

                if(!respuesta)
                    throw new TaskCanceledException("No se edito la familia de tela");

                return familia_encontrada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idFamilia)
        {
            try
            {
                Familia familia_encontrada = await _repository.Obtener(c => c.IdCategoria == idFamilia);

                if(familia_encontrada == null)
                {
                    throw new TaskCanceledException("La familia de tela no existe");
                }

                bool respuesta = await _repository.Eliminar(familia_encontrada);

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        
    }
}

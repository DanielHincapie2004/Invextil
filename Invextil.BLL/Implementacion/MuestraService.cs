using Invextil.BLL.Interfaces;
using Invextil.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invextil.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invextil.BLL.Implementacion
{
    public class MuestraService : IMuestraService
    {
        private readonly IGenericRepository<Muestra> _repositorio;

        public MuestraService(IGenericRepository<Muestra> repositorio)
        {
            _repositorio = repositorio;
        }

        //Metodos para crear, actualizar, listar y eliminar muestras de tela
        public async Task<Muestra> Crear(Muestra entidad)
        {
            try
            {
                Muestra muestra_creada = await _repositorio.Crear(entidad);
                if (muestra_creada.IdMuestra == 0)
                {
                    throw new TaskCanceledException("No se creo la Muestra");
                }
                IQueryable<Muestra> query = await _repositorio.Consultar(p => p.IdTela == muestra_creada.IdTela);
                muestra_creada = query.Include(t => t.IdTelaNavigation).First();
                return muestra_creada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Muestra> Editar(Muestra entidad)
        {
            Muestra muestraExiste = await _repositorio.Obtener(p => p.IdMuestra == entidad.IdMuestra);

            try
            {
                IQueryable<Muestra> queryMuestra = await _repositorio.Consultar(u => u.IdTela == entidad.IdTela);
                Muestra muestraEditar = queryMuestra.First();
                muestraEditar.IdTela = entidad.IdTela;
                muestraEditar.DocumentoCliente = entidad.DocumentoCliente;
                muestraEditar.NombreCliente = entidad.NombreCliente;
                muestraEditar.Direccion = entidad.Direccion;

                bool respuesta = await _repositorio.Editar(muestraEditar);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar la Muestra");
                }

                Muestra citaEditada = queryMuestra.Include(c => c.IdTelaNavigation).First();
                return citaEditada;

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idMuestra)
        {
            try
            {
                Muestra muestra_encontrada = await _repositorio.Obtener(c => c.IdMuestra == idMuestra);

                if (muestra_encontrada == null)
                {
                    throw new TaskCanceledException("La Muestra no existe");
                }

                bool respuesta = await _repositorio.Eliminar(muestra_encontrada);

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Muestra>> Lista()
        {
            IQueryable<Muestra> query = await _repositorio.Consultar();
            return query.Include(c => c.IdTelaNavigation).ToList();
        }
    }


}

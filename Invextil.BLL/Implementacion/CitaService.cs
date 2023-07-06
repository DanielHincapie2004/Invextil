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
    public class CitaService : ICitaService
    {

        //Usamos los metodos genericos crreados en IGenericRepository pero le asignamos que sera usado 
        //especificamente con la clase de citas

        private readonly IGenericRepository<Cita> _repositorio;
        public CitaService(IGenericRepository<Cita> repositorio)
        {
            _repositorio = repositorio;
        }

        //Metodo listar citas
        public async Task<List<Cita>> Lista()
        {
            IQueryable<Cita> query = await _repositorio.Consultar();
            return query.Include(c=>c.IdEmpleadoNavigation).ToList();
        }

        //Metodo crear citas
        public async Task<Cita> Crear(Cita entidad)
        {
            try
            {
                Cita cita_creada = await _repositorio.Crear(entidad);
                if (cita_creada.IdCita == 0)
                {
                    throw new TaskCanceledException("No se creo la Cita");
                }
                IQueryable<Cita> query = await _repositorio.Consultar(p => p.IdEmpleado == cita_creada.IdEmpleado);
                cita_creada = query.Include(t => t.IdEmpleadoNavigation).First();
                return cita_creada;
            }
            catch
            {
                throw;
            }
        }

        //Metodo editar citas
        public async Task<Cita> Editar(Cita entidad)
        {
            Cita citaExiste = await _repositorio.Obtener(p => p.IdCita == entidad.IdCita);

            try
            {
                IQueryable<Cita> queryCita = await _repositorio.Consultar(u => u.IdCita == entidad.IdCita);
                Cita citaEditar = queryCita.First();
                citaEditar.IdEmpleado = entidad.IdEmpleado;
                citaEditar.Cliente = entidad.Cliente;
                citaEditar.Hora = entidad.Hora;
                citaEditar.Fecha = entidad.Fecha;
                citaEditar.Direccion = entidad.Direccion;

                bool respuesta = await _repositorio.Editar(citaEditar);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar la Cita");
                }

                Cita citaEditada = queryCita.Include(c => c.IdEmpleadoNavigation).First();
                return citaEditada;

            }
            catch
            {
                throw;
            }
        }

        //Metodo eliminar citas
        public async Task<bool> Eliminar(int idCita)
        {
            try
            {
                Cita cita_encontrada = await _repositorio.Obtener(c => c.IdCita == idCita);

                if (cita_encontrada == null)
                {
                    throw new TaskCanceledException("La Cita no existe");
                }

                bool respuesta = await _repositorio.Eliminar(cita_encontrada);

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

       
    }
}

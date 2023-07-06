using Invextil.BLL.Interfaces;
using Invextil.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Invextil.DAL.Interfaces;

namespace Invextil.BLL.Implementacion
{
    public class TelaService : ITelaService
    {
        private readonly IGenericRepository<Tela> _repositorio;
        private readonly IFirebaseService _firebase;

        public TelaService(IGenericRepository<Tela> repositorio, IFirebaseService firebase)
        {
            _repositorio = repositorio;
            _firebase = firebase;
        }

        //Metodos para listar, editar, eliminar y crear telas
        public async Task<List<Tela>> Lista()
        {
            IQueryable<Tela> query = await _repositorio.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        //Metodo para crear tela y subir la imagen a firebase
        public async Task<Tela> Crear(Tela entidad, Stream imagen = null, string nombreImagen = "")
        {
            Tela telaExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);
            if(telaExiste != null)
            {
                throw new TaskCanceledException("El codigo de barra ya existe");
            }

            try
            {
                entidad.NombreImagen = nombreImagen;
                if(imagen != null)
                {
                    string urlImagen = await _firebase.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen = urlImagen;
                }

                Tela telaCreada = await _repositorio.Crear(entidad);
                if(telaCreada.IdProducto == 0)
                {
                    throw new TaskCanceledException("no se pudo crear el producto");
                }

                IQueryable<Tela> query = await _repositorio.Consultar(p => p.IdProducto == telaCreada.IdProducto);
                telaCreada = query.Include(t => t.IdCategoriaNavigation).First();

                return telaCreada;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Tela> Editar(Tela entidad, Stream imagen = null)
        {
            Tela productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);

            if(productoExiste != null)
            {
                throw new TaskCanceledException("El codigo de barras ya existe");
            }

            try
            {
                IQueryable<Tela> queryTela = await _repositorio.Consultar(u => u.IdProducto == entidad.IdProducto);
                Tela telaEditar = queryTela.First();
                telaEditar.CodigoBarra = entidad.CodigoBarra;
                telaEditar.Descripcion = entidad.Descripcion;
                telaEditar.IdCategoria = entidad.IdCategoria;
                telaEditar.Stock = entidad.Stock;
                telaEditar.Precio = entidad.Precio;
                telaEditar.EsActivo = entidad.EsActivo;

                if(imagen != null)
                {
                    string urlImagen = await _firebase.SubirStorage(imagen, "carpeta_producto", telaEditar.NombreImagen);
                    telaEditar.UrlImagen = urlImagen;
                }

                bool respuesta = await _repositorio.Editar(telaEditar);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar la tela");
                }

                Tela telaEditada = queryTela.Include(c => c.IdCategoriaNavigation).First();
                return telaEditada;

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idTela)
        {
            try
            {
                Tela telaEncontrada = await _repositorio.Obtener(p => p.IdProducto == idTela);
                
                if(telaEncontrada == null)
                {
                    throw new TaskCanceledException("La tela no existe");
                    
                }

                string nombreImagen = telaEncontrada.NombreImagen;
                bool respuesta = await _repositorio.Eliminar(telaEncontrada);
                if (respuesta)
                {
                    await _firebase.EliminarStorage("carpeta_producto", nombreImagen);
                    
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        
    }
}

using Invextil.BLL.Interfaces;
using Invextil.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Net;
using Invextil.DAL.Interfaces;


namespace Invextil.BLL.Implementacion
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IGenericRepository<Empleado> _repositorio;
        private readonly IFirebaseService _firebaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;

        public EmpleadoService(IGenericRepository<Empleado> repositorio, IFirebaseService firebaseService, IUtilidadesService utilidadesService, ICorreoService correoService)
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }
        public async Task<List<Empleado>> ListaAsesor()
        {
            IQueryable<Empleado> query = await _repositorio.Consultar(c => c.IdRol == 2);
            return query.Include(r => r.IdRolNavigation).ToList();
        }

        public async Task<List<Empleado>> Lista()
        {
            IQueryable<Empleado> query = await _repositorio.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }

        //Metodo para crear empleados
        public async Task<Empleado> Crear(Empleado entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Empleado empleadoExiste = await _repositorio.Obtener(u => u.Correo == entidad.Correo);

            if (empleadoExiste != null)
                throw new TaskCanceledException("El correo ya exite");

            try
            {
                //Genera clave encriptada
                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = urlFoto;
                }

                Empleado empleadoCreado = await _repositorio.Crear(entidad);

                if (empleadoCreado.IdEmpleado == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario");

                //Genera el correo con la contraseña creada anteriormente
                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", empleadoCreado.Correo).Replace("[clave]", clave_generada);

                    string htmlCorreo = "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                            {
                                readerStream = new StreamReader(dataStream);
                            }
                            else
                            {
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }

                    if (htmlCorreo != "")
                        await _correoService.EnviarCorreo(empleadoCreado.Correo, "Cuenta Creada", htmlCorreo);
                }
                IQueryable<Empleado> query = await _repositorio.Consultar(u => u.IdEmpleado == empleadoCreado.IdEmpleado);
                empleadoCreado = query.Include(r => r.IdRolNavigation).First();

                return empleadoCreado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Edita el empleado
        public async Task<Empleado> Editar(Empleado entidad, Stream Foto = null, string NombreFoto = "")
        {
            Empleado empleadoExiste = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdEmpleado != entidad.IdEmpleado);

            if (empleadoExiste != null)
                throw new TaskCanceledException("El correo ya exite");

            try
            {
                IQueryable<Empleado> queryEmpleado = await _repositorio.Consultar(u => u.IdEmpleado == entidad.IdEmpleado);
                Empleado empleadoEditar = queryEmpleado.First();
                empleadoEditar.Nombre = entidad.Nombre;
                empleadoEditar.Correo = entidad.Correo;
                empleadoEditar.IdRol = entidad.IdRol;
                empleadoEditar.EsActivo = entidad.EsActivo;

                if (empleadoEditar.NombreFoto == "")
                {
                    empleadoEditar.NombreFoto = NombreFoto;
                }

                if (Foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", empleadoEditar.NombreFoto);
                    empleadoEditar.UrlFoto = urlFoto;
                }

                bool respuesta = await _repositorio.Editar(empleadoEditar);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se edito el usuario");
                }
                Empleado empleadoEditado = queryEmpleado.Include(r => r.IdRolNavigation).First();

                return empleadoEditado;

            }
            catch
            {
                throw;
            }
        }


        public async Task<bool> Eliminar(int idEmpleado)
        {
            try
            {
                Empleado empleadoEncontrado = await _repositorio.Obtener(r => r.IdEmpleado == idEmpleado);

                if (empleadoEncontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                string nombreFoto = empleadoEncontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(empleadoEncontrado);

                if (respuesta)
                    await _firebaseService.EliminarStorage("carpeta_usuario", nombreFoto);

                return true;
            }
            catch
            {
                throw;
            }
        }

        //Busqueda de empleados en base a sus credenciales 
        public async Task<Empleado> ObtenerPorCredenciales(string correo, string clave)
        {
            string claveEncriptada = _utilidadesService.ConvertirSha256(clave);
            Empleado empleadoEncontrado = await _repositorio.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(claveEncriptada));

            return empleadoEncontrado;
        }

        //Busqueda de empleados en base a su id
        public async Task<Empleado> ObtenerPorId(int idEmpleado)
        {
            IQueryable<Empleado> query = await _repositorio.Consultar(u => u.IdEmpleado == idEmpleado);
            Empleado resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();
            return resultado;
        }
        public async Task<bool> GuardarPerfil(Empleado entidad)
        {
            try
            {
                Empleado empleadoEncontardo = await _repositorio.Obtener(u => u.IdEmpleado == entidad.IdEmpleado);

                if (empleadoEncontardo == null)
                    throw new TaskCanceledException("El usuario no existe");

                empleadoEncontardo.Correo = entidad.Correo;

                bool respuesta = await _repositorio.Editar(empleadoEncontardo);

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        //Metodo para modificar la contraseña
        public async Task<bool> CambiarClave(int idEmpleado, string ClaveActual, string ClaveNueva)
        {
            try
            {
                Empleado empleadoEncontrado = await _repositorio.Obtener(u => u.IdEmpleado == idEmpleado);

                if (empleadoEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                if (empleadoEncontrado.Clave != _utilidadesService.ConvertirSha256(ClaveActual))
                    throw new TaskCanceledException("La contraseña no coincide");

                empleadoEncontrado.Clave = _utilidadesService.ConvertirSha256(ClaveNueva);
                bool respuesta = await _repositorio.Editar(empleadoEncontrado);
                return respuesta;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //metodo para restablecer contraseña y envia el correo con la clave restablecida
        public async Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            try
            {
                Empleado empleadoEncontrado = await _repositorio.Obtener(u => u.Correo == correo);
                if (empleadoEncontrado == null)
                    throw new TaskCanceledException("No encontramos ningun usuario asociado al correo");

                string claveGenerada = _utilidadesService.GenerarClave();
                empleadoEncontrado.Clave = _utilidadesService.ConvertirSha256(claveGenerada);

                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", claveGenerada);

                string htmlCorreo = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Usar data stream
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readerStream = null;

                        if (response.CharacterSet == null)
                        {
                            readerStream = new StreamReader(dataStream);
                        }
                        else
                        {
                            readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                        }
                        htmlCorreo = readerStream.ReadToEnd();
                        response.Close();
                        readerStream.Close();
                    }
                }

                bool correoEnviado = false;

                if (htmlCorreo != "")
                    correoEnviado = await _correoService.EnviarCorreo(correo, "Contraseña Restablecida", htmlCorreo);

                if (correoEnviado == false)
                {
                    throw new TaskCanceledException("Error al enviar el corre, porfavor intente mas tarde");
                }

                bool respuesta = await _repositorio.Editar(empleadoEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }
    }
}

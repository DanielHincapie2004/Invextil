using Invextil.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invextil.BLL.Interfaces
{
    public interface IEmpleadoService
    {
        Task<List<Empleado>> Lista();
        Task<List<Empleado>> ListaAsesor();
        Task<Empleado> Crear(Empleado entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "");
        Task<Empleado> Editar(Empleado entidad, Stream Foto = null, string NombreFoto = "");
        Task<bool> Eliminar(int idEmpleado);
        Task<Empleado> ObtenerPorCredenciales(string correo, string clave);
        Task<Empleado> ObtenerPorId(int idEmpleado);
        Task<bool> GuardarPerfil(Empleado entidad);
        Task<bool> CambiarClave(int idEmpleado, string ClaveActual, string ClaveNueva);
        Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo);
    }
}

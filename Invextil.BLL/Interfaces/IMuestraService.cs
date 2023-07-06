using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invextil.Entity;


namespace Invextil.BLL.Interfaces
{
    public interface IMuestraService
    {
        Task<List<Muestra>> Lista();
        Task<Muestra> Crear(Muestra entidad);
        Task<Muestra> Editar(Muestra entidad);
        Task<bool> Eliminar(int idMuestra);

    }
}

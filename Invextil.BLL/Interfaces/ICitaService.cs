using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invextil.Entity;

namespace Invextil.BLL.Interfaces
{
    public interface ICitaService
    {

        Task<List<Cita>> Lista();
        Task<Cita> Crear(Cita entidad);
        Task<Cita> Editar(Cita entidad);
        Task<bool> Eliminar(int idCita);

    }
}

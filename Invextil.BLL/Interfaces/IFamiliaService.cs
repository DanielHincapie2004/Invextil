using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invextil.Entity;

namespace Invextil.BLL.Interfaces
{
    public interface IFamiliaService
    {

        Task<List<Familia>> Lista();

        Task<Familia> Crear(Familia entidad);
        Task<Familia> Editar(Familia entidad);
        Task<bool> Eliminar(int idFamilia);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invextil.Entity;

namespace Invextil.BLL.Interfaces
{
    public interface ITelaService
    {

        Task<List<Tela>> Lista();
        Task<Tela> Crear(Tela entidad, Stream imagen = null, string nombreImagen = "");
        Task<Tela> Editar(Tela entidad, Stream imagen = null);
        Task<bool> Eliminar(int idTela);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invextil.BLL.Interfaces
{
    public interface IUtilidadesService
    {
        string GenerarClave();

        //Encriptacion
        string ConvertirSha256(string texto);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invextil.Entity;

namespace Invextil.BLL.Interfaces
{
    public interface ITipoDocumentoService
    {

        Task<List<TipoDocumentoVenta>> Lista();

    }
}

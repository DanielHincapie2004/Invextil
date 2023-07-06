using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invextil.BLL.Interfaces
{
    public interface IDashBoardService
    {

        Task<int> TotalCotizacionesUltimaSemana();
        Task<int> TotalTelas();
        Task<int> TotalFamilias();
        Task<Dictionary<string,int>> CotizacionesUltimaSemana();
        Task<Dictionary<string,int>> TelasTopUltimaSemana();


    }
}

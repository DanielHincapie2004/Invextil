using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invextil.BLL.Interfaces;
using Invextil.DAL.Interfaces;
using Invextil.Entity;

namespace Invextil.BLL.Implementacion
{
    public class MenuService : IMenuService
    {
        private readonly IGenericRepository<Menu> _MenuRepository;
        private readonly IGenericRepository<RolMenu> _RolMenuRepository;
        private readonly IGenericRepository<Empleado> _EmpleadoRepository;

        public MenuService(IGenericRepository<Menu> MenuRepository, 
            IGenericRepository<RolMenu> RolMenuRepository, 
            IGenericRepository<Empleado> EmpleadoRepository)
        {
            _EmpleadoRepository = EmpleadoRepository;
            _MenuRepository = MenuRepository;
            _RolMenuRepository = RolMenuRepository;
        }

        // Metodo para cambiar el menu en base al rol del usuario que ingrese
        public async Task<List<Menu>> ObtenerMenu(int idUsuario)
        {
            IQueryable<Empleado> tbEmpleado = await _EmpleadoRepository.Consultar(c => c.IdEmpleado == idUsuario);
            IQueryable<RolMenu> tbRolMenu = await _RolMenuRepository.Consultar();
            IQueryable<Menu> tbMenu = await _MenuRepository.Consultar();

            IQueryable<Menu> MenuPadre = (from u in tbEmpleado
                                          join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          join mpadre in tbMenu on m.IdMenuPadre equals mpadre.IdMenu
                                          select mpadre).Distinct().AsQueryable();

            IQueryable<Menu> menuHijo = (from u in tbEmpleado
                                         join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                         join m in tbMenu on rm.IdMenu equals m.IdMenu
                                         where m.IdMenu != m.IdMenuPadre
                                         select m).Distinct().AsQueryable();

            List<Menu> listaMenu = (from mpadre in MenuPadre
                                    select new Menu()
                                    {
                                        Descripcion = mpadre.Descripcion,
                                        Icono = mpadre.Icono,
                                        Controlador = mpadre.Controlador,
                                        PaginaAccion = mpadre.PaginaAccion,
                                        InverseIdMenuPadreNavigation = (from mhijo in menuHijo
                                                                        where mhijo.IdMenuPadre == mpadre.IdMenu
                                                                        select mhijo).ToList()
                                    }).ToList();

            return listaMenu;
        }
    }
}

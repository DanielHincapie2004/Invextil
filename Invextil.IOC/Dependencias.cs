using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Invextil.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Invextil.DAL.Interfaces;
using Invextil.DAL.Implementacion;

using Invextil.BLL.Implementacion;
using Invextil.BLL.Interfaces;




namespace Invextil.IOC
{
    public static class Dependencias
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            //Uso de la cadena de conexion a la base de datos
            services.AddDbContext<InvextilContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });

            //Listado de dependencias a usar en el sistema donde se relaciona clase con interfaz
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVentaRepository, VentaRepository>();

            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddScoped<IUtilidadesService, UtilidadesService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IEmpleadoService, EmpleadoService>();
            services.AddScoped<IFamiliaService, FamiliaService>();
            services.AddScoped<ITelaService, TelaService>();
            services.AddScoped<ICitaService, CitaService>();
            services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();
            services.AddScoped<ICotizacionService, CotizacionService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            services.AddScoped<IMuestraService, MuestraService>();
            services.AddScoped<IMenuService, MenuService>();
        }
    }
}

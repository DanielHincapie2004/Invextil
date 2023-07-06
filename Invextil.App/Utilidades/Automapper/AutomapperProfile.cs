using Invextil.App.Models.ViewModels;
using Invextil.Entity;
using System.Globalization;
using AutoMapper;

namespace Invextil.App.Utilidades.Automapper
{
    public class AutomapperProfile : Profile
    {

        public AutomapperProfile()
        {
            //Definir conversion de modelos
            //PRimero origen - Destino
            #region Rol
            CreateMap<Rol, VMRol>().ReverseMap();
            #endregion

            #region Empleado

            CreateMap<Empleado, VMEmpleado>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                )
                .ForMember(destino =>
                destino.NombreRol,
                opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion)
                );

            CreateMap<VMEmpleado, Empleado>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(destino =>
                destino.IdRolNavigation,
                opt => opt.Ignore()
                );
            #endregion

            #region Cita

            CreateMap<Cita, VMCita>()
                .ForMember(destino =>
                destino.nombre,
                opt => opt.MapFrom(origen => origen.IdEmpleadoNavigation.Nombre)
                );
            CreateMap<VMCita, Cita>()
               .ForMember(destino =>
                destino.IdEmpleadoNavigation,
                opt => opt.Ignore()
                );
            #endregion

            #region Familia

            CreateMap<Familia, VMFamilia>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                );

            CreateMap<VMFamilia, Familia>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                );

            #endregion

            #region Tela

            CreateMap<Tela, VMTela>()
              .ForMember(destino =>
              destino.EsActivo,
              opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
              )
                .ForMember(destino =>
                destino.nombreCategoria,
                opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion)
                )
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-CO")))
                );
            CreateMap<VMTela, Tela>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(destino =>
                destino.IdCategoriaNavigation,
                opt => opt.Ignore()
                )
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-CO")))
                );
            #endregion

            #region Muestra

            CreateMap<Muestra, VMMuestra>()
                .ForMember(destino =>
                destino.Tela,
                opt => opt.MapFrom(origen => origen.IdTelaNavigation.Descripcion)
                );

            CreateMap<VMMuestra, Muestra>()
                .ForMember(destino =>
                destino.IdTelaNavigation,
                opt => opt.Ignore()
                );

            #endregion

            #region TipoDocumentoVenta
            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap();
            #endregion TipoDocumentoVenta

            #region Venta
            CreateMap<Venta, VMVenta>()
                .ForMember(destino =>
                destino.TipoDocumentoVenta,
                opt => opt.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion)
                )
                .ForMember(destino =>
                destino.Empleado,
                opt => opt.MapFrom(origen => origen.IdEmpleadoNavigation.Nombre)
                )
                .ForMember(destino =>
                destino.SubTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.ImpuestoTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.FechaRegistro,
                opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy"))
                );

            CreateMap<VMVenta, Venta>()
                .ForMember(destino =>
                destino.SubTotal,
                opt => opt.MapFrom(origen => Convert.ToDouble(origen.SubTotal, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.ImpuestoTotal,
                opt => opt.MapFrom(origen => Convert.ToDouble(origen.ImpuestoTotal, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToDouble(origen.Total, new CultureInfo("es-CO")))
                );

            #endregion Venta

            #region DetalleVenta
            CreateMap<DetalleVenta, VMDetalleVenta>()
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-CO")))
                );
            CreateMap<VMDetalleVenta, DetalleVenta>()
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToDouble(origen.Precio, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToDouble(origen.Total, new CultureInfo("es-CO")))
                );
            CreateMap<DetalleVenta, VMReporteVenta>()
                .ForMember(destino =>
                destino.FechaRegistro,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy"))
                )
                .ForMember(destino =>
                destino.NumeroVenta,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.NumeroVenta)
                )
                .ForMember(destino =>
                destino.TipoDocumento,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion)
                )
                .ForMember(destino =>
                destino.DocumentoCliente,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.DocumentoCliente)
                )
                .ForMember(destino =>
                destino.NombreCliente,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.NombreCliente)
                )
                .ForMember(destino =>
                destino.SubTotalVenta,
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.ImpuestoTotalVenta,
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.TotalVenta,
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.Total.Value, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.Producto,
                opt => opt.MapFrom(origen => origen.DescripcionProducto)
                )
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-CO")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-CO")))
                );
            #endregion DetalleVenta

            #region Menu
            CreateMap<Menu, VMMenu>()
                .ForMember(destino =>
                destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation)
                );
            #endregion
        }

    }
}

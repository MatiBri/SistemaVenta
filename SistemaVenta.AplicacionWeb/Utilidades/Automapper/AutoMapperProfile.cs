//Referencias
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.Entity; //Acá estan nuestros mdelos
using System.Globalization;
using AutoMapper;

namespace SistemaVenta.AplicacionWeb.Utilidades.Automapper
{
    public class AutoMapperProfile : Profile //Esta referencia viene de Automapper
    {
        public AutoMapperProfile()
        {
            //Acá definimos la conversion de los modelos a ViewModels. También al revés de Viewmodels a modelos

            //Todo esta configuracion va dentro del Automapper para poder usarlo
            #region Rol
            //Mapeo para Rol. Convertimos el modelo Rol a un ViewModel Rol
            //CreateMap<Rol, VMRol>(); //Esta clase Rol, se va a convertir en ViewModelRol
            CreateMap<Rol, VMRol>().ReverseMap(); //Esto convierte de un ViewModel a un modelo
            #endregion Rol

            #region Usuario
            //Inicio: Esto es para la conversion de Usuario
            CreateMap<Usuario, VMUsuario>().ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                )
                .ForMember(destino =>
                destino.NombreRol,
                opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion)
                );
            //Fin

            CreateMap<VMUsuario, Usuario>().ForMember(destino =>
            destino.EsActivo,
            opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
            )
                .ForMember(destino =>
                destino.IdRolNavigation,
                opt => opt.Ignore()
                );
            #endregion Usuario

            #region Negocio
            CreateMap<Negocio, VMNegocio>()
                .ForMember(destino =>
                destino.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value, new CultureInfo("es-AR")))
                );

            //Ahora hacemos lo contrario
            CreateMap<VMNegocio, Negocio>()
                .ForMember(destino =>
                destino.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto, new CultureInfo("es-AR")))
                );
            #endregion Negocio

            #region Categoria
            CreateMap<Categoria, VMCategoria>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                );

            //Al reves
            CreateMap<VMCategoria, Categoria>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                );
            #endregion Categoria

            #region Producto
            CreateMap<Producto, VMProducto>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                )
                .ForMember(destino =>
                destino.NombreCategoria,
                opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion) //Recordar que el idCategoriaNavigation quiere decir que estamos haciendo uso de esa clase que está agregada en otra clase
                //En este caso en Producto estamos haciendo uso de Categoria, porque tiene esa herencia dentro de la clase
                )
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-AR")))
                );

            //Lógica a la inversa
            CreateMap<VMProducto, Producto>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(destino =>
                destino.IdCategoriaNavigation,
                opt => opt.Ignore() //Ignora la lógica de convertir el Viewmodel de Producto a Producto
                )
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-AR")))
                );
            #endregion Producto

            #region TipoDocumentoVenta
            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap(); //Convierto a Viewmodel y que la conversión también puede suceder al revés
            #endregion TipoDocumentoVenta

            #region Venta
            CreateMap<Venta, VMVenta>()
                .ForMember(destino =>
                destino.TipoDocumentoVenta,
                opt => opt.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion)
                )
                .ForMember(destino =>
                destino.Usuario,
                opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre)
                )
                .ForMember(destino =>
                destino.SubTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.ImpuestoTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.FechaRegistro,
                opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy"))
                );

            //Lógica a la inversa
            CreateMap<VMVenta, Venta>()
                .ForMember(destino =>
                destino.SubTotal,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.ImpuestoTotal,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-AR")))
                );
            #endregion Venta

            #region DetalleVenta
            CreateMap<DetalleVenta, VMDetalleVenta>()
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-AR")))
                );

            //Lógica a la inversa
            CreateMap<VMDetalleVenta, DetalleVenta>()
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-AR")))
                );

            //Reporte de venta dentro del detalle
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
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.ImpuestoTotalVenta,
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.TotalVenta,
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.Total.Value, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.Producto,
                opt => opt.MapFrom(origen => origen.DescripcionProducto)
                )
                .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-AR")))
                )
                .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-AR")))
                );
            #endregion DetalleVenta

            #region Menu
            CreateMap<Menu, VMMenu>()
                .ForMember(destino =>
                destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation)
                );
            #endregion Menu
        }
    }
}

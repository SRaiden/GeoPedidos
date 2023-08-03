using GeoPedidos.AplicacionWeb.Models.ViewModels;
using GeoPedidos.Entity;
using System.Globalization;
using AutoMapper;

namespace GeoPedidos.AplicacionWeb.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region GeneralEmpresa
            CreateMap<GeneralEmpresa, VMGeneralEmpresa>()
                    .ForMember(destino =>
                    destino.Confirmar,
                    opt => opt.MapFrom(origen => origen.Confirmar == true ? 1 : 0)
                    );

            CreateMap<VMGeneralEmpresa, GeneralEmpresa>()
                .ForMember(destino =>
                destino.Confirmar,
                opt => opt.MapFrom(origen => origen.Confirmar == 1 ? true : false)
                );
            #endregion

            #region GeneralSucursales
            CreateMap<GeneralSucursales, VMGeneralSucursales>()
                    .ForMember(destino =>
                    destino.Activa,
                    opt => opt.MapFrom(origen => origen.Activa == true ? 1 : 0)
                    );

            CreateMap<VMGeneralSucursales, GeneralSucursales>()
                .ForMember(destino =>
                destino.Activa,
                opt => opt.MapFrom(origen => origen.Activa == 1 ? true : false)
                );
            #endregion

            #region Fabrica_Usuarios
            CreateMap<FabricaUsuario, VMFabricaUsuario>()
                    .ForMember(destino =>
                    destino.Active,
                    opt => opt.MapFrom(origen => origen.Active == true ? 1 : 0)
                    ).ForMember(destino =>
                    destino.OkLogin,
                    opt => opt.MapFrom(origen => origen.OkLogin == true ? 1 : 0)
                    ).ForMember(destino =>
                    destino.Created,
                    opt => opt.MapFrom(origen => origen.Created.Value.ToString("dd/MM/yyyy"))
                    ).ForMember(destino =>
                    destino.Modified,
                    opt => opt.MapFrom(origen => origen.Modified.Value.ToString("dd/MM/yyyy"))
                    );

            CreateMap<VMFabricaUsuario, FabricaUsuario>()
                .ForMember(destino =>
                destino.Active,
                opt => opt.MapFrom(origen => origen.Active == 1 ? true : false)
                ).ForMember(destino =>
                destino.OkLogin,
                opt => opt.MapFrom(origen => origen.OkLogin == 1 ? true : false)
                );
            #endregion

            #region Fabrica_Pedidos
            CreateMap<FabricaPedido, VMFabricaPedido>()
                    .ForMember(destino =>
                    destino.Created,
                    opt => opt.MapFrom(origen => origen.Created.Value.ToString("dd-MM-yyyy"))
                    ) ;

            CreateMap<VMFabricaPedido, FabricaPedido>()
                    .ForMember(destino =>
                    destino.Created,
                    opt => opt.MapFrom(origen => DateTime.Parse(origen.Created))
                    );
            #endregion

            #region Fabrica_Pedidos_Detalle
            CreateMap<FabricaPedidosDetalle, VMFabricaPedidoDetalle>()
                    .ForMember(destino =>
                    destino.Created,
                    opt => opt.MapFrom(origen => origen.Created.Value.ToString("dd-MM-yyyy"))
                    );

            CreateMap<VMFabricaPedidoDetalle, FabricaPedidosDetalle>()
                    .ForMember(destino =>
                    destino.Created,
                    opt => opt.MapFrom(origen => DateTime.Parse(origen.Created.ToString()))
                    );
            #endregion

            #region Fabrica_TODOSLOSPRODUCTOS
            CreateMap<FabricaGusto, VMListaProductos>().ReverseMap();
            CreateMap<FabricaInsumo, VMListaProductos>().ReverseMap();
            CreateMap<FabricaPasteleria, VMListaProductos>().ReverseMap();
            CreateMap<FabricaProducto, VMListaProductos>().ReverseMap();
            #endregion
        }
    }
}

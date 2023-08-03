using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GeoPedidos.DAL.DBContext;

using GeoPedidos.DAL.Interfaces;
using GeoPedidos.DAL.Implementacion;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.BLL.Implementacion;

namespace GeoPedidos.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GeoPedidosContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });

            // Conecto las interfaces con sus repositorios respectivos
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // ABM generico
            services.AddTransient(typeof(IPedidoRepository<>), typeof(PedidosRepository<>)); // ABM generico

            services.AddScoped<ICorreoServices, CorreoServices>(); // Servicio Correo
            services.AddScoped<IUtilidadesServices, UtilidadesServices>(); // Generar y Encriptar Claves de login
            services.AddScoped<IFabricaUsuariosServices, FabricaUsuariosServices>(); // Fabrica_Usuarios
            services.AddScoped<IEmpresasServices, EmpresasServices>(); // Empresas
            services.AddScoped<ISucursalServices, SucursalServices>(); // Sucursales
            services.AddScoped<IPedidosServices, PedidosServices>(); // Pedidos
        }
    }
}

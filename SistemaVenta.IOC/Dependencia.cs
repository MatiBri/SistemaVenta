using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Referencias
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; //Inyección de dependencia
using SistemaVenta.DAL.DBContext; //La carpeta donde está el contexto
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.BLL.Implementacion;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        //Método de extensión: (this IServiceCollection services, IConfiguration configuration)
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<DbventaContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("CadenaSQL")); //CadenaSQL Es el nombre de la cadena de conexión que viene de appsettings.json
            });

            //De esta forma usamos inyección de Dependencia para nuestro GenericRepository y Venta Repository
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //Decimos que sea Transient para que varíe sus valores, esto es porque como es generico no sé con que clase o entidad se puede trabajar
            services.AddScoped<IVentaRepository, VentaRepository>();


            //Agrego la dependencia del envío de correo
            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IFireBaseService, FireBaseService>();
        }
    }
}

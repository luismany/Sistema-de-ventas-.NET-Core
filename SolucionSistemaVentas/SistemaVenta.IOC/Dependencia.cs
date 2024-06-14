using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void InyectarDepependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbventaContext>(Options =>
            {
                Options.UseSqlServer(configuration.GetConnectionString("CadenaSQL")); 
            }); 
        }
    }
}

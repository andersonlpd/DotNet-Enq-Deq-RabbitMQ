using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MySQLApp.Services;

namespace MySQLApp.Config
{
    public static class MySQLConfig
    {
        public static void AddMySQLService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddScoped<MySQLService>();
        }
    }
}
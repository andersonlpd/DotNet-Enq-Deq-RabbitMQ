using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDBApp.Services;

namespace MongoDBApp.Config
{
    public static class MongoDBConfig
    {
        public static void AddMongoDBService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddScoped<MongoDBService>();
        }
    }
}
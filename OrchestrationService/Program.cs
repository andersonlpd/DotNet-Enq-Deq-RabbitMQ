using OrchService.Config;
using OrchService.PublishToMongoDB;
using OrchService.PublishToMySQL;
using OrchService.Queue;


namespace MySQLController
{

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Adding Logging config
            services.AddScoped<LoggingConfig>();
            services.AddScoped<RabbitMQOrchestrator>();
            services.AddScoped<MySQLPublisher>();
            services.AddScoped<MongoDBPublisher>();

            // Registrando acesso ao IConfiguration
            services.AddSingleton(Configuration);

            //Adding Controllers
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Inicializando o RabbitMQOrchestrator dentro de um escopo de serviço
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var orchestrator = scope.ServiceProvider.GetService<RabbitMQOrchestrator>();
                orchestrator.StartListening();
            }
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
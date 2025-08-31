using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeUrokDBManager.Core.Interfaces.Reposoitories;
using NeUrokDBManager.Infrastructure;
using NeUrokDBManager.Infrastructure.Reposoitories;
using NeUrokDBManager.WPF.Views.Pages;

namespace NeUrokDBManager.WPF
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath);

                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .Build();

            var app = host.Services.GetService<App>();

            app?.Run();
        }

        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddSingleton(configuration);
            //services.AddMediatR(cfg =>
            //    cfg.RegisterServicesFromAssembly(typeof(RegistrateCommand).Assembly));

    //        services.AddDbContext<ApplicationDbContext>(options =>
    //            options.UseMySql(configuration.GetConnectionString("Main"),
    //new MySqlServerVersion(new Version(8, 0, 36))));

            services.AddScoped<IClientRepository, ClientRepository>();

            services.AddSingleton<App>();
            services.AddSingleton<MainWindow>();

            services.AddTransient<MenuPage>();
        }

    }
}

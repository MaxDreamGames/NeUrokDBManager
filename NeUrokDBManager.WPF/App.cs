using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NeUrokDBManager.WPF
{
    public class App : System.Windows.Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
               .Build();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            Program.ConfigureServices(context.Configuration, services);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = new MainWindow();
            //var navService = _host.Services.GetRequiredService<INavigationService>();

            //navService.NavigateTo<MenuPage>();

            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }
            base.OnExit(e);
        }
    }
}

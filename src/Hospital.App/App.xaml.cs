using System.Windows;
using Hospital.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.App;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var serviceProvider = new ServiceCollection()
                .AddHospitalAppServices()
                .BuildServiceProvider();

            var startup = serviceProvider.GetRequiredService<IStartupService>();
            if (!startup.Run())
            {
                Shutdown();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"应用程序启动失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}

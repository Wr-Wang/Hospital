using System.Windows;
using Hospital.App.Services;
using Hospital.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddSingleton<IAppContext, ApplicationContext>();
        services.AddSingleton<INavigationService, NavigationService>();
        var serviceProvider = services.BuildServiceProvider();

        var navigation = serviceProvider.GetRequiredService<INavigationService>();
        var appContext = serviceProvider.GetRequiredService<IAppContext>();

        var mainWindow = new MainWindow();
        mainWindow.AttachNavigation(navigation);
        mainWindow.DataContext = new MainWindowViewModel(navigation, appContext);
        mainWindow.Show();

        navigation.Navigate("shell.home");
    }
}

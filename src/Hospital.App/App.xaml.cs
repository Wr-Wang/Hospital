using System.Windows;
using Hospital.App.Services;
using Hospital.App.ViewModels;
using Hospital.App.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var services = new ServiceCollection();
        services.AddSingleton<IAppContext, ApplicationContext>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        var serviceProvider = services.BuildServiceProvider();

        var authService = serviceProvider.GetRequiredService<IAuthenticationService>();
        var appContext = serviceProvider.GetRequiredService<IAppContext>();
        var navigation = serviceProvider.GetRequiredService<INavigationService>();

        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.AttachNavigation(navigation);
        mainWindow.DataContext = new MainWindowViewModel(navigation, appContext);

        var loginWindow = new LoginWindow
        {
            DataContext = new LoginViewModel(authService, appContext)
        };

        var loginResult = loginWindow.ShowDialog();
        if (loginResult != true)
        {
            Shutdown();
            return;
        }

        mainWindow.Show();
        navigation.Navigate("shell.home");
        ShutdownMode = ShutdownMode.OnMainWindowClose;
    }
}

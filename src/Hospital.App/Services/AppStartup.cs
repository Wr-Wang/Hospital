using System.Windows;
using Hospital.App.Views;

namespace Hospital.App.Services;

public sealed class AppStartup : IStartupService
{
    private readonly INavigationService _navigation;
    private readonly MainWindow _mainWindow;
    private readonly LoginWindow _loginWindow;

    public AppStartup(INavigationService navigation, MainWindow mainWindow, LoginWindow loginWindow)
    {
        _navigation = navigation;
        _mainWindow = mainWindow;
        _loginWindow = loginWindow;
    }

    public bool Run()
    {
        System.Windows.Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // 先注册 MainWindow，使应用有正确的主窗口引用
        System.Windows.Application.Current.MainWindow = _mainWindow;

        var loginResult = _loginWindow.ShowDialog();
        if (loginResult != true)
        {
            return false;
        }

        try
        {
            _mainWindow.Show();
            _navigation.Navigate("shell.home");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"启动主窗口失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        System.Windows.Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        return true;
    }
}

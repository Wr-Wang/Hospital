using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using Hospital.App.Services;
using Hospital.App.ViewModels;
using Hospital.App.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.App;

public partial class App : System.Windows.Application
{
    private IServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        // 统一日期显示格式：App 内所有日期控件（DatePicker 等）一律显示为 yyyy/MM/dd。
        // WPF DatePicker 文本框用当前线程文化的 ShortDatePattern 格式化选中日期，
        // 因此克隆 zh-CN 文化并覆盖短日期模式即可全局生效。
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("zh-CN").Clone();
        culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        base.OnStartup(e);

        // 全局未处理异常处理
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;

        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            _serviceProvider = new ServiceCollection()
                .AddHospitalAppServices(configuration)
                .BuildServiceProvider();

            if (!StartLoginFlow())
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

    private static void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"发生未处理的异常: {e.Exception.Message}\n\n应用程序将继续运行。", "异常",
            MessageBoxButton.OK, MessageBoxImage.Warning);
        e.Handled = true;
    }

    private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            MessageBox.Show($"严重错误: {ex.Message}\n\n应用程序将关闭。", "严重错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool StartLoginFlow()
    {
        // 防止登录弹窗关闭时（当前唯一窗口）导致应用退出
        Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();

        if (loginWindow.DataContext is LoginViewModel loginVm)
        {
            loginVm.LoginSucceeded += OnLoginSucceeded;
            loginVm.RequestClose += OnLoginRequestClose;
        }

        var result = loginWindow.ShowDialog();

        if (loginWindow.DataContext is LoginViewModel vm)
        {
            vm.LoginSucceeded -= OnLoginSucceeded;
            vm.RequestClose -= OnLoginRequestClose;
        }

        return result == true;
    }

    private void OnLoginSucceeded(object? sender, EventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var navigation = _serviceProvider.GetRequiredService<INavigationService>();
        var appContext = _serviceProvider.GetRequiredService<IAppContext>();

        // 获取 MainWindow 实际绑定的 ViewModel 实例
        if (mainWindow.DataContext is MainWindowViewModel mainVm)
        {
            mainVm.LogoutRequested += OnLogoutRequested;
            // MainWindow 是单例，退出后重新登录会复用同一实例；每次登录按当前用户权限重建菜单
            mainVm.RefreshMenuPermissions();
        }

        // 不要在 LoginSucceeded 事件中关闭登录窗口——
        // LoginWindow.OnLoginSucceeded 负责设置 DialogResult 来正确关闭模态窗口。
        // 在 .NET 8 WPF 中对模态窗口直接调用 Close() 而未设置 DialogResult
        // 会抛出 InvalidOperationException。

        Current.MainWindow = mainWindow;
        mainWindow.Show();

        // 恢复关闭模式，使主窗口关闭时应用正常退出
        Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
    }

    private static void OnLoginRequestClose(object? sender, EventArgs e)
    {
        // 登录窗口被关闭（取消或 X 按钮）——无需额外操作
    }

    private void OnLogoutRequested(object? sender, EventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var appContext = _serviceProvider.GetRequiredService<IAppContext>();

        // 清除认证状态
        appContext.AccessToken = null;
        appContext.CurrentUserDisplayName = null;
        appContext.CampusName = null;
        appContext.Roles = null;

        if (mainWindow.DataContext is MainWindowViewModel mainVm)
        {
            mainVm.LogoutRequested -= OnLogoutRequested;
        }

        mainWindow.Hide();

        if (!StartLoginFlow())
        {
            Shutdown();
        }
    }
}

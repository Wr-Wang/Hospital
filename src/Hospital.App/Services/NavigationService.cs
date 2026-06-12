using System;
using System.Windows;
using System.Windows.Controls;
using Hospital.App.ViewModels;
using Hospital.App.Views;
using Hospital.App.Views.Placeholder;
using Hospital.Application.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.App.Services;

public sealed class NavigationService : INavigationService
{
    private readonly Dictionary<string, Func<UIElement>> _routes = new(StringComparer.OrdinalIgnoreCase);
    private readonly IServiceProvider _services;
    private ContentControl? _host;
    private string? _pendingRoute;

    public NavigationService(IServiceProvider services)
    {
        _services = services;

        // Dashboard
        Register(RouteKeys.Home, () => CreateView<HomePlaceholderView, HomePlaceholderViewModel>());

        // M2 患者
        Register(RouteKeys.PatientRegister, () => CreateView<PatientRegisterView, PatientRegisterViewModel>());
        Register(RouteKeys.PatientSearch, () => CreateView<PatientSearchView, PatientSearchViewModel>());
        Register(RouteKeys.Patient360, () => CreateView<Patient360View, Patient360ViewModel>());

        // M1 主数据
        Register(RouteKeys.Campus, () => CreateView<CampusView, CampusViewModel>());
        Register(RouteKeys.Department, () => CreateView<DepartmentView, DepartmentViewModel>());
        Register(RouteKeys.Staff, () => CreateView<StaffView, StaffListViewModel>());
        Register(RouteKeys.Dictionary, () => CreateView<DictionaryView, DictionaryViewModel>());

        // M3 挂号
        Register(RouteKeys.Schedule, () => CreateView<ScheduleView, ScheduleViewModel>());
        Register(RouteKeys.RegisterWorkbench, () => CreateView<RegisterWorkbenchView, RegisterWorkbenchViewModel>());

        // M5 门诊
        Register(RouteKeys.Encounter, () => CreateView<EncounterWorkbenchView, EncounterWorkbenchViewModel>());

        // M6 发药
        Register(RouteKeys.Dispense, () => CreateView<DispenseWorkbenchView, DispenseWorkbenchViewModel>());

        // M11 收费
        Register(RouteKeys.Cashier, () => CreateView<CashierWorkbenchView, CashierWorkbenchViewModel>());

        // M13 系统
        Register(RouteKeys.UserRole, () => CreateView<UserRoleView, UserRoleViewModel>());
    }

    public void Attach(ContentControl host)
    {
        _host = host;
        if (_pendingRoute is not null)
        {
            Navigate(_pendingRoute);
            _pendingRoute = null;
        }
    }

    public void Navigate(string routeKey)
    {
        if (_host is null)
        {
            _pendingRoute = routeKey;
            return;
        }

        if (!_routes.TryGetValue(routeKey, out var factory))
        {
            _host.Content = new TextBlock
            {
                Text = $"未注册路由: {routeKey}",
                Margin = new Thickness(16),
                FontSize = 16
            };
            return;
        }

        _host.Content = factory();
    }

    public void Register(string routeKey, Func<UIElement> factory) => _routes[routeKey] = factory;

    private UIElement CreateView<TView, TViewModel>()
        where TView : UserControl
        where TViewModel : class
    {
        var view = _services.GetRequiredService<TView>();
        // 如果 View 的构造函数已注入 ViewModel 并设置了 DataContext，则不再覆盖；
        // 否则（如参数构造的 View）才从 DI 获取 ViewModel。
        if (view.DataContext is null)
        {
            var vm = _services.GetRequiredService<TViewModel>();
            view.DataContext = vm;
        }
        return view;
    }
}

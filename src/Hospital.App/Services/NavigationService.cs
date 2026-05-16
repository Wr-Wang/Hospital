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
        Register(RouteKeys.Home, () => new HomePlaceholderView());

        // M2 患者
        Register(RouteKeys.PatientRegister, () => CreateView<PatientRegisterView, PatientRegisterViewModel>());
        Register(RouteKeys.PatientSearch, () => CreateView<PatientSearchView, PatientSearchViewModel>());

        // M1 主数据
        Register(RouteKeys.Campus, () => new PagePlaceholderView
        {
            Title = "院区管理",
            Description = "维护集团下属各院区基础信息，支持多院区统一管理"
        });
        Register(RouteKeys.Department, () => new PagePlaceholderView
        {
            Title = "科室维护",
            Description = "管理院区科室树结构，支持科室新增、编辑、停用"
        });
        Register(RouteKeys.Staff, () => new PagePlaceholderView
        {
            Title = "人员档案",
            Description = "维护医护人员档案与执业信息，资质到期自动预警"
        });
        Register(RouteKeys.Dictionary, () => new PagePlaceholderView
        {
            Title = "字典管理",
            Description = "维护 ICD 诊断编码、收费项目、药品目录等系统字典"
        });

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
        Register(RouteKeys.UserRole, () => new PagePlaceholderView
        {
            Title = "用户与角色",
            Description = "管理用户账号、角色权限配置、审计日志查询"
        });
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
        var vm = _services.GetRequiredService<TViewModel>();
        view.DataContext = vm;
        return view;
    }
}

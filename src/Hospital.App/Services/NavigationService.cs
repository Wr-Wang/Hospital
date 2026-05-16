using System;
using System.Windows;
using System.Windows.Controls;
using Hospital.App.Views.Placeholder;

namespace Hospital.App.Services;

public sealed class NavigationService : INavigationService
{
    private readonly Dictionary<string, Func<UIElement>> _routes = new(StringComparer.OrdinalIgnoreCase);
    private ContentControl? _host;
    private string? _pendingRoute;

    public NavigationService()
    {
        // Dashboard
        Register("shell.home", () => new HomePlaceholderView());

        // M2 患者
        Register("pat.register", () => new PagePlaceholderView
        {
            Title = "患者建档",
            Description = "为患者建立档案，支持身份证、手机号等多方式录入，自动查重"
        });
        Register("pat.search", () => new PagePlaceholderView
        {
            Title = "患者检索",
            Description = "按姓名、身份证号、手机号等条件模糊搜索患者信息"
        });

        // M1 主数据
        Register("mdm.campus", () => new PagePlaceholderView
        {
            Title = "院区管理",
            Description = "维护集团下属各院区基础信息，支持多院区统一管理"
        });
        Register("mdm.dept", () => new PagePlaceholderView
        {
            Title = "科室维护",
            Description = "管理院区科室树结构，支持科室新增、编辑、停用"
        });
        Register("mdm.staff", () => new PagePlaceholderView
        {
            Title = "人员档案",
            Description = "维护医护人员档案与执业信息，资质到期自动预警"
        });
        Register("mdm.dict", () => new PagePlaceholderView
        {
            Title = "字典管理",
            Description = "维护 ICD 诊断编码、收费项目、药品目录等系统字典"
        });

        // M3 挂号
        Register("opd.schedule", () => new PagePlaceholderView
        {
            Title = "排班号表",
            Description = "管理医生排班与号源发布，支持停诊替诊操作"
        });
        Register("opd.register", () => new PagePlaceholderView
        {
            Title = "挂号工作台",
            Description = "窗口快速挂号、退号改签，支持按科室和医生筛选号源"
        });

        // M5 门诊
        Register("opd.encounter", () => new PagePlaceholderView
        {
            Title = "门诊医生站",
            Description = "结构化病历书写、诊断开立、处方开立、检验检查申请"
        });

        // M6 发药
        Register("pha.dispense", () => new PagePlaceholderView
        {
            Title = "发药工作台",
            Description = "门诊处方审核、发药、退药，管控药双人核对"
        });

        // M11 收费
        Register("fin.cash", () => new PagePlaceholderView
        {
            Title = "收费工作台",
            Description = "门诊费用收取、退费处理、发票打印"
        });

        // M13 系统
        Register("sys.userrole", () => new PagePlaceholderView
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
}

using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.App.Services;

namespace Hospital.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigation;

    public MainWindowViewModel(INavigationService navigation, IAppContext appContext)
    {
        _navigation = navigation;
        AppContext = appContext;

        MenuItems.Add(new NavMenuItem("首 页", "shell.home", "🏠"));
        MenuItems.Add(new NavMenuItem("患者建档", "pat.register", "👤"));
        MenuItems.Add(new NavMenuItem("患者检索", "pat.search", "🔍"));
        MenuItems.Add(new NavMenuItem("排班号表", "opd.schedule", "📅"));
        MenuItems.Add(new NavMenuItem("挂号工作台", "opd.register", "📋"));
        MenuItems.Add(new NavMenuItem("门诊医生站", "opd.encounter", "🩺"));
        MenuItems.Add(new NavMenuItem("发药工作台", "pha.dispense", "💊"));
        MenuItems.Add(new NavMenuItem("收费工作台", "fin.cash", "💰"));

        MenuItems.Add(new NavMenuItem("", "", "")); // separator

        MenuItems.Add(new NavMenuItem("院区管理", "mdm.campus", "🏢"));
        MenuItems.Add(new NavMenuItem("科室维护", "mdm.dept", "🏛️"));
        MenuItems.Add(new NavMenuItem("人员档案", "mdm.staff", "👥"));
        MenuItems.Add(new NavMenuItem("字典管理", "mdm.dict", "📖"));
        MenuItems.Add(new NavMenuItem("用户与角色", "sys.userrole", "🔐"));

        // Auto-select first item
        SelectedMenuItem = MenuItems[0];
    }

    public IAppContext AppContext { get; }

    public ObservableCollection<NavMenuItem> MenuItems { get; } = new();

    [ObservableProperty]
    private NavMenuItem? selectedMenuItem;

    public event EventHandler? LogoutRequested;

    partial void OnSelectedMenuItemChanged(NavMenuItem? value)
    {
        if (value is null || string.IsNullOrEmpty(value.RouteKey))
            return;

        foreach (var item in MenuItems)
        {
            item.IsSelected = item == value;
        }
        _navigation.Navigate(value.RouteKey);
    }

    [RelayCommand]
    private void Navigate(NavMenuItem? item)
    {
        if (item is null) return;
        SelectedMenuItem = item;
    }

    [RelayCommand]
    private void Logout()
    {
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }
}

public sealed partial class NavMenuItem : ObservableObject
{
    public NavMenuItem(string title, string routeKey, string icon)
    {
        Title = title;
        RouteKey = routeKey;
        Icon = icon;
    }

    public string Title { get; }
    public string RouteKey { get; }
    public string Icon { get; }

    [ObservableProperty]
    private bool isSelected;
}

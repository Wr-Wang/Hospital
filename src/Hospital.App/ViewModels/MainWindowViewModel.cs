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
        MenuItems.Add(new NavMenuItem("院区管理", "mdm.campus", "🏢"));
        MenuItems.Add(new NavMenuItem("挂号工作台", "opd.register", "📋"));

        // Auto-select first item
        SelectedMenuItem = MenuItems[0];
    }

    public IAppContext AppContext { get; }

    public ObservableCollection<NavMenuItem> MenuItems { get; } = new();

    [ObservableProperty]
    private NavMenuItem? selectedMenuItem;

    partial void OnSelectedMenuItemChanged(NavMenuItem? value)
    {
        foreach (var item in MenuItems)
        {
            item.IsSelected = item == value;
        }
        if (value is not null)
        {
            _navigation.Navigate(value.RouteKey);
        }
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
        System.Windows.Application.Current.Shutdown();
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

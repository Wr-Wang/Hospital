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
        MenuItems.Add(new NavMenuItem("首页", "shell.home"));
        MenuItems.Add(new NavMenuItem("院区管理", "mdm.campus"));
        MenuItems.Add(new NavMenuItem("挂号工作台", "opd.register"));
    }

    public IAppContext AppContext { get; }

    public ObservableCollection<NavMenuItem> MenuItems { get; } = new();

    [RelayCommand]
    private void Navigate(NavMenuItem? item)
    {
        if (item is null) return;
        _navigation.Navigate(item.RouteKey);
    }
}

public sealed record NavMenuItem(string Title, string RouteKey);

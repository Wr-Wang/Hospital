using System.Windows;
using Hospital.App.Services;
using Hospital.App.ViewModels;

namespace Hospital.App;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel, INavigationService navigation)
    {
        InitializeComponent();
        DataContext = viewModel;
        navigation.Attach(ShellContentHost);
    }
}

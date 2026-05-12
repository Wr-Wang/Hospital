using System.Windows;
using Hospital.App.Services;

namespace Hospital.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void AttachNavigation(INavigationService navigation)
    {
        navigation.Attach(ShellContentHost);
    }
}

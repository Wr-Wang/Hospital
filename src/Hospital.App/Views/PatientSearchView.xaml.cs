using System.Windows;
using System.Windows.Controls;
using Hospital.App.ViewModels;

namespace Hospital.App.Views;

public partial class PatientSearchView : UserControl
{
    public PatientSearchView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is PatientSearchViewModel vm)
            await vm.InitializeAsync();
    }
}

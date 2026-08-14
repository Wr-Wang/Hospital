using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hospital.App.Services;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.App.ViewModels;

namespace Hospital.App.Views;

public partial class PatientSearchView : UserControl
{
    private readonly INavigationService _navigation;

    public PatientSearchView(INavigationService navigation)
    {
        InitializeComponent();
        _navigation = navigation;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is PatientSearchViewModel vm)
        {
            // 防重复订阅：同一实例可能多次 Loaded
            vm.PatientSelected -= OnPatientSelected;
            vm.PatientSelected += OnPatientSelected;
            await vm.InitializeAsync();
        }
    }

    private void OnPatientItemClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PatientDto patient }
            && DataContext is PatientSearchViewModel vm)
        {
            vm.SelectPatientCommand.Execute(patient);
        }
    }

    private void OnPatientSelected(PatientDto patient)
    {
        _navigation.Navigate(RouteKeys.Patient360, patient.Id);
    }
}

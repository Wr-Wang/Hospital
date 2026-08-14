using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hospital.Application.DTOs;
using Hospital.App.ViewModels;

namespace Hospital.App.Views;

public partial class DictionaryView : UserControl
{
    public DictionaryView(DictionaryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeAsync();
    }

    private void OnTypeClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: DictionaryTypeDto type }
            && DataContext is DictionaryViewModel vm)
        {
            vm.SelectTypeCommand.Execute(type);
        }
    }
}

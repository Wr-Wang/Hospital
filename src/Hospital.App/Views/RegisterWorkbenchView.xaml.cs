using System.Windows.Controls;
using Hospital.App.ViewModels;

namespace Hospital.App.Views;

public partial class RegisterWorkbenchView : UserControl
{
    public RegisterWorkbenchView(RegisterWorkbenchViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeAsync();
    }
}

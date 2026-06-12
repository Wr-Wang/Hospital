using System.Windows.Controls;
using Hospital.App.ViewModels;

namespace Hospital.App.Views;

public partial class CashierWorkbenchView : UserControl
{
    public CashierWorkbenchView(CashierWorkbenchViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeAsync();
    }
}

using System.Windows.Controls;
using Hospital.App.ViewModels;

namespace Hospital.App.Views;

public partial class UserRoleView : UserControl
{
    public UserRoleView(UserRoleViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeAsync();
    }
}

using System.Windows.Controls;
using Hospital.App.ViewModels;

namespace Hospital.App.Views.Placeholder;

public partial class HomePlaceholderView : UserControl
{
    public HomePlaceholderView(HomePlaceholderViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeAsync();
    }
}

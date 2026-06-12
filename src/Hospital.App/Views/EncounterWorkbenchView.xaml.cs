using System.Windows.Controls;
using Hospital.App.ViewModels;

namespace Hospital.App.Views;

public partial class EncounterWorkbenchView : UserControl
{
    public EncounterWorkbenchView(EncounterWorkbenchViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeAsync();
    }
}

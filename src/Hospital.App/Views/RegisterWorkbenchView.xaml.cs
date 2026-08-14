using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hospital.Application.DTOs;
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

    /// <summary>点击排班下的时段，设置 SelectedSlotName</summary>
    private void OnSlotClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ScheduleSlotDto slot }
            && DataContext is RegisterWorkbenchViewModel vm)
        {
            vm.SelectSlotNameCommand.Execute(slot.SlotName);
        }
    }
}

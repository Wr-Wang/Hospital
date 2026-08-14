using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

    /// <summary>点击权限项，切换该权限的选中状态（嵌套 ItemsControl 内，命令绑定无法用 RelativeSource 定位）</summary>
    private void OnPermissionClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PermissionOption option }
            && DataContext is UserRoleViewModel vm)
        {
            vm.ToggleRolePermissionCommand.Execute(option.Value);
        }
    }
}

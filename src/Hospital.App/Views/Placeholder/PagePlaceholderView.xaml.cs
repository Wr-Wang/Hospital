using System.Windows;
using System.Windows.Controls;

namespace Hospital.App.Views.Placeholder;

public partial class PagePlaceholderView : UserControl
{
    public PagePlaceholderView()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => ModuleTitle.Text;
        set => ModuleTitle.Text = value;
    }

    public string Description
    {
        get => ModuleDescription.Text;
        set => ModuleDescription.Text = value;
    }
}

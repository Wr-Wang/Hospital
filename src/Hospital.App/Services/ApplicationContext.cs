using CommunityToolkit.Mvvm.ComponentModel;

namespace Hospital.App.Services;

public sealed partial class ApplicationContext : ObservableObject, IAppContext
{
    [ObservableProperty]
    private string? currentUserDisplayName = "演示用户";

    [ObservableProperty]
    private string? campusName = "演示院区";

    [ObservableProperty]
    private string? accessToken;

    [ObservableProperty]
    private string[]? roles;
}

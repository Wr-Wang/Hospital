namespace Hospital.App.Services;

public sealed class ApplicationContext : IAppContext
{
    public string? CurrentUserDisplayName { get; set; } = "演示用户";
    public string? CampusName { get; set; } = "演示院区";
}

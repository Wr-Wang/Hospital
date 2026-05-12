namespace Hospital.App.Services;

public interface IAppContext
{
    string? CurrentUserDisplayName { get; set; }
    string? CampusName { get; set; }
}

using System.ComponentModel;

namespace Hospital.App.Services;

public interface IAppContext : INotifyPropertyChanged
{
    string? CurrentUserDisplayName { get; set; }
    string? CampusName { get; set; }
}

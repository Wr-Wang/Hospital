using System.Windows.Controls;

namespace Hospital.App.Services;

public interface INavigationService
{
    void Attach(ContentControl host);

    void Navigate(string routeKey);
}

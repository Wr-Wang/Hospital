using System.Windows.Controls;

namespace Hospital.App.Services;

public interface INavigationService
{
    void Attach(ContentControl host);

    void Navigate(string routeKey);

    /// <summary>带参数导航，将参数传给该路由的页面工厂（如患者 360 的患者 ID）</summary>
    void Navigate(string routeKey, object? parameter);
}

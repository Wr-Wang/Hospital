using System.Windows;
using System.Windows.Controls;
using Hospital.App.Views.Placeholder;

namespace Hospital.App.Services;

public sealed class NavigationService : INavigationService
{
    private readonly Dictionary<string, Func<UIElement>> _routes = new(StringComparer.OrdinalIgnoreCase);
    private ContentControl? _host;

    public NavigationService()
    {
        Register("shell.home", () => new HomePlaceholderView());
        Register("mdm.campus", () => new HomePlaceholderView());
        Register("opd.register", () => new HomePlaceholderView());
    }

    public void Attach(ContentControl host) => _host = host;

    public void Navigate(string routeKey)
    {
        if (_host is null)
            throw new InvalidOperationException("Navigation host is not attached.");

        if (!_routes.TryGetValue(routeKey, out var factory))
        {
            _host.Content = new TextBlock
            {
                Text = $"未注册路由: {routeKey}",
                Margin = new Thickness(16),
                FontSize = 16
            };
            return;
        }

        _host.Content = factory();
    }

    public void Register(string routeKey, Func<UIElement> factory) => _routes[routeKey] = factory;
}

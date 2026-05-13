using System.Windows;
using System.Windows.Controls;
using Hospital.App.Views.Placeholder;

namespace Hospital.App.Services;

public sealed class NavigationService : INavigationService
{
    private readonly Dictionary<string, Func<UIElement>> _routes = new(StringComparer.OrdinalIgnoreCase);
    private ContentControl? _host;
    private string? _pendingRoute;

    public NavigationService()
    {
        Register("shell.home", () => new HomePlaceholderView());
        Register("mdm.campus", () => new HomePlaceholderView());
        Register("opd.register", () => new HomePlaceholderView());
    }

    public void Attach(ContentControl host)
    {
        _host = host;
        if (_pendingRoute is not null)
        {
            Navigate(_pendingRoute);
            _pendingRoute = null;
        }
    }

    public void Navigate(string routeKey)
    {
        if (_host is null)
        {
            // Defer until host is attached
            _pendingRoute = routeKey;
            return;
        }

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

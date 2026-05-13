using System;
using Hospital.App.Services;
using Hospital.App.ViewModels;
using Hospital.App.Views;
using Hospital.Application.Services;
using Hospital.Infrastructure.ExternalServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.App;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHospitalAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppContext, ApplicationContext>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5075/api/");
        });

        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IAuthenticationApplicationService, AuthenticationApplicationService>();

        services.AddSingleton<IStartupService, AppStartup>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        return services;
    }
}

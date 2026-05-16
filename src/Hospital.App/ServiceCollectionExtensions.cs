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
        })
        .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddTransient<AuthDelegatingHandler>();

        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IAuthenticationApplicationService, AuthenticationApplicationService>();

        services.AddTransient<IPatientApplicationService, PatientService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<PatientRegisterViewModel>();
        services.AddTransient<PatientSearchViewModel>();
        services.AddTransient<Patient360ViewModel>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        // Views
        services.AddTransient<Views.PatientRegisterView>();
        services.AddTransient<Views.PatientSearchView>();
        services.AddTransient<Views.Patient360View>();

        return services;
    }
}

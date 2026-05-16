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

        // Auth services
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IAuthenticationApplicationService, AuthenticationApplicationService>();

        // Patient services
        services.AddTransient<IPatientApplicationService, PatientService>();

        // MDM services (WPF HTTP implementations)
        services.AddTransient<IDepartmentApplicationService, DepartmentService>();
        services.AddTransient<IStaffApplicationService, StaffService>();

        // Registration services (Schedule + Registration)
        services.AddSingleton<IScheduleApplicationService, ScheduleService>();
        services.AddSingleton<IRegistrationApplicationService, RegistrationService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<PatientRegisterViewModel>();
        services.AddTransient<PatientSearchViewModel>();
        services.AddTransient<Patient360ViewModel>();
        services.AddTransient<ScheduleViewModel>();
        services.AddTransient<RegisterWorkbenchViewModel>();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddSingleton<MainWindow>();

        // Views
        services.AddTransient<PatientRegisterView>();
        services.AddTransient<PatientSearchView>();
        services.AddTransient<Patient360View>();
        services.AddTransient<ScheduleView>();
        services.AddTransient<RegisterWorkbenchView>();

        return services;
    }
}

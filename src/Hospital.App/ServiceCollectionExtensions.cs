using System;
using Hospital.App.Services;
using Hospital.App.ViewModels;
using Hospital.App.Views;
using Hospital.App.Views.Placeholder;
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
            client.BaseAddress = new Uri("http://192.168.31.20:8080/api/");
        })
        .AddHttpMessageHandler<AuthDelegatingHandler>();

        services.AddTransient<AuthDelegatingHandler>();

        // Auth services
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IAuthenticationApplicationService, AuthenticationApplicationService>();

        // Patient services
        services.AddTransient<IPatientApplicationService, PatientService>();

        // MDM services (WPF HTTP implementations)
        services.AddTransient<ICampusApplicationService, CampusService>();
        services.AddTransient<IDepartmentApplicationService, DepartmentService>();
        services.AddTransient<IStaffApplicationService, StaffService>();
        services.AddTransient<IDictionaryApplicationService, DictionaryService>();

        // Registration services (Schedule + Registration)
        services.AddSingleton<IScheduleApplicationService, ScheduleService>();
        services.AddSingleton<IRegistrationApplicationService, RegistrationService>();

        // Encounter services (Outpatient Doctor Station)
        services.AddSingleton<IEncounterApplicationService, EncounterService>();
        services.AddSingleton<IMedicalRecordApplicationService, MedicalRecordService>();
        services.AddSingleton<IDiagnosisApplicationService, DiagnosisService>();
        services.AddSingleton<IPrescriptionApplicationService, PrescriptionService>();
        services.AddSingleton<ILabOrderApplicationService, LabOrderService>();

        // Cashier and Dispense services
        services.AddSingleton<ICashierApplicationService, CashierService>();
        services.AddSingleton<IDispenseApplicationService, DispenseService>();

        // UserRole services
        services.AddSingleton<IUserRoleApplicationService, UserRoleService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<PatientRegisterViewModel>();
        services.AddTransient<PatientSearchViewModel>();
        services.AddTransient<Patient360ViewModel>();
        services.AddTransient<ScheduleViewModel>();
        services.AddTransient<RegisterWorkbenchViewModel>();
        services.AddTransient<EncounterWorkbenchViewModel>();
        services.AddTransient<CashierWorkbenchViewModel>();
        services.AddTransient<DispenseWorkbenchViewModel>();
        services.AddTransient<UserRoleViewModel>();
        services.AddTransient<CampusViewModel>();
        services.AddTransient<DepartmentViewModel>();
        services.AddTransient<StaffListViewModel>();
        services.AddTransient<DictionaryViewModel>();
        services.AddTransient<HomePlaceholderViewModel>();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddSingleton<MainWindow>();

        // Views
        services.AddTransient<PatientRegisterView>();
        services.AddTransient<PatientSearchView>();
        services.AddTransient<Patient360View>();
        services.AddTransient<ScheduleView>();
        services.AddTransient<RegisterWorkbenchView>();
        services.AddTransient<EncounterWorkbenchView>();
        services.AddTransient<CashierWorkbenchView>();
        services.AddTransient<DispenseWorkbenchView>();
        services.AddTransient<UserRoleView>();
        services.AddTransient<CampusView>();
        services.AddTransient<DepartmentView>();
        services.AddTransient<StaffView>();
        services.AddTransient<DictionaryView>();
        services.AddTransient<HomePlaceholderView>();

        return services;
    }
}

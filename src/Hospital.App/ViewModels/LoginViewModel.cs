using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.App.Services;

namespace Hospital.App.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IAppContext _appContext;

    public event EventHandler? LoginSucceeded;
    public event EventHandler? RequestClose;

    public LoginViewModel(IAuthenticationService authenticationService, IAppContext appContext)
    {
        _authenticationService = authenticationService;
        _appContext = appContext;
    }

    [ObservableProperty]
    private string? userName;

    [ObservableProperty]
    private string? password;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            var result = await _authenticationService.AuthenticateAsync(UserName ?? string.Empty, Password ?? string.Empty);
            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            _appContext.CurrentUserDisplayName = result.DisplayName;
            _appContext.CampusName = result.CampusName;
            LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private bool CanLogin() => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }
}

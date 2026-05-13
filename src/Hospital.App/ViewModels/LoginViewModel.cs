using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.Application.Services;
using Hospital.App.Services;

namespace Hospital.App.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly IAuthenticationApplicationService _authenticationService;
    private readonly IAppContext _appContext;

    public event EventHandler? LoginSucceeded;
    public event EventHandler? RequestClose;

    public LoginViewModel(IAuthenticationApplicationService authenticationService, IAppContext appContext)
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
            var result = await _authenticationService.LoginAsync(UserName ?? string.Empty, Password ?? string.Empty);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            _appContext.CurrentUserDisplayName = result.UserInfo?.DisplayName;
            _appContext.CampusName = result.UserInfo?.CampusName;
            _appContext.AccessToken = result.Token;
            _appContext.Roles = result.UserInfo?.Roles;
            LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"无法连接到服务器: {ex.Message}";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "登录请求超时，请检查网络或服务器状态";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"登录失败: {ex.Message}";
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

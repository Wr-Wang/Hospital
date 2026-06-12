using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Hospital.App.Services;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>首页 ViewModel，展示统计概览</summary>
public sealed partial class HomePlaceholderViewModel : ObservableObject
{
    private readonly ICampusApplicationService _campusService;
    private readonly IAppContext _appContext;

    public HomePlaceholderViewModel(
        ICampusApplicationService campusService,
        IAppContext appContext)
    {
        _campusService = campusService;
        _appContext = appContext;
    }

    public IAppContext AppContext => _appContext;

    [ObservableProperty]
    private int campusCount;

    [ObservableProperty]
    private string todayRegistrationCount = "—";

    public async Task InitializeAsync()
    {
        try
        {
            var campuses = await _campusService.GetAllAsync();
            CampusCount = campuses.Count;
        }
        catch
        {
            CampusCount = 0;
        }
    }
}

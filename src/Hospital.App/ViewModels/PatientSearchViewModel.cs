using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.App.Constants;
using Hospital.App.Services;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>患者检索页面 ViewModel，初始化加载当前账号待就诊患者，支持模糊搜索和分页</summary>
public sealed partial class PatientSearchViewModel : ObservableObject
{
    private readonly IPatientApplicationService _patientService;
    private readonly IRegistrationApplicationService _registrationService;
    private readonly IAppContext _appContext;

    private List<PatientDto> _allQueuePatients = new();
    private bool _isQueueMode;

    public PatientSearchViewModel(
        IPatientApplicationService patientService,
        IRegistrationApplicationService registrationService,
        IAppContext appContext)
    {
        _patientService = patientService;
        _registrationService = registrationService;
        _appContext = appContext;
        Patients.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasResults));
    }

    // ===== 搜索条件与分页 =====

    [ObservableProperty]
    private string? keyword;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private int page = 1;

    [ObservableProperty]
    private int totalPages;

    /// <summary>是否有上一页</summary>
    public bool HasPrevious => Page > 1;

    /// <summary>是否有下一页</summary>
    public bool HasNext => Page < TotalPages;

    partial void OnPageChanged(int value)
    {
        OnPropertyChanged(nameof(HasPrevious));
        OnPropertyChanged(nameof(HasNext));
    }

    partial void OnTotalPagesChanged(int value)
    {
        OnPropertyChanged(nameof(HasNext));
    }

    partial void OnTotalCountChanged(int value)
    {
        OnPropertyChanged(nameof(HasPrevious));
    }

    /// <summary>搜索结果列表</summary>
    public ObservableCollection<PatientDto> Patients { get; } = new();

    /// <summary>是否有搜索结果</summary>
    public bool HasResults => Patients.Count > 0;

    /// <summary>选中患者事件，通知导航到 360 视图</summary>
    public event Action<PatientDto>? PatientSelected;

    // ===== 初始化 =====

    /// <summary>页面加载时调用，拉取当前账号今日待就诊患者</summary>
    public async Task InitializeAsync()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            var registrations = await _registrationService.GetByDoctorAsync(_appContext.CurrentUserId, today);

            // 只取待就诊（已挂号）患者
            var waiting = registrations.Where(r => r.Status == "已挂号").ToList();

            // 加载每个挂号对应的患者详情
            _allQueuePatients = new List<PatientDto>();
            foreach (var reg in waiting)
            {
                var patient = await _patientService.GetByIdAsync(reg.PatientId);
                if (patient is not null)
                    _allQueuePatients.Add(patient);
            }

            _isQueueMode = true;
            TotalCount = _allQueuePatients.Count;
            TotalPages = (int)Math.Ceiling((double)TotalCount / AppConstants.SearchPageSize);
            Page = 1;
            ApplyCurrentPage();
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"加载待就诊患者失败: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载数据出错: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>按当前分页显示数据</summary>
    private void ApplyCurrentPage()
    {
        Patients.Clear();
        var pageItems = _allQueuePatients
            .Skip((Page - 1) * AppConstants.SearchPageSize)
            .Take(AppConstants.SearchPageSize)
            .ToList();
        foreach (var item in pageItems)
            Patients.Add(item);
    }

    // ===== 搜索 =====

    /// <summary>执行搜索，按关键词从后端获取患者列表</summary>
    [RelayCommand]
    private async Task Search()
    {
        ErrorMessage = null;
        _isQueueMode = false;
        IsBusy = true;

        try
        {
            var result = await _patientService.SearchAsync(Keyword, Page, AppConstants.SearchPageSize);
            Patients.Clear();
            foreach (var p in result.Items)
                Patients.Add(p);

            TotalCount = result.TotalCount;
            TotalPages = (result.TotalCount + AppConstants.SearchPageSize - 1) / AppConstants.SearchPageSize;
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"搜索失败: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"搜索出错: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ===== 分页 =====

    /// <summary>翻到下一页</summary>
    [RelayCommand]
    private async Task NextPage()
    {
        if (!HasNext) return;
        Page++;
        if (_isQueueMode)
            ApplyCurrentPage();
        else
            await Search();
    }

    /// <summary>翻到上一页</summary>
    [RelayCommand]
    private async Task PreviousPage()
    {
        if (!HasPrevious) return;
        Page--;
        if (_isQueueMode)
            ApplyCurrentPage();
        else
            await Search();
    }

    // ===== 选择患者 =====

    /// <summary>选择某条搜索结果</summary>
    [RelayCommand]
    private void SelectPatient(PatientDto? patient)
    {
        if (patient is not null)
            PatientSelected?.Invoke(patient);
    }
}

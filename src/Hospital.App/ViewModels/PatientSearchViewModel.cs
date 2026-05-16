using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.App.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>患者检索页面 ViewModel，支持模糊搜索和分页</summary>
public sealed partial class PatientSearchViewModel : ObservableObject
{
    private readonly IPatientApplicationService _patientService;

    public PatientSearchViewModel(IPatientApplicationService patientService)
    {
        _patientService = patientService;
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

    /// <summary>选中患者事件，通知导航到 360 视图</summary>
    public event Action<PatientDto>? PatientSelected;

    /// <summary>执行搜索，按关键词从后端获取患者列表</summary>
    [RelayCommand]
    private async Task Search()
    {
        ErrorMessage = null;
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

    /// <summary>翻到下一页</summary>
    [RelayCommand]
    private async Task NextPage()
    {
        if (HasNext)
        {
            Page++;
            await Search();
        }
    }

    /// <summary>翻到上一页</summary>
    [RelayCommand]
    private async Task PreviousPage()
    {
        if (HasPrevious)
        {
            Page--;
            await Search();
        }
    }

    /// <summary>选择某条搜索结果</summary>
    [RelayCommand]
    private void SelectPatient(PatientDto? patient)
    {
        if (patient is not null)
            PatientSelected?.Invoke(patient);
    }
}

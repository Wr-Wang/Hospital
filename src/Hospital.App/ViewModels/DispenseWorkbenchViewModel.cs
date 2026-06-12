using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.App.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>发药工作台 ViewModel，支持已缴费处方查询、发药、退药</summary>
public sealed partial class DispenseWorkbenchViewModel : ObservableObject
{
    private readonly IDispenseApplicationService _dispenseService;
    private readonly IPatientApplicationService _patientService;

    public DispenseWorkbenchViewModel(
        IDispenseApplicationService dispenseService,
        IPatientApplicationService patientService)
    {
        _dispenseService = dispenseService;
        _patientService = patientService;
    }

    // ===== 页面状态 =====

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isSuccess;

    public string TodayDate => DateTime.Today.ToString("yyyy-MM-dd");

    // ===== 患者搜索 =====

    [ObservableProperty]
    private string patientKeyword = string.Empty;

    [ObservableProperty]
    private List<PatientDto> patientResults = new();

    [ObservableProperty]
    private string selectedPatientInfo = string.Empty;

    private long _selectedPatientId;

    // ===== 已缴费处方 =====

    [ObservableProperty]
    private List<PrescriptionDto> paidPrescriptions = new();

    [ObservableProperty]
    private bool hasPrescriptions;

    // ===== 初始化 =====

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    // ===== 患者搜索 =====

    [RelayCommand]
    private async Task SearchPatient()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            var result = await _patientService.SearchAsync(PatientKeyword, 1, AppConstants.SearchPageSize);
            PatientResults = result.Items;
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"搜索患者失败: {ex.Message}";
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

    [RelayCommand]
    private async Task SelectPatient(PatientDto? patient)
    {
        if (patient is null) return;
        _selectedPatientId = patient.Id;
        SelectedPatientInfo = $"{patient.Name}（病历号: {patient.PatientNo}）";
        IsSuccess = false;

        await LoadPrescriptionsAsync();
    }

    private async Task LoadPrescriptionsAsync()
    {
        try
        {
            PaidPrescriptions = await _dispenseService.GetPaidPrescriptionsAsync(_selectedPatientId);
            HasPrescriptions = PaidPrescriptions.Count > 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载已缴费处方失败: {ex.Message}";
            PaidPrescriptions = new();
            HasPrescriptions = false;
        }
    }

    // ===== 发药 =====

    [RelayCommand]
    private async Task Dispense(long id)
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            await _dispenseService.DispenseAsync(id);
            IsSuccess = true;
            await LoadPrescriptionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"发药失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ===== 退药 =====

    [RelayCommand]
    private async Task Return(long id)
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            await _dispenseService.ReturnAsync(id);
            IsSuccess = true;
            await LoadPrescriptionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"退药失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

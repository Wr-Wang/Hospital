using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.App.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>收费工作台 ViewModel，支持患者搜索、待收费项目查询、缴费</summary>
public sealed partial class CashierWorkbenchViewModel : ObservableObject
{
    private readonly ICashierApplicationService _cashierService;
    private readonly IPatientApplicationService _patientService;

    public CashierWorkbenchViewModel(
        ICashierApplicationService cashierService,
        IPatientApplicationService patientService)
    {
        _cashierService = cashierService;
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

    // ===== 待收费项目 =====

    [ObservableProperty]
    private List<ChargeItemDisplay> pendingItems = new();

    [ObservableProperty]
    private bool hasItems;

    [ObservableProperty]
    private int selectedCount;

    // ===== 初始化 =====

    public async Task InitializeAsync()
    {
        // 无初始化加载
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

        await LoadPendingItemsAsync();
    }

    private async Task LoadPendingItemsAsync()
    {
        try
        {
            var items = await _cashierService.GetPendingItemsAsync(_selectedPatientId);
            PendingItems = items.Select(i => new ChargeItemDisplay
            {
                Id = i.Id,
                ItemType = i.ItemType,
                ItemName = i.ItemName,
                Amount = i.Amount,
                Status = i.Status,
                CreateTime = i.CreateTime,
                IsSelected = false,
            }).ToList();
            HasItems = PendingItems.Count > 0;
            SelectedCount = 0;
        }
        catch
        {
            PendingItems = new();
            HasItems = false;
        }
    }

    // ===== 选择项目 =====

    [RelayCommand]
    private void ToggleItemSelection(ChargeItemDisplay? item)
    {
        if (item is null) return;
        item.IsSelected = !item.IsSelected;
        SelectedCount = PendingItems.Count(i => i.IsSelected);
    }

    // ===== 缴费 =====

    [RelayCommand]
    private async Task Pay()
    {
        ErrorMessage = null;
        IsSuccess = false;

        var selected = PendingItems.Where(i => i.IsSelected).ToList();
        if (selected.Count == 0)
        {
            ErrorMessage = "请选择要缴费的项目";
            return;
        }

        IsBusy = true;

        try
        {
            var items = selected.Select(i => new PayItemDto(i.Id, i.ItemType)).ToList();
            var dto = new PayRequestDto(items);
            await _cashierService.PayAsync(dto);
            IsSuccess = true;
            await LoadPendingItemsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"缴费失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

/// <summary>待收费项目展示模型</summary>
public sealed partial class ChargeItemDisplay : ObservableObject
{
    public long Id { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CreateTime { get; set; } = string.Empty;

    [ObservableProperty]
    private bool isSelected;
}

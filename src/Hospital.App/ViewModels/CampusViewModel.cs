using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>院区管理 ViewModel</summary>
public sealed partial class CampusViewModel : ObservableObject
{
    private readonly ICampusApplicationService _campusService;

    public CampusViewModel(ICampusApplicationService campusService)
    {
        _campusService = campusService;
    }

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private List<CampusDto> campuses = new();

    [ObservableProperty]
    private bool showForm;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private long editingId;

    [ObservableProperty]
    private string formCode = string.Empty;

    [ObservableProperty]
    private string formName = string.Empty;

    [ObservableProperty]
    private string? formAddress;

    [ObservableProperty]
    private string? formPhone;

    public async Task InitializeAsync()
    {
        await LoadCampusesAsync();
    }

    private async Task LoadCampusesAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Campuses = await _campusService.GetAllAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载院区列表失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ShowCreateForm()
    {
        IsEditing = false;
        EditingId = 0;
        FormCode = string.Empty;
        FormName = string.Empty;
        FormAddress = null;
        FormPhone = null;
        ShowForm = true;
    }

    [RelayCommand]
    private void ShowEditForm(CampusDto? campus)
    {
        if (campus is null) return;

        IsEditing = true;
        EditingId = campus.Id;
        FormCode = campus.Code;
        FormName = campus.Name;
        FormAddress = campus.Address;
        FormPhone = campus.Phone;
        ShowForm = true;
    }

    [RelayCommand]
    private void CancelForm()
    {
        ShowForm = false;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(FormName))
        {
            ErrorMessage = "院区名称不能为空";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsEditing)
            {
                var dto = new UpdateCampusDto(FormName, FormAddress, FormPhone);
                await _campusService.UpdateAsync(EditingId, dto);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FormCode))
                {
                    ErrorMessage = "院区编码不能为空";
                    return;
                }

                var dto = new CreateCampusDto(FormCode, FormName, FormAddress, FormPhone);
                await _campusService.CreateAsync(dto);
            }

            ShowForm = false;
            await LoadCampusesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleActive(CampusDto? campus)
    {
        if (campus is null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (campus.IsActive)
                await _campusService.DeactivateAsync(campus.Id);
            else
                await _campusService.ActivateAsync(campus.Id);

            await LoadCampusesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"操作失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

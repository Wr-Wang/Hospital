using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>科室管理 ViewModel</summary>
public sealed partial class DepartmentViewModel : ObservableObject
{
    private readonly IDepartmentApplicationService _deptService;
    private readonly ICampusApplicationService _campusService;

    public DepartmentViewModel(
        IDepartmentApplicationService deptService,
        ICampusApplicationService campusService)
    {
        _deptService = deptService;
        _campusService = campusService;
    }

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private List<CampusDto> campuses = new();

    [ObservableProperty]
    private CampusDto? selectedCampus;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ParentDeptOptions))]
    private List<DepartmentDto> departments = new();

    [ObservableProperty]
    private bool showForm;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ParentDeptOptions))]
    private bool isEditing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ParentDeptOptions))]
    private long editingId;

    [ObservableProperty]
    private string formCode = string.Empty;

    [ObservableProperty]
    private string formName = string.Empty;

    [ObservableProperty]
    private long formCampusId;

    [ObservableProperty]
    private string formType = "门诊";

    [ObservableProperty]
    private long? formParentId;

    public async Task InitializeAsync()
    {
        await LoadCampusesAsync();
    }

    private async Task LoadCampusesAsync()
    {
        try
        {
            Campuses = await _campusService.GetAllAsync();
            SelectedCampus = Campuses.FirstOrDefault();
            if (SelectedCampus is not null)
                await LoadDepartmentsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载院区失败: {ex.Message}";
        }
    }

    partial void OnSelectedCampusChanged(CampusDto? value)
    {
        if (value is not null)
            _ = LoadDepartmentsAsync();
    }

    private async Task LoadDepartmentsAsync()
    {
        if (SelectedCampus is null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Departments = await _deptService.GetTreeByCampusIdAsync(SelectedCampus.Id);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载科室列表失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private List<DepartmentDto> GetFlatList(List<DepartmentDto>? items)
    {
        var result = new List<DepartmentDto>();
        if (items is null) return result;

        foreach (var item in items)
        {
            result.Add(item);
            result.AddRange(GetFlatList(item.Children));
        }
        return result;
    }

    /// <summary>供上级科室选择器使用的扁平列表</summary>
    public List<DepartmentDto> ParentDeptOptions
    {
        get
        {
            var flat = GetFlatList(Departments);
            if (IsEditing)
                flat = flat.Where(d => d.Id != EditingId).ToList();
            return flat;
        }
    }

    [RelayCommand]
    private void ShowCreateForm()
    {
        IsEditing = false;
        EditingId = 0;
        FormCode = string.Empty;
        FormName = string.Empty;
        FormCampusId = SelectedCampus?.Id ?? 0;
        FormType = "门诊";
        FormParentId = null;
        ShowForm = true;
    }

    [RelayCommand]
    private void ShowEditForm(DepartmentDto? dept)
    {
        if (dept is null) return;

        IsEditing = true;
        EditingId = dept.Id;
        FormCode = dept.Code;
        FormName = dept.Name;
        FormCampusId = dept.CampusId;
        FormType = dept.Type;
        FormParentId = dept.ParentId;
        ShowForm = true;
    }

    [RelayCommand]
    private void CancelForm() => ShowForm = false;

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(FormName))
        {
            ErrorMessage = "科室名称不能为空";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsEditing)
            {
                var dto = new UpdateDepartmentDto(FormName, FormType, FormParentId);
                await _deptService.UpdateAsync(EditingId, dto);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FormCode))
                {
                    ErrorMessage = "科室编码不能为空";
                    return;
                }

                var dto = new CreateDepartmentDto(FormCode, FormName, FormCampusId, FormType, FormParentId);
                await _deptService.CreateAsync(dto);
            }

            ShowForm = false;
            await LoadDepartmentsAsync();
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
    private async Task ToggleActive(DepartmentDto? dept)
    {
        if (dept is null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (dept.IsActive)
                await _deptService.DeactivateAsync(dept.Id);
            else
                await _deptService.ActivateAsync(dept.Id);

            await LoadDepartmentsAsync();
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

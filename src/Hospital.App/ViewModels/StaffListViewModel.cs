using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.App.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>人员档案 ViewModel</summary>
public sealed partial class StaffListViewModel : ObservableObject
{
    private readonly IStaffApplicationService _staffService;
    private readonly IDepartmentApplicationService _deptService;
    private readonly ICampusApplicationService _campusService;

    public StaffListViewModel(
        IStaffApplicationService staffService,
        IDepartmentApplicationService deptService,
        ICampusApplicationService campusService)
    {
        _staffService = staffService;
        _deptService = deptService;
        _campusService = campusService;
    }

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string keyword = string.Empty;

    [ObservableProperty]
    private List<StaffDto> staffList = new();

    [ObservableProperty]
    private List<DepartmentDto> departments = new();

    [ObservableProperty]
    private DepartmentDto? selectedDept;

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
    private string formGender = "男";

    [ObservableProperty]
    private string? formPhone;

    [ObservableProperty]
    private long formCampusId;

    [ObservableProperty]
    private long formDeptId;

    [ObservableProperty]
    private string formLicenseType = "执业医师";

    [ObservableProperty]
    private string formLicenseNo = string.Empty;

    [ObservableProperty]
    private DateTime? formLicenseExpiry;

    public async Task InitializeAsync()
    {
        await Task.WhenAll(LoadCampusesAsync(), LoadDepartmentsAsync());
        await Search();
    }

    private async Task LoadCampusesAsync()
    {
        try
        {
            Campuses = await _campusService.GetAllAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载院区列表失败: {ex.Message}";
        }
    }

    private async Task LoadDepartmentsAsync()
    {
        try
        {
            Departments = await _deptService.GetAllAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载科室列表失败: {ex.Message}";
        }
    }

    partial void OnSelectedDeptChanged(DepartmentDto? value)
    {
        if (value is not null)
            _ = LoadStaffAsync();
    }

    [RelayCommand]
    private async Task Search()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                StaffList = await _staffService.GetAllAsync();
                StaffList = StaffList.Where(s =>
                    s.Name.Contains(Keyword, StringComparison.OrdinalIgnoreCase) ||
                    s.Code.Contains(Keyword, StringComparison.OrdinalIgnoreCase) ||
                    (s.Phone?.Contains(Keyword) ?? false)).ToList();
            }
            else
            {
                await LoadStaffAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"搜索失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadStaffAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (SelectedDept is not null)
                StaffList = await _staffService.GetByDeptIdAsync(SelectedDept.Id);
            else
                StaffList = await _staffService.GetAllAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载人员列表失败: {ex.Message}";
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
        FormGender = "男";
        FormPhone = null;
        FormCampusId = 0;
        FormDeptId = SelectedDept?.Id ?? 0;
        FormLicenseType = "执业医师";
        FormLicenseNo = string.Empty;
        FormLicenseExpiry = null;
        ShowForm = true;
    }

    [RelayCommand]
    private void ShowEditForm(StaffDto? staff)
    {
        if (staff is null) return;

        IsEditing = true;
        EditingId = staff.Id;
        FormCode = staff.Code;
        FormName = staff.Name;
        FormGender = staff.Gender;
        FormPhone = staff.Phone;
        FormCampusId = staff.CampusId;
        FormDeptId = staff.DeptId;
        FormLicenseType = staff.LicenseType;
        FormLicenseNo = staff.LicenseNo;
        FormLicenseExpiry = staff.LicenseExpiry;
        ShowForm = true;
    }

    [RelayCommand]
    private void CancelForm() => ShowForm = false;

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(FormName))
        {
            ErrorMessage = "人员姓名不能为空";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsEditing)
            {
                var dto = new UpdateStaffDto(FormName, FormGender, FormPhone, FormDeptId);
                await _staffService.UpdateAsync(EditingId, dto);

                if (!string.IsNullOrWhiteSpace(FormLicenseNo))
                {
                    var licenseDto = new UpdateStaffLicenseDto(FormLicenseType, FormLicenseNo,
                        FormLicenseExpiry ?? DateTime.Now.AddYears(5));
                    await _staffService.UpdateLicenseAsync(EditingId, licenseDto);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FormCode))
                {
                    ErrorMessage = "人员编码不能为空";
                    return;
                }

                var dto = new CreateStaffDto(FormCode, FormName, FormGender, FormPhone,
                    FormCampusId, FormDeptId, FormLicenseType, FormLicenseNo,
                    FormLicenseExpiry);
                await _staffService.CreateAsync(dto);
            }

            ShowForm = false;
            await LoadStaffAsync();
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
    private async Task ToggleActive(StaffDto? staff)
    {
        if (staff is null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (staff.IsActive)
                await _staffService.DeactivateAsync(staff.Id);
            else
                await _staffService.ActivateAsync(staff.Id);

            await LoadStaffAsync();
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

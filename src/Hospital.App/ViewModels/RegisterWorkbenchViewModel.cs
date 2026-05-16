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

/// <summary>挂号工作台 ViewModel，支持选科室→医生→时段→患者→确认挂号流程</summary>
public sealed partial class RegisterWorkbenchViewModel : ObservableObject
{
    private readonly IRegistrationApplicationService _registrationService;
    private readonly IScheduleApplicationService _scheduleService;
    private readonly IDepartmentApplicationService _departmentService;
    private readonly IStaffApplicationService _staffService;
    private readonly IPatientApplicationService _patientService;

    public RegisterWorkbenchViewModel(
        IRegistrationApplicationService registrationService,
        IScheduleApplicationService scheduleService,
        IDepartmentApplicationService departmentService,
        IStaffApplicationService staffService,
        IPatientApplicationService patientService)
    {
        _registrationService = registrationService;
        _scheduleService = scheduleService;
        _departmentService = departmentService;
        _staffService = staffService;
        _patientService = patientService;
    }

    // ===== 科室与医生 =====

    [ObservableProperty]
    private List<DepartmentDto> departments = new();

    [ObservableProperty]
    private DepartmentDto? selectedDept;

    [ObservableProperty]
    private List<StaffDto> doctors = new();

    [ObservableProperty]
    private StaffDto? selectedDoctor;

    // ===== 可用排班 =====

    [ObservableProperty]
    private List<ScheduleDto> availableSchedules = new();

    [ObservableProperty]
    private ScheduleDto? selectedSchedule;

    [ObservableProperty]
    private string selectedSlotName = string.Empty;

    // ===== 患者搜索与选择 =====

    [ObservableProperty]
    private string patientKeyword = string.Empty;

    [ObservableProperty]
    private List<PatientDto> patientResults = new();

    [ObservableProperty]
    private string selectedPatientInfo = string.Empty;

    private long _selectedPatientId;

    // ===== 页面状态 =====

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isSuccess;

    [ObservableProperty]
    private bool isRegistered;

    /// <summary>显示今日日期</summary>
    public string TodayDate => DateTime.Today.ToString("yyyy-MM-dd");

    // ===== 今日挂号列表 =====

    [ObservableProperty]
    private List<RegistrationDisplayItem> todayRegistrations = new();

    // ===== 初始化 =====

    public async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            Departments = await _departmentService.GetAllAsync();
            await LoadTodayRegistrations();
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"加载数据失败: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"初始化失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedDeptChanged(DepartmentDto? value)
    {
        if (value is not null)
        {
            SelectedDoctor = null;
            AvailableSchedules = new();
            SelectedSlotName = string.Empty;
            IsRegistered = false;
            _ = LoadDoctorsAsync(value.Id);
            _ = LoadSchedulesAsync(value.Id, null);
        }
    }

    partial void OnSelectedDoctorChanged(StaffDto? value)
    {
        if (value is not null && SelectedDept is not null)
        {
            AvailableSchedules = new();
            SelectedSlotName = string.Empty;
            IsRegistered = false;
            _ = LoadSchedulesAsync(SelectedDept.Id, value.Id);
        }
    }

    partial void OnSelectedScheduleChanged(ScheduleDto? value)
    {
        SelectedSlotName = string.Empty;
        IsRegistered = false;
    }

    private async Task LoadDoctorsAsync(long deptId)
    {
        try
        {
            Doctors = await _staffService.GetByDeptIdAsync(deptId);
        }
        catch
        {
            Doctors = new();
        }
    }

    private async Task LoadSchedulesAsync(long deptId, long? doctorId)
    {
        try
        {
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            AvailableSchedules = await _scheduleService.GetAvailableAsync(deptId, doctorId, today);
        }
        catch
        {
            AvailableSchedules = new();
        }
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
    private void SelectPatient(PatientDto? patient)
    {
        if (patient is null) return;
        _selectedPatientId = patient.Id;
        SelectedPatientInfo = $"{patient.Name}（病历号: {patient.PatientNo}）";
        IsRegistered = false;
    }

    // ===== 挂号 =====

    [RelayCommand]
    private async Task Register()
    {
        ErrorMessage = null;
        IsSuccess = false;

        if (SelectedDept is null || SelectedDoctor is null || SelectedSchedule is null)
        {
            ErrorMessage = "请选择科室、医生和排班时段";
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedSlotName))
        {
            ErrorMessage = "请选择就诊时段";
            return;
        }

        if (_selectedPatientId == 0)
        {
            ErrorMessage = "请搜索并选择患者";
            return;
        }

        IsBusy = true;

        try
        {
            var dto = new CreateRegistrationDto(
                _selectedPatientId, SelectedSchedule.Id, SelectedDoctor.Id,
                SelectedDept.Id, SelectedSchedule.CampusId, SelectedSlotName);

            await _registrationService.RegisterAsync(dto);
            IsSuccess = true;
            IsRegistered = true;

            await LoadTodayRegistrations();
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"挂号失败: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"挂号出错: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ===== 退号 =====

    [RelayCommand]
    private async Task VoidRegistration(long id)
    {
        ErrorMessage = null;

        try
        {
            await _registrationService.VoidAsync(id);
            await LoadTodayRegistrations();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"退号失败: {ex.Message}";
        }
    }

    // ===== 加载今日挂号 =====

    private async Task LoadTodayRegistrations()
    {
        try
        {
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            var registrations = await _registrationService.GetByDoctorAsync(0, today);

            TodayRegistrations = registrations.Select(r => new RegistrationDisplayItem
            {
                Id = r.Id,
                QueueNumber = r.QueueNumber,
                SlotName = r.SlotName,
                Status = r.Status,
                RegisterTime = r.RegisterTime,
                PatientDisplay = $"#{r.QueueNumber} (ID: {r.PatientId})",
            }).ToList();
        }
        catch
        {
            TodayRegistrations = new();
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        ErrorMessage = null;
        await LoadTodayRegistrations();
    }
}

/// <summary>挂号记录展示项</summary>
public sealed class RegistrationDisplayItem
{
    public long Id { get; set; }
    public int QueueNumber { get; set; }
    public string SlotName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RegisterTime { get; set; } = string.Empty;
    public string PatientDisplay { get; set; } = string.Empty;
    public bool CanVoid => Status == "已挂号";
}

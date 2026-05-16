using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>排班号表页面 ViewModel，支持排班查询、创建、发布/停用、号源修改</summary>
public sealed partial class ScheduleViewModel : ObservableObject
{
    private readonly IScheduleApplicationService _scheduleService;
    private readonly IDepartmentApplicationService _departmentService;
    private readonly IStaffApplicationService _staffService;

    public ScheduleViewModel(
        IScheduleApplicationService scheduleService,
        IDepartmentApplicationService departmentService,
        IStaffApplicationService staffService)
    {
        _scheduleService = scheduleService;
        _departmentService = departmentService;
        _staffService = staffService;
    }

    // ===== 筛选条件 =====

    [ObservableProperty]
    private List<DepartmentDto> departments = new();

    [ObservableProperty]
    private DepartmentDto? selectedDept;

    [ObservableProperty]
    private List<StaffDto> doctors = new();

    [ObservableProperty]
    private StaffDto? selectedDoctor;

    [ObservableProperty]
    private string scheduleDate = DateTime.Today.ToString("yyyy-MM-dd");

    // ===== 排班列表 =====

    [ObservableProperty]
    private List<ScheduleDto> schedules = new();

    // ===== 页面状态 =====

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    // ===== 创建排班表单 =====

    [ObservableProperty]
    private bool showCreateForm;

    [ObservableProperty]
    private DepartmentDto? createDept;

    [ObservableProperty]
    private StaffDto? createDoctor;

    [ObservableProperty]
    private string createDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

    public ObservableCollection<ScheduleSlotEntry> CreateSlots { get; } = new()
    {
        new("上午", "08:00", "12:00", 30),
        new("下午", "13:00", "17:00", 30),
    };

    // ===== 初始化 =====

    /// <summary>页面加载时初始化科室列表</summary>
    public async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            Departments = await _departmentService.GetAllAsync();
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"加载科室列表失败: {ex.Message}";
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
            _ = LoadDoctorsAsync(value.Id);
    }

    partial void OnCreateDeptChanged(DepartmentDto? value)
    {
        if (value is not null)
            _ = LoadCreateDoctorsAsync(value.Id);
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

    private async Task LoadCreateDoctorsAsync(long deptId)
    {
        try
        {
            CreateDoctor = null;
            Doctors = await _staffService.GetByDeptIdAsync(deptId);
        }
        catch
        {
            Doctors = new();
        }
    }

    // ===== 查询排班 =====

    [RelayCommand]
    private async Task LoadSchedules()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            if (SelectedDept is not null)
            {
                Schedules = await _scheduleService.GetByDeptAsync(SelectedDept.Id, ScheduleDate);
            }
            else
            {
                Schedules = new();
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"查询排班失败: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"查询出错: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ===== 创建排班 =====

    [RelayCommand]
    private void ToggleCreateForm()
    {
        ShowCreateForm = !ShowCreateForm;
        ErrorMessage = null;
    }

    [RelayCommand]
    private void AddSlot()
    {
        CreateSlots.Add(new("", "08:00", "12:00", 10));
    }

    [RelayCommand]
    private void RemoveSlot(ScheduleSlotEntry? slot)
    {
        if (slot is not null && CreateSlots.Count > 1)
            CreateSlots.Remove(slot);
    }

    [RelayCommand]
    private async Task CreateSchedule()
    {
        ErrorMessage = null;

        if (CreateDoctor is null || CreateDept is null)
        {
            ErrorMessage = "请选择科室和医生";
            return;
        }

        if (CreateSlots.Any(s => string.IsNullOrWhiteSpace(s.SlotName)))
        {
            ErrorMessage = "请填写所有时段名称";
            return;
        }

        IsBusy = true;

        try
        {
            var slotDtos = CreateSlots.Select(s => new CreateScheduleSlotDto(
                s.SlotName, s.StartTime, s.EndTime, s.TotalQuota)).ToList();

            var dto = new CreateScheduleDto(
                CreateDoctor.Id, CreateDept.Id, 1L, CreateDate, slotDtos);

            await _scheduleService.CreateAsync(dto);
            ShowCreateForm = false;
            await LoadSchedules();
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"创建排班失败: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"创建出错: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ===== 排班操作 =====

    [RelayCommand]
    private async Task PublishSchedule(long id)
    {
        try
        {
            await _scheduleService.PublishAsync(id);
            await LoadSchedules();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"发布失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeactivateSchedule(long id)
    {
        try
        {
            await _scheduleService.DeactivateAsync(id);
            await LoadSchedules();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"停用失败: {ex.Message}";
        }
    }

    // ===== 状态显示辅助 =====

    public static string StatusDisplay(string status) => status switch
    {
        "已发布" => "已发布",
        "已停用" => "已停用",
        "已满" => "已满",
        _ => status,
    };

    public static bool CanPublish(string status) => status == "已停用";
    public static bool CanDeactivate(string status) => status == "已发布" || status == "已满";
}

/// <summary>创建排班时的时段条目</summary>
public sealed partial class ScheduleSlotEntry : ObservableObject
{
    public ScheduleSlotEntry(string slotName, string startTime, string endTime, int totalQuota)
    {
        SlotName = slotName;
        StartTime = startTime;
        EndTime = endTime;
        TotalQuota = totalQuota;
    }

    [ObservableProperty]
    private string slotName = string.Empty;

    [ObservableProperty]
    private string startTime = string.Empty;

    [ObservableProperty]
    private string endTime = string.Empty;

    [ObservableProperty]
    private int totalQuota;
}

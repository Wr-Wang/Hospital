using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>用户与角色管理 ViewModel，TabControl 双标签页</summary>
public sealed partial class UserRoleViewModel : ObservableObject
{
    private readonly IUserRoleApplicationService _userRoleService;

    public UserRoleViewModel(IUserRoleApplicationService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    // ===== 页面状态 =====

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isSuccess;

    [ObservableProperty]
    private int selectedTabIndex;

    // ===== 用户列表 =====

    [ObservableProperty]
    private List<UserDto> users = new();

    [ObservableProperty]
    private UserDto? selectedUser;

    // ===== 新建用户表单 =====

    [ObservableProperty]
    private bool showCreateUser;

    [ObservableProperty]
    private string newLoginName = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string newDisplayName = string.Empty;

    [ObservableProperty]
    private string newCampusName = "总院区";

    [ObservableProperty]
    private string newUserRole = string.Empty;

    // ===== 角色列表 =====

    [ObservableProperty]
    private List<RoleDto> roles = new();

    [ObservableProperty]
    private RoleDto? selectedRole;

    [ObservableProperty]
    private bool showCreateRole;

    [ObservableProperty]
    private string newRoleName = string.Empty;

    [ObservableProperty]
    private string newRoleDesc = string.Empty;

    /// <summary>所有可用权限列表</summary>
    public List<PermissionGroup> AllPermissions { get; } = new()
    {
        new PermissionGroup("基础权限", new List<PermissionOption>
        {
            new PermissionOption("sys.shell.use", "登录使用系统"),
        }),
        new PermissionGroup("系统管理", new List<PermissionOption>
        {
            new PermissionOption("sys.security.manage", "用户与角色管理"),
        }),
        new PermissionGroup("主数据", new List<PermissionOption>
        {
            new PermissionOption("mdm.campus.manage", "院区管理"),
            new PermissionOption("mdm.dept.manage", "科室维护"),
            new PermissionOption("mdm.staff.manage", "人员档案"),
            new PermissionOption("mdm.dict.manage", "字典管理"),
        }),
        new PermissionGroup("患者", new List<PermissionOption>
        {
            new PermissionOption("pat.register", "患者建档"),
            new PermissionOption("pat.search", "患者检索"),
        }),
        new PermissionGroup("挂号", new List<PermissionOption>
        {
            new PermissionOption("opd.schedule", "排班号表"),
            new PermissionOption("opd.register", "挂号工作台"),
        }),
        new PermissionGroup("门诊", new List<PermissionOption>
        {
            new PermissionOption("opd.encounter", "门诊医生站"),
        }),
        new PermissionGroup("药房", new List<PermissionOption>
        {
            new PermissionOption("pha.dispense", "发药工作台"),
        }),
        new PermissionGroup("收费", new List<PermissionOption>
        {
            new PermissionOption("fin.cash", "收费工作台"),
        }),
    };

    /// <summary>新建角色已选权限</summary>
    public ObservableCollection<string> NewRolePermissions { get; } = new();

    // ===== 初始化 =====

    public async Task InitializeAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsBusy = true;
            await Task.WhenAll(LoadUsersAsync(), LoadRolesAsync());
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载数据失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            Users = await _userRoleService.GetAllUsersAsync();
        }
        catch
        {
            Users = new();
        }
    }

    private async Task LoadRolesAsync()
    {
        try
        {
            Roles = await _userRoleService.GetAllRolesAsync();
        }
        catch
        {
            Roles = new();
        }
    }

    // ===== 创建用户 =====

    [RelayCommand]
    private void ToggleCreateUser()
    {
        ShowCreateUser = !ShowCreateUser;
        ErrorMessage = null;
    }

    [RelayCommand]
    private async Task CreateUser()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(NewLoginName) || string.IsNullOrWhiteSpace(NewDisplayName))
        {
            ErrorMessage = "请填写登录名和显示名称";
            return;
        }

        IsBusy = true;

        try
        {
            var roles = string.IsNullOrWhiteSpace(NewUserRole) ? new List<string>() : new List<string> { NewUserRole };
            var dto = new CreateUserDto(NewLoginName, NewPassword, NewDisplayName, NewCampusName, roles);
            await _userRoleService.CreateUserAsync(dto);
            ShowCreateUser = false;
            ClearCreateUserForm();
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"创建用户失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ClearCreateUserForm()
    {
        NewLoginName = string.Empty;
        NewPassword = string.Empty;
        NewDisplayName = string.Empty;
        NewCampusName = "总院区";
        NewUserRole = string.Empty;
    }

    [RelayCommand]
    private async Task ToggleUserActive(UserDto? user)
    {
        if (user is null) return;

        try
        {
            var dto = new UpdateUserDto(null, null, !user.IsActive, null);
            await _userRoleService.UpdateUserAsync(user.Id, dto);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"操作失败: {ex.Message}";
        }
    }

    // ===== 角色操作 =====

    [RelayCommand]
    private void ToggleCreateRole()
    {
        ShowCreateRole = !ShowCreateRole;
        ErrorMessage = null;
    }

    [RelayCommand]
    private void ToggleRolePermission(string? permission)
    {
        if (permission is null) return;

        if (NewRolePermissions.Contains(permission))
            NewRolePermissions.Remove(permission);
        else
            NewRolePermissions.Add(permission);
    }

    [RelayCommand]
    private async Task CreateRole()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(NewRoleName))
        {
            ErrorMessage = "请填写角色名称";
            return;
        }

        IsBusy = true;

        try
        {
            var dto = new CreateRoleDto(NewRoleName, NewRoleDesc, NewRolePermissions.ToList());
            await _userRoleService.CreateRoleAsync(dto);
            ShowCreateRole = false;
            NewRoleName = string.Empty;
            NewRoleDesc = string.Empty;
            NewRolePermissions.Clear();
            await LoadRolesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"创建角色失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteRole(RoleDto? role)
    {
        if (role is null) return;

        try
        {
            await _userRoleService.DeleteRoleAsync(role.Id);
            await LoadRolesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除角色失败: {ex.Message}";
        }
    }

    /// <summary>检查指定权限是否被选中</summary>
    public bool IsPermissionSelected(string permission)
        => NewRolePermissions.Contains(permission);
}

/// <summary>权限分组展示</summary>
public sealed record PermissionGroup(string GroupName, List<PermissionOption> Permissions);

/// <summary>权限选项</summary>
public sealed record PermissionOption(string Value, string Label);

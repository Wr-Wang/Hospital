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

/// <summary>患者建档页面 ViewModel，支持身份证号自动查重</summary>
public sealed partial class PatientRegisterViewModel : ObservableObject
{
    private readonly IPatientApplicationService _patientService;

    public PatientRegisterViewModel(IPatientApplicationService patientService)
    {
        _patientService = patientService;
    }

    // ===== 患者搜索 =====

    [ObservableProperty]
    private string patientKeyword = string.Empty;

    [ObservableProperty]
    private List<PatientDto> patientResults = new();

    [ObservableProperty]
    private bool isSearching;

    [ObservableProperty]
    private string selectedPatientInfo = string.Empty;

    private long _selectedPatientId;

    // ===== 表单字段 =====

    [ObservableProperty]
    private string patientNo = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string? selectedGender;

    [ObservableProperty]
    private string? birthDate;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? idCard;

    [ObservableProperty]
    private string? allergiesText;

    // ===== 状态字段 =====

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isSuccess;

    [ObservableProperty]
    private bool showDuplicateWarning;

    [ObservableProperty]
    private string duplicateMessage = string.Empty;

    [ObservableProperty]
    private List<PatientDto>? duplicatePatients;

    /// <summary>性别下拉选项（中文展示）</summary>
    public List<string> GenderOptions => GenderMapper.DisplayOptions;

    /// <summary>身份证号变更时自动触发查重（仅当输入满 18 位时）</summary>
    partial void OnIdCardChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Length == AppConstants.IdCardFullLength && !IsBusy)
        {
            _ = CheckDuplicateByIdCard();
        }
    }

    /// <summary>按身份证号查重，若已存在则显示警告</summary>
    private async Task CheckDuplicateByIdCard()
    {
        try
        {
            var existing = await _patientService.GetByIdCardAsync(IdCard!);
            if (existing is not null)
            {
                ShowDuplicateWarning = true;
                DuplicateMessage = $"⚠️ 身份证号已存在：{existing.Name}（病历号：{existing.PatientNo}）";
            }
            else
            {
                ShowDuplicateWarning = false;
            }
        }
        catch (HttpRequestException)
        {
            // 网络错误时静默处理，不影响用户填写
        }
        catch (TaskCanceledException)
        {
            // 超时时静默处理
        }
    }

    // ===== 患者搜索 =====

    /// <summary>按关键字搜索已存在的患者（用于从预约/预登记信息中快速匹配建档）</summary>
    [RelayCommand]
    private async Task SearchPatient()
    {
        ErrorMessage = null;
        IsSuccess = false;

        if (string.IsNullOrWhiteSpace(PatientKeyword))
        {
            PatientResults = new();
            return;
        }

        IsSearching = true;

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
            IsSearching = false;
        }
    }

    /// <summary>选择已搜索到的患者，自动填入表单各字段</summary>
    [RelayCommand]
    private void SelectPatient(PatientDto? patient)
    {
        if (patient is null) return;

        _selectedPatientId = patient.Id;
        SelectedPatientInfo = $"{patient.Name}（病历号: {patient.PatientNo}）";

        // 自动填入表单
        PatientNo = patient.PatientNo;
        Name = patient.Name;
        SelectedGender = GenderMapper.ToDisplayValue(patient.Gender);
        BirthDate = patient.BirthDate;
        Phone = patient.Phone;
        IdCard = patient.IdCard;
        AllergiesText = patient.AllergiesText;

        // 清除错误/警告
        ErrorMessage = null;
        ShowDuplicateWarning = false;
    }

    /// <summary>清除患者选择，清空表单</summary>
    [RelayCommand]
    private void ClearSelection()
    {
        _selectedPatientId = 0;
        SelectedPatientInfo = string.Empty;
        PatientResults = new();
        PatientKeyword = string.Empty;
        ClearForm();
    }

    // ===== 提交建档 =====

    /// <summary>提交建档表单，调用后端 API 创建患者</summary>
    [RelayCommand]
    private async Task Submit()
    {
        ErrorMessage = null;
        IsSuccess = false;

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(PatientNo))
        {
            ErrorMessage = "姓名和病历号为必填项";
            return;
        }

        IsBusy = true;

        try
        {
            var dto = new CreatePatientDto(PatientNo, Name, GenderMapper.ToApiValue(SelectedGender),
                BirthDate, Phone, AllergiesText, IdCard);
            var id = await _patientService.CreateAsync(dto);
            IsSuccess = true;
            ErrorMessage = $"建档成功！患者 ID: {id}";
            ClearForm();
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"服务器错误: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"建档失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>提交成功后清空表单</summary>
    private void ClearForm()
    {
        _selectedPatientId = 0;
        SelectedPatientInfo = string.Empty;
        PatientNo = string.Empty;
        Name = string.Empty;
        SelectedGender = null;
        BirthDate = null;
        Phone = null;
        IdCard = null;
        AllergiesText = null;
        ShowDuplicateWarning = false;
    }
}

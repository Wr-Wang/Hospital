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

public sealed partial class Patient360ViewModel : ObservableObject
{
    private readonly IPatientApplicationService _patientService;
    private long _patientId;

    public Patient360ViewModel(IPatientApplicationService patientService)
    {
        _patientService = patientService;
    }

    // ===== 患者头部展示字段 =====

    [ObservableProperty]
    private string? patientName;

    [ObservableProperty]
    private string? patientInfo;

    [ObservableProperty]
    private string? patientIdLabel;

    // ===== 页面状态字段 =====

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool hasPatient;

    [ObservableProperty]
    private int selectedTab;

    // ===== Tab1：基本信息字段 =====

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? gender;

    [ObservableProperty]
    private string? birthDate;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? idCard;

    [ObservableProperty]
    private string? patientNo;

    [ObservableProperty]
    private string? allergiesText;

    // ===== Tab2：就诊历史 =====

    [ObservableProperty]
    private string visitHistoryText = AppConstants.NoVisitHistory;

    [ObservableProperty]
    private ObservableCollection<VisitSummaryDto> visitHistory = new();

    public bool HasVisits => VisitHistory.Count > 0;

    // ===== Tab3：处方记录 =====

    [ObservableProperty]
    private ObservableCollection<PrescriptionDto> prescriptions = new();

    public bool HasPrescriptions => Prescriptions.Count > 0;

    // ===== Tab4：检查报告（检验 + 放射） =====

    [ObservableProperty]
    private ObservableCollection<LabOrderDto> labOrders = new();

    public bool HasLabOrders => LabOrders.Count > 0;

    [ObservableProperty]
    private ObservableCollection<RadOrderDto> radOrders = new();

    public bool HasRadOrders => RadOrders.Count > 0;

    /// <summary>根据患者 ID 加载完整 360 视图数据</summary>
    public async Task LoadPatientAsync(long patientId)
    {
        _patientId = patientId;
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            var profile = await _patientService.GetProfileAsync(patientId);
            if (profile is null)
            {
                ErrorMessage = AppConstants.PatientNotFound;
                HasPatient = false;
                return;
            }

            // 更新患者信息
            PatientName = profile.Name;
            PatientInfo = $"{profile.Gender ?? AppConstants.NullDisplay}{AppConstants.Separator}{profile.BirthDate ?? AppConstants.NullDisplay}{AppConstants.Separator}{profile.Phone ?? AppConstants.NullDisplay}";
            PatientIdLabel = $"病历号: {profile.PatientNo} | 身份证: {profile.IdCard ?? AppConstants.NullDisplay}";

            // 更新基本字段
            Name = profile.Name;
            Gender = profile.Gender is not null ? GenderMapper.ToDisplayValue(profile.Gender) : null;
            BirthDate = profile.BirthDate;
            Phone = profile.Phone;
            IdCard = profile.IdCard;
            PatientNo = profile.PatientNo;
            AllergiesText = profile.AllergiesText;

            // 就诊历史
            VisitHistory = new ObservableCollection<VisitSummaryDto>(profile.RecentVisits ?? new());
            VisitHistoryText = profile.RecentVisits is { Count: > 0 }
                ? $"共 {profile.RecentVisits.Count} 条就诊记录"
                : AppConstants.NoVisitHistory;

            // 处方记录
            Prescriptions = new ObservableCollection<PrescriptionDto>(profile.Prescriptions ?? new());

            // 检查报告
            LabOrders = new ObservableCollection<LabOrderDto>(profile.LabOrders ?? new());
            RadOrders = new ObservableCollection<RadOrderDto>(profile.RadOrders ?? new());

            // 集合赋值不会自动刷新计算属性，手动通知
            OnPropertyChanged(nameof(HasVisits));
            OnPropertyChanged(nameof(HasPrescriptions));
            OnPropertyChanged(nameof(HasLabOrders));
            OnPropertyChanged(nameof(HasRadOrders));

            HasPatient = true;
            SelectedTab = 0;
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"获取患者信息失败: {ex.Message}";
            HasPatient = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"出错: {ex.Message}";
            HasPatient = false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

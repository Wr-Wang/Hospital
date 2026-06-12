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

/// <summary>门诊医生站 ViewModel，TabControl 多标签页设计</summary>
public sealed partial class EncounterWorkbenchViewModel : ObservableObject
{
    private readonly IEncounterApplicationService _encounterService;
    private readonly IMedicalRecordApplicationService _medicalRecordService;
    private readonly IDiagnosisApplicationService _diagnosisService;
    private readonly IPrescriptionApplicationService _prescriptionService;
    private readonly ILabOrderApplicationService _labOrderService;
    private readonly IPatientApplicationService _patientService;

    public EncounterWorkbenchViewModel(
        IEncounterApplicationService encounterService,
        IMedicalRecordApplicationService medicalRecordService,
        IDiagnosisApplicationService diagnosisService,
        IPrescriptionApplicationService prescriptionService,
        ILabOrderApplicationService labOrderService,
        IPatientApplicationService patientService)
    {
        _encounterService = encounterService;
        _medicalRecordService = medicalRecordService;
        _diagnosisService = diagnosisService;
        _prescriptionService = prescriptionService;
        _labOrderService = labOrderService;
        _patientService = patientService;
    }

    // ===== 页面状态 =====

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private int selectedTabIndex;

    public string TodayDate => DateTime.Today.ToString("yyyy-MM-dd");

    /// <summary>当前选中的就诊 ID，Tab 切换时加载对应数据</summary>
    private long _currentEncounterId;

    // ===== Tab 1: 患者队列 =====

    [ObservableProperty]
    private List<EncounterQueueItemDto> queueItems = new();

    [ObservableProperty]
    private EncounterQueueItemDto? selectedQueueItem;

    [ObservableProperty]
    private string selectedPatientInfo = string.Empty;

    public bool HasSelectedPatient => _currentEncounterId > 0;

    // ===== Tab 2: 病历 =====

    [ObservableProperty]
    private string chiefComplaint = string.Empty;

    [ObservableProperty]
    private string presentIllness = string.Empty;

    [ObservableProperty]
    private string pastHistory = string.Empty;

    [ObservableProperty]
    private string physicalExam = string.Empty;

    [ObservableProperty]
    private string recordStatusText = string.Empty;

    [ObservableProperty]
    private bool hasRecord;

    // ===== Tab 3: 诊断 =====

    [ObservableProperty]
    private List<DiagnosisDto> diagnoses = new();

    [ObservableProperty]
    private string newDiagnosisType = "主要诊断";

    [ObservableProperty]
    private string newIcdCode = string.Empty;

    [ObservableProperty]
    private string newDiagnosisDesc = string.Empty;

    public List<string> DiagnosisTypeOptions { get; } = new() { "主要诊断", "次要诊断", "疑似诊断" };

    // ===== Tab 4: 处方 =====

    [ObservableProperty]
    private List<PrescriptionDto> prescriptions = new();

    public ObservableCollection<PrescriptionItemEntry> NewPrescriptionItems { get; } = new()
    {
        new("", "", "", "", "", 1, 1, ""),
    };

    // ===== Tab 5: 检验检查 =====

    [ObservableProperty]
    private List<LabOrderDto> labOrders = new();

    [ObservableProperty]
    private string newLabItemCode = string.Empty;

    [ObservableProperty]
    private string newLabItemName = string.Empty;

    // ===== 初始化 =====

    public async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            await LoadQueueAsync();
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

    // ===== 队列加载 =====

    [RelayCommand]
    private async Task LoadQueue()
    {
        ErrorMessage = null;
        await LoadQueueAsync();
    }

    private async Task LoadQueueAsync()
    {
        try
        {
            // 使用当前医生 ID（从登录上下文获取），默认使用 1（模拟）
            var doctorId = 1L;
            QueueItems = await _encounterService.GetQueueAsync(doctorId, TodayDate);
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"加载队列失败: {ex.Message}";
            QueueItems = new();
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "加载队列超时，请重试";
            QueueItems = new();
        }
    }

    // ===== 选择患者 =====

    [RelayCommand]
    private async Task SelectPatient(EncounterQueueItemDto? item)
    {
        if (item is null) return;

        SelectedQueueItem = item;
        _currentEncounterId = item.Id;
        SelectedPatientInfo = $"{item.PatientName} | 排队号: {item.QueueNumber} | {item.SlotName}";

        // 加载该就诊的所有数据
        SelectedTabIndex = 1; // 自动切换到病历 Tab
        await LoadEncounterDataAsync();
    }

    private async Task LoadEncounterDataAsync()
    {
        await Task.WhenAll(
            LoadMedicalRecordAsync(),
            LoadDiagnosesAsync(),
            LoadPrescriptionsAsync(),
            LoadLabOrdersAsync());
    }

    // ===== 接诊操作 =====

    [RelayCommand]
    private async Task StartConsultation()
    {
        if (_currentEncounterId == 0) return;
        ErrorMessage = null;

        try
        {
            await _encounterService.StartConsultationAsync(_currentEncounterId);
            await LoadQueueAsync();
            // 更新选中项状态
            if (SelectedQueueItem is not null)
            {
                var updated = QueueItems.FirstOrDefault(q => q.Id == _currentEncounterId);
                if (updated is not null)
                    SelectedQueueItem = updated;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"开始接诊失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CompleteConsultation()
    {
        if (_currentEncounterId == 0) return;
        ErrorMessage = null;

        try
        {
            await _encounterService.CompleteConsultationAsync(_currentEncounterId);
            _currentEncounterId = 0;
            SelectedQueueItem = null;
            SelectedPatientInfo = string.Empty;
            await LoadQueueAsync();
            ClearEncounterData();
            SelectedTabIndex = 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"完成接诊失败: {ex.Message}";
        }
    }

    private void ClearEncounterData()
    {
        ChiefComplaint = string.Empty;
        PresentIllness = string.Empty;
        PastHistory = string.Empty;
        PhysicalExam = string.Empty;
        RecordStatusText = string.Empty;
        HasRecord = false;
        Diagnoses = new();
        Prescriptions = new();
        LabOrders = new();
    }

    // ===== 病历 =====

    private async Task LoadMedicalRecordAsync()
    {
        try
        {
            var record = await _medicalRecordService.GetByEncounterAsync(_currentEncounterId);
            if (record is not null)
            {
                ChiefComplaint = record.ChiefComplaint;
                PresentIllness = record.PresentIllness;
                PastHistory = record.PastHistory;
                PhysicalExam = record.PhysicalExam;
                RecordStatusText = record.Status;
                HasRecord = true;
            }
            else
            {
                ClearMedicalRecord();
            }
        }
        catch (HttpRequestException)
        {
            ClearMedicalRecord();
        }
        catch (TaskCanceledException)
        {
            ClearMedicalRecord();
        }
    }

    private void ClearMedicalRecord()
    {
        ChiefComplaint = string.Empty;
        PresentIllness = string.Empty;
        PastHistory = string.Empty;
        PhysicalExam = string.Empty;
        RecordStatusText = string.Empty;
        HasRecord = false;
    }

    [RelayCommand]
    private async Task SaveMedicalRecord()
    {
        if (_currentEncounterId == 0) return;
        ErrorMessage = null;

        try
        {
            var dto = new SaveMedicalRecordDto(
                ChiefComplaint, PresentIllness, PastHistory, PhysicalExam, false);
            await _medicalRecordService.SaveAsync(_currentEncounterId, dto);
            RecordStatusText = "草稿";
            HasRecord = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存病历失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SubmitMedicalRecord()
    {
        if (_currentEncounterId == 0) return;
        ErrorMessage = null;

        try
        {
            var dto = new SaveMedicalRecordDto(
                ChiefComplaint, PresentIllness, PastHistory, PhysicalExam, true);
            await _medicalRecordService.SaveAsync(_currentEncounterId, dto);
            RecordStatusText = "终稿";
            HasRecord = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"提交病历失败: {ex.Message}";
        }
    }

    // ===== 诊断 =====

    private async Task LoadDiagnosesAsync()
    {
        try
        {
            Diagnoses = await _diagnosisService.GetByEncounterAsync(_currentEncounterId);
        }
        catch (HttpRequestException)
        {
            Diagnoses = new();
        }
        catch (TaskCanceledException)
        {
            Diagnoses = new();
        }
    }

    [RelayCommand]
    private async Task AddDiagnosis()
    {
        if (_currentEncounterId == 0 || string.IsNullOrWhiteSpace(NewDiagnosisDesc)) return;
        ErrorMessage = null;

        try
        {
            var dto = new CreateDiagnosisDto(NewDiagnosisType, NewIcdCode, NewDiagnosisDesc, NewDiagnosisType == "主要诊断");
            await _diagnosisService.AddAsync(_currentEncounterId, dto);
            NewIcdCode = string.Empty;
            NewDiagnosisDesc = string.Empty;
            await LoadDiagnosesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"添加诊断失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RemoveDiagnosis(long id)
    {
        try
        {
            await _diagnosisService.RemoveAsync(id);
            await LoadDiagnosesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除诊断失败: {ex.Message}";
        }
    }

    // ===== 处方 =====

    private async Task LoadPrescriptionsAsync()
    {
        try
        {
            Prescriptions = await _prescriptionService.GetByEncounterAsync(_currentEncounterId);
        }
        catch (HttpRequestException)
        {
            Prescriptions = new();
        }
        catch (TaskCanceledException)
        {
            Prescriptions = new();
        }
    }

    [RelayCommand]
    private void AddPrescriptionItem()
    {
        NewPrescriptionItems.Add(new("", "", "", "", "", 1, 1, ""));
    }

    [RelayCommand]
    private void RemovePrescriptionItem(PrescriptionItemEntry? item)
    {
        if (item is not null && NewPrescriptionItems.Count > 1)
            NewPrescriptionItems.Remove(item);
    }

    [RelayCommand]
    private async Task CreatePrescription()
    {
        if (_currentEncounterId == 0) return;
        ErrorMessage = null;

        if (NewPrescriptionItems.Any(i => string.IsNullOrWhiteSpace(i.DrugName)))
        {
            ErrorMessage = "请填写所有药品名称";
            return;
        }

        IsBusy = true;

        try
        {
            var items = NewPrescriptionItems.Select(i => new CreatePrescriptionItemDto(
                i.DrugName, i.Spec, i.Form, i.Freq, i.Dosage, i.Duration, i.Qty,
                string.IsNullOrWhiteSpace(i.Note) ? null : i.Note)).ToList();

            var dto = new CreatePrescriptionDto(items);
            await _prescriptionService.CreateAsync(_currentEncounterId, 1L, dto);
            NewPrescriptionItems.Clear();
            NewPrescriptionItems.Add(new("", "", "", "", "", 1, 1, ""));
            await LoadPrescriptionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"开立处方失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task VoidPrescription(long id)
    {
        try
        {
            await _prescriptionService.VoidAsync(id);
            await LoadPrescriptionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"作废处方失败: {ex.Message}";
        }
    }

    // ===== 检验检查 =====

    private async Task LoadLabOrdersAsync()
    {
        try
        {
            LabOrders = await _labOrderService.GetByEncounterAsync(_currentEncounterId);
        }
        catch (HttpRequestException)
        {
            LabOrders = new();
        }
        catch (TaskCanceledException)
        {
            LabOrders = new();
        }
    }

    [RelayCommand]
    private async Task AddLabOrder()
    {
        if (_currentEncounterId == 0 || string.IsNullOrWhiteSpace(NewLabItemName)) return;
        ErrorMessage = null;

        try
        {
            var dto = new CreateLabOrderDto(NewLabItemCode, NewLabItemName);
            await _labOrderService.CreateAsync(_currentEncounterId, dto);
            NewLabItemCode = string.Empty;
            NewLabItemName = string.Empty;
            await LoadLabOrdersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"添加检验申请失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CancelLabOrder(long id)
    {
        try
        {
            await _labOrderService.CancelAsync(id);
            await LoadLabOrdersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"取消检验申请失败: {ex.Message}";
        }
    }
}

/// <summary>处方条目编辑模型</summary>
public sealed partial class PrescriptionItemEntry : ObservableObject
{
    public PrescriptionItemEntry(string drugName, string spec, string form,
        string freq, string dosage, int duration, int qty, string note)
    {
        DrugName = drugName;
        Spec = spec;
        Form = form;
        Freq = freq;
        Dosage = dosage;
        Duration = duration;
        Qty = qty;
        Note = note;
    }

    [ObservableProperty]
    private string drugName = string.Empty;

    [ObservableProperty]
    private string spec = string.Empty;

    [ObservableProperty]
    private string form = string.Empty;

    [ObservableProperty]
    private string freq = string.Empty;

    [ObservableProperty]
    private string dosage = string.Empty;

    [ObservableProperty]
    private int duration;

    [ObservableProperty]
    private int qty;

    [ObservableProperty]
    private string note = string.Empty;
}

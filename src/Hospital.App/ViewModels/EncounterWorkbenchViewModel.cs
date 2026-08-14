using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.App.Constants;
using Hospital.App.Services;
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
    private readonly IDictionaryApplicationService _dictService;
    private readonly IAppContext _appContext;
    private readonly INotificationService _notifications;

    public EncounterWorkbenchViewModel(
        IEncounterApplicationService encounterService,
        IMedicalRecordApplicationService medicalRecordService,
        IDiagnosisApplicationService diagnosisService,
        IPrescriptionApplicationService prescriptionService,
        ILabOrderApplicationService labOrderService,
        IPatientApplicationService patientService,
        IDictionaryApplicationService dictService,
        IAppContext appContext,
        INotificationService notificationService)
    {
        _encounterService = encounterService;
        _medicalRecordService = medicalRecordService;
        _diagnosisService = diagnosisService;
        _prescriptionService = prescriptionService;
        _labOrderService = labOrderService;
        _patientService = patientService;
        _dictService = dictService;
        _appContext = appContext;
        _notifications = notificationService;
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
    private ObservableCollection<QueueRow> queueRows = new();

    [ObservableProperty]
    private QueueRow? selectedQueueItem;

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

    [ObservableProperty]
    private DictionaryItemDto? selectedIcdItem;

    public List<string> DiagnosisTypeOptions { get; } = new() { "主要诊断", "次要诊断", "疑似诊断" };

    // ===== Tab 4: 处方 =====

    [ObservableProperty]
    private List<PrescriptionDto> prescriptions = new();

    public ObservableCollection<PrescriptionItemEntry> NewPrescriptionItems { get; } = new()
    {
        new("", "", "", "", "", 1, 1, "") { Form = "口服" },
    };

    /// <summary>常用频次选项（展示含中文说明，保存时取冒号前编码）</summary>
    public IReadOnlyList<string> FreqOptions { get; } = new[]
    {
        "QD 每日1次", "BID 每日2次", "TID 每日3次", "QID 每日4次", "QN 睡前", "PRN 必要时",
    };

    // ===== Tab 5: 检验检查 =====

    [ObservableProperty]
    private List<LabOrderDto> labOrders = new();

    [ObservableProperty]
    private string newLabItemCode = string.Empty;

    [ObservableProperty]
    private string newLabItemName = string.Empty;

    [ObservableProperty]
    private DictionaryItemDto? selectedLabItem;

    // ===== 参考字典（门诊医生站引导数据） =====

    /// <summary>ICD-10 诊断字典项（Code=ICD 编码，Name=诊断名称）</summary>
    [ObservableProperty]
    private List<DictionaryItemDto> icdItems = new();

    /// <summary>常用药品字典项（Name 约定「药品名 | 规格 | 剂型」）</summary>
    [ObservableProperty]
    private List<DictionaryItemDto> drugOptions = new();

    /// <summary>检验检查项目字典项（Code=项目编码，Name=项目名称）</summary>
    [ObservableProperty]
    private List<DictionaryItemDto> labItems = new();

    // ===== 初始化 =====

    public async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            await Task.WhenAll(LoadQueueAsync(), LoadReferenceDictionariesAsync());
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

    private async Task LoadReferenceDictionariesAsync()
    {
        try
        {
            var icdTask = _dictService.GetItemsByTypeCodeAsync("ICD_10");
            var drugTask = _dictService.GetItemsByTypeCodeAsync("DRUG");
            var labTask = _dictService.GetItemsByTypeCodeAsync("LAB_ITEM");
            await Task.WhenAll(icdTask, drugTask, labTask);
            IcdItems = icdTask.Result;
            DrugOptions = drugTask.Result;
            LabItems = labTask.Result;
        }
        catch
        {
            // 参考字典加载失败不影响主流程，保持空列表（表单仍可手动输入）
            IcdItems = new();
            DrugOptions = new();
            LabItems = new();
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
            // 使用当前登录医生的 ID（从登录上下文获取）
            var doctorId = _appContext.CurrentUserId;
            var items = await _encounterService.GetQueueAsync(doctorId, TodayDate);
            QueueRows = new ObservableCollection<QueueRow>(items.Select(i => new QueueRow(i)));
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"加载队列失败: {ex.Message}";
            QueueRows = new();
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "加载队列超时，请重试";
            QueueRows = new();
        }
    }

    // ===== 选择患者 =====

    [RelayCommand]
    private async Task SelectPatient(QueueRow? row)
    {
        if (row is null) return;

        SelectPatientCore(row);

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

    /// <summary>呼叫就诊人（模拟场景）：仅标记「已呼叫」并提示，不改变接诊状态</summary>
    [RelayCommand]
    private void CallPatient(QueueRow? row)
    {
        if (row is null) return;

        row.IsCalled = true;
        _notifications.Success($"已呼叫 {row.PatientName}（排队号 {row.QueueNumber}），请到诊室候诊");
    }

    [RelayCommand]
    private async Task StartConsultation(QueueRow? row)
    {
        if (row is null)
        {
            // 顶栏按钮：按当前选中患者接诊
            if (_currentEncounterId == 0) return;
        }
        else
        {
            // 队列每行按钮：按就诊人接诊，同时选中并载入该患者数据
            SelectPatientCore(row);
            SelectedTabIndex = 1; // 开始接诊后自动进入病历 Tab
        }
        ErrorMessage = null;

        try
        {
            await _encounterService.StartConsultationAsync(_currentEncounterId);
            _notifications.Success("接诊开始");
            await LoadQueueAsync();
            // 更新选中项状态
            if (SelectedQueueItem is not null)
            {
                var updated = QueueRows.FirstOrDefault(q => q.Id == _currentEncounterId);
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
    private async Task CompleteConsultation(QueueRow? row)
    {
        if (row is not null)
        {
            // 队列每行按钮：按就诊人完成接诊
            SelectPatientCore(row);
        }
        if (_currentEncounterId == 0) return;
        ErrorMessage = null;

        try
        {
            await _encounterService.CompleteConsultationAsync(_currentEncounterId);
            _notifications.Success("就诊完成");
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

    /// <summary>选中就诊人：记录 Id、更新选中项与顶栏信息</summary>
    private void SelectPatientCore(QueueRow row)
    {
        SelectedQueueItem = row;
        _currentEncounterId = row.Id;
        SelectedPatientInfo = $"{row.PatientName} | 排队号: {row.QueueNumber} | {row.SlotName}";
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
            _notifications.Success("病历已保存");
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
            _notifications.Success("病历已提交");
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

    /// <summary>从常用诊断下拉选择：编码与描述一次填好</summary>
    partial void OnSelectedIcdItemChanged(DictionaryItemDto? value)
    {
        if (value is null) return;
        NewIcdCode = value.Code;
        NewDiagnosisDesc = value.Name;
    }

    /// <summary>输入诊断描述时按包含关系自动匹配 ICD 编码（取匹配到的最长名称），无需手查编码</summary>
    partial void OnNewDiagnosisDescChanged(string value)
    {
        var text = value?.Trim() ?? string.Empty;
        if (text.Length == 0)
        {
            if (SelectedIcdItem is null && NewIcdCode.Length > 0)
                NewIcdCode = string.Empty;
            return;
        }

        // 用户手动编辑描述时清除下拉选中，避免下拉回弹覆盖手工输入
        if (SelectedIcdItem is not null && SelectedIcdItem.Name != value)
            SelectedIcdItem = null;

        var match = IcdItems
            .Where(i => !string.IsNullOrEmpty(i.Name) && i.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(i => i.Name.Length)
            .FirstOrDefault();
        NewIcdCode = match is null ? string.Empty : match.Code;
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
            _notifications.Success("诊断已添加");
            NewIcdCode = string.Empty;
            NewDiagnosisDesc = string.Empty;
            SelectedIcdItem = null;
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
            _notifications.Success("诊断已删除");
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
        NewPrescriptionItems.Add(new("", "", "", "", "", 1, 1, "") { Form = "口服" });
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
            var items = NewPrescriptionItems.Select(i =>
            {
                // 频次下拉展示「BID 每日2次」，保存时取空格前的编码部分；未选下拉则用原文本
                var freq = string.IsNullOrWhiteSpace(i.SelectedFreq)
                    ? i.Freq
                    : i.SelectedFreq.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? i.Freq;
                return new CreatePrescriptionItemDto(
                    i.DrugName, i.Spec, i.Form, freq, i.Dosage, i.Duration, i.Qty,
                    string.IsNullOrWhiteSpace(i.Note) ? null : i.Note);
            }).ToList();

            var dto = new CreatePrescriptionDto(items);
            await _prescriptionService.CreateAsync(_currentEncounterId, _appContext.CurrentUserId, dto);
            _notifications.Success("处方已开具");
            NewPrescriptionItems.Clear();
            NewPrescriptionItems.Add(new("", "", "", "", "", 1, 1, "") { Form = "口服" });
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
            _notifications.Success("处方已作废");
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

    /// <summary>从项目下拉选择：编码与名称一次填好</summary>
    partial void OnSelectedLabItemChanged(DictionaryItemDto? value)
    {
        if (value is null) return;
        NewLabItemCode = value.Code;
        NewLabItemName = value.Name;
    }

    /// <summary>常用项目一键开立：按项目编码从字典取回后直接创建检验申请</summary>
    [RelayCommand]
    private async Task QuickAddLab(string? code)
    {
        if (_currentEncounterId == 0 || string.IsNullOrWhiteSpace(code)) return;
        var item = LabItems.FirstOrDefault(i => i.Code == code);
        if (item is null) return;
        ErrorMessage = null;

        try
        {
            var dto = new CreateLabOrderDto(item.Code, item.Name);
            await _labOrderService.CreateAsync(_currentEncounterId, dto);
            _notifications.Success($"已开立：{item.Name}");
            await LoadLabOrdersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"添加检验申请失败: {ex.Message}";
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
            _notifications.Success("检验已开具");
            NewLabItemCode = string.Empty;
            NewLabItemName = string.Empty;
            SelectedLabItem = null;
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
            _notifications.Success("检验已取消");
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

    /// <summary>选中的药品字典项（Name 约定「药品名 | 规格 | 剂型 | 频次 | 每次剂量 | 天数 | 总量」）</summary>
    [ObservableProperty]
    private DictionaryItemDto? selectedDrug;

    /// <summary>选中的用法频次（展示「BID 每日2次」，保存时取编码部分）</summary>
    [ObservableProperty]
    private string selectedFreq = string.Empty;

    /// <summary>频次编码 → 下拉展示文案</summary>
    private static readonly IReadOnlyDictionary<string, string> FreqDisplay = new Dictionary<string, string>
    {
        ["QD"] = "QD 每日1次",
        ["BID"] = "BID 每日2次",
        ["TID"] = "TID 每日3次",
        ["QID"] = "QID 每日4次",
        ["QN"] = "QN 睡前",
        ["PRN"] = "PRN 必要时",
    };

    /// <summary>选中药品后按「 | 」拆分，自动填充 药品名/规格/剂型 与默认用法（频次/剂量/天数/总量）</summary>
    partial void OnSelectedDrugChanged(DictionaryItemDto? value)
    {
        if (value is null) return;
        var parts = value.Name.Split('|').Select(p => p.Trim()).ToArray();
        if (parts.Length > 0 && parts[0].Length > 0)
            DrugName = parts[0];
        if (parts.Length > 1)
            Spec = parts[1];
        if (parts.Length > 2)
            Form = parts[2];
        if (parts.Length > 3 && parts[3].Length > 0)
        {
            var freqCode = parts[3];
            Freq = freqCode;
            SelectedFreq = FreqDisplay.TryGetValue(freqCode, out var display) ? display : freqCode;
        }
        if (parts.Length > 4 && parts[4].Length > 0)
            Dosage = parts[4];
        if (parts.Length > 5 && int.TryParse(parts[5], out var days) && days > 0)
            Duration = days;
        if (parts.Length > 6 && int.TryParse(parts[6], out var qty) && qty > 0)
            Qty = qty;
    }
}

/// <summary>
/// 队列行显示模型：包装 EncounterQueueItemDto，附加客户端「已呼叫」标记。
/// 呼叫为模拟场景，仅本次会话生效，不持久化；接诊状态仍由后端 encounter.Status 决定。
/// </summary>
public sealed partial class QueueRow : ObservableObject
{
    public QueueRow(EncounterQueueItemDto dto)
    {
        Dto = dto;
    }

    public EncounterQueueItemDto Dto { get; }

    public long Id => Dto.Id;
    public long PatientId => Dto.PatientId;
    public string PatientName => Dto.PatientName;
    public string PatientGender => Dto.PatientGender;
    public string PatientIdCard => Dto.PatientIdCard;
    public int QueueNumber => Dto.QueueNumber;
    public string SlotName => Dto.SlotName;
    public string Status => Dto.Status;
    public string RegisterTime => Dto.RegisterTime;

    /// <summary>是否已被呼叫（模拟呼叫标记）</summary>
    [ObservableProperty]
    private bool isCalled;
}

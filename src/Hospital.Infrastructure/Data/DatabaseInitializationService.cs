using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.Aggregates.Schedule;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Infrastructure.Data;

/// <summary>
/// 启动时初始化数据库（EnsureCreated 建表 + 种子患者数据）。
/// 初始化失败不阻断应用启动，由后台循环重试，直到 SQL Server 可用。
/// 避免 SQL 短暂不可用时整个 API 以 500.30 挂掉、且 SQL 恢复后需要手动回收应用池才能恢复的问题。
/// </summary>
public sealed class DatabaseInitializationService : BackgroundService
{
    private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseInitializationService> _logger;
    private int _initialized;
    private int _consecutiveFailures;

    public DatabaseInitializationService(
        IServiceScopeFactory scopeFactory,
        ILogger<DatabaseInitializationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>启动时尽力完成一次初始化；失败只记录日志，由后台任务继续重试。</summary>
    public Task EnsureDatabaseReadyAsync() => TryInitializeAsync();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await TryInitializeAsync())
                return;

            try
            {
                await Task.Delay(RetryInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private async Task<bool> TryInitializeAsync()
    {
        if (Volatile.Read(ref _initialized) == 1)
            return true;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

            db.Database.EnsureCreated();
            SeedPatientsIfEmpty(db);
            SeedDemoClinicalData(db);
            SeedReferenceDictionaries(db);

            Volatile.Write(ref _initialized, 1);
            _consecutiveFailures = 0;
            _logger.LogInformation("数据库初始化完成（EnsureCreated + 种子患者数据 + 演示临床链 + 参考字典）");
            return true;
        }
        catch (Exception ex)
        {
            _consecutiveFailures++;
            if (_consecutiveFailures == 1)
                _logger.LogWarning(ex, "数据库初始化失败：{Message}。每 {Interval}s 自动重试，直到 SQL Server 可用",
                    ex.Message, RetryInterval.TotalSeconds);
            else
                _logger.LogDebug(ex, "数据库初始化重试中（第 {Attempt} 次失败）：{Message}", _consecutiveFailures, ex.Message);
            return false;
        }
    }

    private static void SeedPatientsIfEmpty(HospitalDbContext db)
    {
        if (db.Patients.Any())
            return;

        db.Patients.AddRange(
            new Patient("P20250001", "张明", Gender.Male, new DateOnly(1990, 3, 15), new PhoneNumber("13800138001"), "青霉素过敏", new IdCard("110101199003151234")),
            new Patient("P20250002", "李芳", Gender.Female, new DateOnly(1985, 7, 20), new PhoneNumber("13800138002"), null, new IdCard("110101198507202345")),
            new Patient("P20250003", "王建国", Gender.Male, new DateOnly(1978, 11, 11), new PhoneNumber("13800138003"), "磺胺类过敏", new IdCard("110101197811113456")),
            new Patient("P20250004", "赵秀英", Gender.Female, new DateOnly(1992, 8, 8), new PhoneNumber("13800138004"), null, new IdCard("110101199208084567")),
            new Patient("P20250005", "刘浩然", Gender.Male, new DateOnly(2001, 5, 5), new PhoneNumber("13800138005"), null, new IdCard("110101200105055678")),
            new Patient("P20250006", "陈德明", Gender.Male, new DateOnly(1965, 12, 25), new PhoneNumber("13800138006"), "阿司匹林过敏", new IdCard("110101196512256789")),
            new Patient("P20250007", "杨雪", Gender.Female, new DateOnly(1995, 9, 15), new PhoneNumber("13800138007"), null, new IdCard("110101199509152345")),
            new Patient("P20250008", "黄海波", Gender.Male, new DateOnly(1982, 3, 30), new PhoneNumber("13800138008"), null, new IdCard("110101198203308901")),
            new Patient("P20250009", "周玉兰", Gender.Female, new DateOnly(1976, 9, 9), new PhoneNumber("13800138009"), "头孢类过敏", new IdCard("110101197609092345")),
            new Patient("P20250010", "吴磊", Gender.Male, new DateOnly(1998, 8, 18), new PhoneNumber("13800138010"), null, new IdCard("110101199808186789"))
        );
        db.SaveChanges();
    }

    /// <summary>
    /// 演示临床链种子：总院区 → 内科 → E0002 张医生 → 今日排班 → 张明已完成就诊
    /// （含诊断/处方/检验/放射），供患者360开箱即可演示。
    /// 整体幂等：主数据按存在性 find-or-create，临床链按患者挂号记录守卫，重复启动不产生重复数据。
    /// </summary>
    private static void SeedDemoClinicalData(HospitalDbContext db)
    {
        var campus = db.Campuses.FirstOrDefault(c => c.Code.Value == "ZONGYUAN");
        if (campus is null)
        {
            campus = new Campus(new CampusCode("ZONGYUAN"), "总院区", null, null);
            db.Campuses.Add(campus);
            db.SaveChanges();
        }

        var dept = db.Departments.FirstOrDefault(d => d.CampusId == campus.Id && d.Code.Value == "NK");
        if (dept is null)
        {
            dept = new Department(new DepartmentCode("NK"), "内科", campus.Id, DepartmentType.Clinical);
            db.Departments.Add(dept);
            db.SaveChanges();
        }

        var doctor = db.Staffs.FirstOrDefault(s => s.Code == "E0002");
        if (doctor is null)
        {
            doctor = new Staff("E0002", "张医生", Gender.Male, "13800138002", campus.Id, dept.Id,
                LicenseType.执业医师, new LicenseNumber("110101199001011234"), null);
            db.Staffs.Add(doctor);
            db.SaveChanges();
        }

        var patient = db.Patients.FirstOrDefault(p => p.PatientNo == "P20250001");
        // 患者不存在或该患者已有挂号记录时跳过，避免重复铺链
        if (patient is null || db.Registrations.Any(r => r.PatientId == patient.Id))
            return;

        var today = DateOnly.FromDateTime(DateTime.Now);
        var schedule = db.Schedules.FirstOrDefault(s => s.DoctorId == doctor.Id && s.ScheduleDate == today);
        ScheduleSlot slot;
        if (schedule is null)
        {
            var slots = new List<ScheduleSlot>
            {
                new(new TimeSlot("08:00-09:00", new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0)), 5),
                new(new TimeSlot("09:00-10:00", new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0)), 5),
                new(new TimeSlot("10:00-11:00", new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0)), 5),
                new(new TimeSlot("11:00-12:00", new TimeSpan(11, 0, 0), new TimeSpan(12, 0, 0)), 5),
                new(new TimeSlot("14:00-15:00", new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0)), 5),
                new(new TimeSlot("15:00-16:00", new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0)), 5),
                new(new TimeSlot("16:00-17:00", new TimeSpan(16, 0, 0), new TimeSpan(17, 0, 0)), 5),
            };
            schedule = new Schedule(doctor.Id, dept.Id, campus.Id, today, slots);
            db.Schedules.Add(schedule);
            db.SaveChanges();
            slot = slots[0];
        }
        else
        {
            slot = schedule.GetSlot("08:00-09:00") ?? schedule.Slots.First();
        }

        // Registration.ScheduleId 存的是 ScheduleSlot.Id（时段 Id），不是排班模板 Id
        // 注意：以下临床种子链为"演示用终态数据"，直接调用业务方法构造已完成就诊，
        // 不代表真实操作路径（真实操作见挂号/接诊/收费/发药应用服务）
        var registration = new Registration(patient.Id, slot.Id, doctor.Id, dept.Id, campus.Id, 1, "08:00-09:00");
        registration.MarkVisited();
        db.Registrations.Add(registration);
        db.SaveChanges();

        var encounter = new Encounter(patient.Id, doctor.Id, dept.Id, campus.Id, registration.Id);
        encounter.StartConsultation();
        encounter.CompleteConsultation();
        db.Encounters.Add(encounter);
        db.SaveChanges();

        db.Diagnoses.Add(new Diagnosis(encounter.Id, DiagnosisType.主要诊断, "I10.x05", "高血压病2级", isPrimary: true));
        db.Diagnoses.Add(new Diagnosis(encounter.Id, DiagnosisType.次要诊断, "I10.x05", "高血压", isPrimary: false));

        var prescription = new Prescription(encounter.Id, doctor.Id);
        prescription.Pay();
        prescription.Dispense();
        prescription.AddItem(new PrescriptionItem(0, "硝苯地平片", "10mg*100片", "片剂", "QD", "10mg", 30, 30, "口服"));
        db.Prescriptions.Add(prescription);

        db.LabOrders.Add(new LabOrder(encounter.Id, "CBC", "血常规"));
        db.LabOrders.Add(new LabOrder(encounter.Id, "LIVER_FUNC", "肝功能"));
        db.RadOrders.Add(new RadOrder(encounter.Id, "CHEST_XRAY", "胸部X线检查"));

        db.SaveChanges();
    }

    /// <summary>
    /// 门诊医生站参考字典种子：ICD_10 诊断、DRUG 常用药品、LAB_ITEM 检验检查项目。
    /// 供医生站在诊断/处方/检验 Tab 提供快捷选择、自动填充与占位示例。
    /// 幂等：类型与项均按编码 find-or-create，重复启动不会产生重复数据。
    /// 注：DRUG 项 Name 约定为「药品名 | 规格 | 剂型」，App 端按竖线拆分。
    /// </summary>
    private static void SeedReferenceDictionaries(HospitalDbContext db)
    {
        var icdType = EnsureDictionaryType(db, "ICD_10", "ICD-10 诊断", "门诊诊断常用 ICD-10 编码");
        SeedDictionaryItems(db, icdType.Id, new (string, string)[]
        {
            ("I10", "高血压病"),
            ("I10.x05", "高血压病2级"),
            ("I11", "高血压性心脏病"),
            ("I63.9", "脑梗死"),
            ("I25.1", "冠状动脉粥样硬化性心脏病"),
            ("I20.9", "心绞痛"),
            ("I48", "心房颤动"),
            ("E11.9", "2型糖尿病"),
            ("E14.9", "糖尿病"),
            ("J06.9", "急性上呼吸道感染"),
            ("J20.9", "急性支气管炎"),
            ("J18.9", "肺炎"),
            ("J45.9", "支气管哮喘"),
            ("J02.9", "急性咽炎"),
            ("J03.9", "急性扁桃体炎"),
            ("J32.9", "慢性鼻窦炎"),
            ("J40", "慢性支气管炎"),
            ("K35.9", "急性阑尾炎"),
            ("K29.5", "慢性胃炎"),
            ("K21.0", "胃食管反流病"),
            ("K52.9", "急性胃肠炎"),
            ("N39.0", "泌尿道感染"),
            ("N20.0", "肾结石"),
            ("M54.5", "腰痛"),
            ("M17.9", "膝骨关节炎"),
            ("M10.9", "痛风"),
            ("B18.1", "乙型病毒性肝炎"),
            ("A09", "感染性腹泻"),
            ("F32.9", "抑郁症"),
            ("G43.9", "偏头痛"),
            ("G40.9", "癫痫"),
            ("H10.9", "结膜炎"),
            ("R05", "咳嗽"),
            ("R50.9", "发热"),
            ("R42", "头晕"),
            ("Z00.0", "健康体检"),
        });

        var drugType = EnsureDictionaryType(db, "DRUG", "常用药品", "门诊常用药品（名称 | 规格 | 剂型 | 频次 | 每次剂量 | 天数 | 总量）");
        SeedDictionaryItems(db, drugType.Id, new (string, string)[]
        {
            ("AMOXIL", "阿莫西林胶囊 | 0.25g*24粒 | 胶囊剂 | BID | 1粒 | 7 | 14"),
            ("CEFIXIME", "头孢克肟片 | 50mg*12片 | 片剂 | BID | 1片 | 5 | 10"),
            ("IBUPROFEN", "布洛芬缓释胶囊 | 0.3g*20粒 | 缓释胶囊 | TID | 1粒 | 3 | 9"),
            ("NIFEDIPINE", "硝苯地平片 | 10mg*100片 | 片剂 | QD | 10mg | 30 | 30"),
            ("PARACETAMOL", "对乙酰氨基酚片 | 0.5g*20片 | 片剂 | PRN | 1片 | 3 | 6"),
            ("METFORMIN", "二甲双胍片 | 0.5g*20片 | 片剂 | BID | 1片 | 30 | 60"),
            ("OMEPRAZOLE", "奥美拉唑肠溶胶囊 | 20mg*14粒 | 肠溶胶囊 | QD | 1粒 | 14 | 14"),
            ("LOSARTAN", "氯沙坦钾片 | 50mg*7片 | 片剂 | QD | 1片 | 30 | 30"),
            ("ATORVASTATIN", "阿托伐他汀钙片 | 10mg*7片 | 片剂 | QN | 1片 | 30 | 30"),
            ("ASPIRIN", "阿司匹林肠溶片 | 0.1g*30片 | 肠溶片 | QD | 1片 | 30 | 30"),
            ("CLARITHROMYCIN", "克拉霉素片 | 0.25g*6片 | 片剂 | BID | 2片 | 7 | 28"),
            ("AMBROXOL", "盐酸氨溴索片 | 30mg*20片 | 片剂 | TID | 1片 | 5 | 15"),
            ("LORATADINE", "氯雷他定片 | 10mg*12片 | 片剂 | QD | 1片 | 7 | 7"),
            ("VITAMIN_C", "维生素C片 | 0.1g*100片 | 片剂 | QD | 1片 | 30 | 30"),
            ("AMPICILLIN", "氨苄西林胶囊 | 0.25g*24粒 | 胶囊剂 | QID | 2粒 | 7 | 56"),
        });

        var labType = EnsureDictionaryType(db, "LAB_ITEM", "检验检查项目", "常用检验检查项目");
        SeedDictionaryItems(db, labType.Id, new (string, string)[]
        {
            ("CBC", "血常规"),
            ("UAN", "尿常规"),
            ("FOB", "便常规"),
            ("LIVER_FUNC", "肝功能"),
            ("RENAL", "肾功能"),
            ("FBS", "空腹血糖"),
            ("LIPID", "血脂四项"),
            ("CRP", "C反应蛋白"),
            ("HBA1C", "糖化血红蛋白"),
            ("T3", "甲状腺功能三项"),
            ("ECG", "心电图"),
            ("CHEST_XRAY", "胸部X线检查"),
            ("ABDOMEN_US", "腹部彩超"),
            ("CARDIAC_US", "心脏彩超"),
            ("HEPATITIS_B", "乙肝两对半"),
        });

        db.SaveChanges();
    }

    /// <summary>按编码 find-or-create 字典类型，返回类型实体（保证 Id 可用）。</summary>
    private static DictionaryType EnsureDictionaryType(HospitalDbContext db, string code, string name, string? description)
    {
        var type = db.DictionaryTypes.FirstOrDefault(t => t.Code == code);
        if (type is null)
        {
            type = new DictionaryType(code, name, description);
            db.DictionaryTypes.Add(type);
            db.SaveChanges();
        }
        return type;
    }

    /// <summary>
    /// 按编码 find-or-create 字典项；已存在但内容（名称/排序）与种子不一致时更新，
    /// 便于种子约定调整后（如 DRUG 追加默认用法）已部署的库也能自愈。SortOrder 从 1 顺序递增。
    /// </summary>
    private static void SeedDictionaryItems(HospitalDbContext db, long typeId, (string Code, string Name)[] items)
    {
        var existingByCode = db.DictionaryItems
            .Where(i => i.TypeId == typeId)
            .ToDictionary(i => i.Code);
        var sortOrder = 1;
        foreach (var (code, name) in items)
        {
            if (existingByCode.TryGetValue(code, out var item))
            {
                if (item.Name != name || item.SortOrder != sortOrder)
                    item.UpdateInfo(name, null, sortOrder);
            }
            else
            {
                db.DictionaryItems.Add(new DictionaryItem(typeId, code, name, null, sortOrder));
            }
            sortOrder++;
        }
    }
}

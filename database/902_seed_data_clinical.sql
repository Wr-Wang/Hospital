/*
  902_seed_data_clinical.sql
  全量正式测试数据 - 临床部分：就诊、病历、检验、检查、处方、药品、发药。
  依赖：901_seed_data.sql 已执行完毕。
  可重复执行：使用 IF NOT EXISTS / MERGE 判断。
*/
USE [Hospital];
GO

SET NOCOUNT ON;

-- =========================================================================
-- 变量声明（独立运行需确保 901 已先执行）
-- =========================================================================
DECLARE @Campus1 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'ZONGYUAN');
DECLARE @StaffDoc1 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0002' AND CampusId = @Campus1);
DECLARE @StaffDoc2 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0003' AND CampusId = @Campus1);
DECLARE @StaffDoc3 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0004' AND CampusId = @Campus1);
DECLARE @StaffDoc4 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0005' AND CampusId = @Campus1);
DECLARE @StaffDoc5 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0009' AND CampusId = @Campus1);
DECLARE @StaffDoc6 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0010' AND CampusId = @Campus1);
DECLARE @StaffNurse1 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0006' AND CampusId = @Campus1);
DECLARE @Pat1 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250001');
DECLARE @Pat2 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250002');
DECLARE @Pat3 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250003');
DECLARE @Pat4 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250004');
DECLARE @Pat5 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250005');
DECLARE @Pat6 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250006');
DECLARE @Pat7 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250007');
DECLARE @Pat8 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250008');
DECLARE @Pat9 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250009');
DECLARE @Pat10 BIGINT = (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250010');
DECLARE @DeptXH BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'NK_XH' AND CampusId = @Campus1);
DECLARE @DeptHX BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'NK_HX' AND CampusId = @Campus1);
DECLARE @DeptPT BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'WK_PT' AND CampusId = @Campus1);
DECLARE @DeptGU BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'GUK' AND CampusId = @Campus1);
DECLARE @DeptEK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'EK' AND CampusId = @Campus1);
DECLARE @UserIdDoc1 BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'doctor1');
DECLARE @UserIdDoc2 BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'doctor2');

/* =========================================================================
   第五部分：就诊 (ENC)
   ========================================================================= */

/* ---------- enc.OutpatientEncounters ---------- */
-- For finished registrations (past days)
INSERT INTO enc.OutpatientEncounters (CampusId, PatientId, RegistrationId, DepartmentId, StaffId, Status, StartedAt, EndedAt)
SELECT
    @Campus1,
    r.PatientId,
    r.Id,
    CASE
        WHEN s.StaffId = @StaffDoc1 THEN @DeptXH
        WHEN s.StaffId = @StaffDoc2 THEN @DeptHX
        WHEN s.StaffId = @StaffDoc3 THEN @DeptPT
        WHEN s.StaffId = @StaffDoc4 THEN @DeptGU
        WHEN s.StaffId = @StaffDoc5 THEN (SELECT Id FROM mdm.Departments WHERE Code = N'FK_FC' AND CampusId = @Campus1)
        ELSE @DeptEK
    END,
    s.StaffId,
    N'Closed',
    DATEADD(MINUTE, 30, r.CreatedAt),
    DATEADD(MINUTE, 90, r.CreatedAt)
FROM opd.Registrations r
INNER JOIN opd.ScheduleSlots s ON r.SlotId = s.Id
WHERE r.CampusId = @Campus1 AND r.Status = N'Finished'
  AND NOT EXISTS (SELECT 1 FROM enc.OutpatientEncounters e WHERE e.RegistrationId = r.Id);

-- For today's registrations (status = Open)
INSERT INTO enc.OutpatientEncounters (CampusId, PatientId, RegistrationId, DepartmentId, StaffId, Status, StartedAt)
SELECT
    @Campus1,
    r.PatientId,
    r.Id,
    CASE
        WHEN s.StaffId = @StaffDoc1 THEN @DeptXH
        WHEN s.StaffId = @StaffDoc2 THEN @DeptHX
        WHEN s.StaffId = @StaffDoc3 THEN @DeptPT
        ELSE @DeptEK
    END,
    s.StaffId,
    N'Open',
    SYSUTCDATETIME()
FROM opd.Registrations r
INNER JOIN opd.ScheduleSlots s ON r.SlotId = s.Id
WHERE r.CampusId = @Campus1 AND r.Status = N'Registered'
  AND NOT EXISTS (SELECT 1 FROM enc.OutpatientEncounters e WHERE e.RegistrationId = r.Id);

/* ---------- enc.EmrDocuments ---------- */
-- Generate EMR for closed encounters
INSERT INTO enc.EmrDocuments (OutpatientEncounterId, DocType, ContentJson, SignedAt, CreatedAt)
SELECT
    e.Id,
    N'门诊病历',
    N'{"主诉":"胸闷气促1周","现病史":"患者近1周感胸闷、气促，活动后加重，无胸痛、心悸。","既往史":"高血压病史5年，规律服药。","体格检查":"BP 145/95mmHg，HR 88次/分，律齐，无杂音。","处理意见":"继续降压治疗，建议动态心电图检查。"}',
    DATEADD(MINUTE, 60, e.StartedAt),
    e.StartedAt
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc1
  AND NOT EXISTS (SELECT 1 FROM enc.EmrDocuments d WHERE d.OutpatientEncounterId = e.Id AND d.DocType = N'门诊病历');

INSERT INTO enc.EmrDocuments (OutpatientEncounterId, DocType, ContentJson, SignedAt, CreatedAt)
SELECT
    e.Id,
    N'门诊病历',
    N'{"主诉":"咳嗽咳痰3天","现病史":"患者3天前受凉后出现咳嗽、咳黄痰，伴发热38.5℃。","既往史":"体健。","体格检查":"T 38.2℃，双肺呼吸音粗，可闻及湿啰音。","处理意见":"抗感染治疗，必要时胸片检查。"}',
    DATEADD(MINUTE, 45, e.StartedAt),
    e.StartedAt
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc2
  AND NOT EXISTS (SELECT 1 FROM enc.EmrDocuments d WHERE d.OutpatientEncounterId = e.Id AND d.DocType = N'门诊病历');

INSERT INTO enc.EmrDocuments (OutpatientEncounterId, DocType, ContentJson, SignedAt, CreatedAt)
SELECT
    e.Id,
    N'门诊病历',
    N'{"主诉":"右下腹痛2天","现病史":"患者2天前出现右下腹持续性疼痛，伴恶心、无呕吐。","既往史":"体健。","体格检查":"McBurney点压痛(+)，反跳痛(+)。","处理意见":"建议血常规及腹部B超检查，考虑急性阑尾炎可能。"}',
    DATEADD(MINUTE, 40, e.StartedAt),
    e.StartedAt
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc3
  AND NOT EXISTS (SELECT 1 FROM enc.EmrDocuments d WHERE d.OutpatientEncounterId = e.Id AND d.DocType = N'门诊病历');

INSERT INTO enc.EmrDocuments (OutpatientEncounterId, DocType, ContentJson, SignedAt, CreatedAt)
SELECT
    e.Id,
    N'门诊病历',
    N'{"主诉":"腰痛伴左下肢放射痛1月","现病史":"患者1月前始感腰痛，久坐后加重，伴左下肢放射痛。","既往史":"体健。","体格检查":"腰椎压痛，直腿抬高试验(+)。","处理意见":"腰椎MRI检查，建议理疗。"}',
    DATEADD(MINUTE, 50, e.StartedAt),
    e.StartedAt
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc4
  AND NOT EXISTS (SELECT 1 FROM enc.EmrDocuments d WHERE d.OutpatientEncounterId = e.Id AND d.DocType = N'门诊病历');

/* ---------- enc.Diagnoses ---------- */
INSERT INTO enc.Diagnoses (OutpatientEncounterId, IcdCode, IcdName, DiagnosisType, IsPrimary)
SELECT e.Id, N'I10.x05', N'高血压病2级', N'OUTPATIENT', 1
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc1
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'I10.x05');

INSERT INTO enc.Diagnoses (OutpatientEncounterId, IcdCode, IcdName, DiagnosisType, IsPrimary)
SELECT e.Id, N'J15.901', N'细菌性肺炎', N'OUTPATIENT', 1
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc2
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'J15.901');

INSERT INTO enc.Diagnoses (OutpatientEncounterId, IcdCode, IcdName, DiagnosisType, IsPrimary)
SELECT e.Id, N'K35.900', N'急性阑尾炎', N'OUTPATIENT', 1
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc3
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'K35.900');

INSERT INTO enc.Diagnoses (OutpatientEncounterId, IcdCode, IcdName, DiagnosisType, IsPrimary)
SELECT e.Id, N'M51.104', N'腰椎间盘突出症', N'OUTPATIENT', 1
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc4
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'M51.104');

INSERT INTO enc.Diagnoses (OutpatientEncounterId, IcdCode, IcdName, DiagnosisType, IsPrimary)
SELECT e.Id, N'I10.x05', N'高血压', N'OUTPATIENT', 0
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId = @StaffDoc1
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'I10.x05' AND d.IsPrimary = 0);

/* =========================================================================
   第六部分：检验 (LAB)
   ========================================================================= */

/* ---------- lab.LabOrders ---------- */
INSERT INTO lab.LabOrders (CampusId, OutpatientEncounterId, OrderedByStaffId, Status, OrderedAt)
SELECT @Campus1, e.Id, e.StaffId, N'Ordered', DATEADD(MINUTE, 35, e.StartedAt)
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId IN (@StaffDoc1, @StaffDoc2, @StaffDoc3)
  AND NOT EXISTS (SELECT 1 FROM lab.LabOrders o WHERE o.OutpatientEncounterId = e.Id);

/* ---------- lab.LabOrderLines ---------- */
DECLARE @CBCItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'CBC' AND CampusId = @Campus1);
DECLARE @UrineItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'URINALYSIS' AND CampusId = @Campus1);
DECLARE @LiverItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'LIVER_FUNC' AND CampusId = @Campus1);
DECLARE @RenalItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'RENAL_FUNC' AND CampusId = @Campus1);
DECLARE @SugarItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'BLOOD_SUGAR' AND CampusId = @Campus1);

INSERT INTO lab.LabOrderLines (LabOrderId, ChargeItemId, ItemName, SpecimenType, Qty)
SELECT o.Id, @CBCItem, N'血常规', N'BLOOD', 1
FROM lab.LabOrders o
  WHERE NOT EXISTS (SELECT 1 FROM lab.LabOrderLines l WHERE l.LabOrderId = o.Id AND l.ChargeItemId = @CBCItem);

INSERT INTO lab.LabOrderLines (LabOrderId, ChargeItemId, ItemName, SpecimenType, Qty)
SELECT o.Id, @LiverItem, N'肝功能', N'BLOOD', 1
FROM lab.LabOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc1)
  AND NOT EXISTS (SELECT 1 FROM lab.LabOrderLines l WHERE l.LabOrderId = o.Id AND l.ChargeItemId = @LiverItem);

INSERT INTO lab.LabOrderLines (LabOrderId, ChargeItemId, ItemName, SpecimenType, Qty)
SELECT o.Id, @RenalItem, N'肾功能', N'BLOOD', 1
FROM lab.LabOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc1)
  AND NOT EXISTS (SELECT 1 FROM lab.LabOrderLines l WHERE l.LabOrderId = o.Id AND l.ChargeItemId = @RenalItem);

INSERT INTO lab.LabOrderLines (LabOrderId, ChargeItemId, ItemName, SpecimenType, Qty)
SELECT o.Id, @UrineItem, N'尿常规', N'URINE', 1
FROM lab.LabOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc3)
  AND NOT EXISTS (SELECT 1 FROM lab.LabOrderLines l WHERE l.LabOrderId = o.Id AND l.ChargeItemId = @UrineItem);

/* ---------- lab.SpecimenCollections ---------- */
INSERT INTO lab.SpecimenCollections (LabOrderId, CollectedAt, CollectorId, Status)
SELECT o.Id, DATEADD(HOUR, 1, o.OrderedAt), @StaffNurse1, N'Collected'
FROM lab.LabOrders o
WHERE o.Status = N'Ordered'
  AND NOT EXISTS (SELECT 1 FROM lab.SpecimenCollections c WHERE c.LabOrderId = o.Id);

/* =========================================================================
   第七部分：影像检查 (RAD)
   ========================================================================= */

/* ---------- rad.ImagingOrders ---------- */
DECLARE @ChestXrayItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'CHEST_XRAY' AND CampusId = @Campus1);
DECLARE @HeadCTItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'HEAD_CT' AND CampusId = @Campus1);
DECLARE @MRILumbarItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'MRI_LUMBAR' AND CampusId = @Campus1);
DECLARE @AbdomenCTItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'ABDOMEN_CT' AND CampusId = @Campus1);

INSERT INTO rad.ImagingOrders (CampusId, OutpatientEncounterId, OrderedByStaffId, Status, OrderedAt)
SELECT @Campus1, e.Id, e.StaffId, N'Ordered', DATEADD(MINUTE, 40, e.StartedAt)
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId IN (@StaffDoc2, @StaffDoc3, @StaffDoc4)
  AND NOT EXISTS (SELECT 1 FROM rad.ImagingOrders o WHERE o.OutpatientEncounterId = e.Id);

/* ---------- rad.ImagingOrderLines ---------- */
INSERT INTO rad.ImagingOrderLines (ImagingOrderId, ChargeItemId, BodyPart, Laterality, Qty)
SELECT o.Id, @ChestXrayItem, N'胸部', NULL, 1
FROM rad.ImagingOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc2)
  AND NOT EXISTS (SELECT 1 FROM rad.ImagingOrderLines l WHERE l.ImagingOrderId = o.Id);

INSERT INTO rad.ImagingOrderLines (ImagingOrderId, ChargeItemId, BodyPart, Laterality, Qty)
SELECT o.Id, @AbdomenCTItem, N'腹部', NULL, 1
FROM rad.ImagingOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc3)
  AND NOT EXISTS (SELECT 1 FROM rad.ImagingOrderLines l WHERE l.ImagingOrderId = o.Id);

INSERT INTO rad.ImagingOrderLines (ImagingOrderId, ChargeItemId, BodyPart, Laterality, Qty)
SELECT o.Id, @MRILumbarItem, N'腰椎', NULL, 1
FROM rad.ImagingOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc4)
  AND NOT EXISTS (SELECT 1 FROM rad.ImagingOrderLines l WHERE l.ImagingOrderId = o.Id);

/* ---------- rad.Appointments ---------- */
INSERT INTO rad.Appointments (ImagingOrderId, Modality, ScheduledAt, Status)
SELECT o.Id, N'CR', DATEADD(HOUR, 24, o.OrderedAt), N'Scheduled'
FROM rad.ImagingOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc2)
  AND NOT EXISTS (SELECT 1 FROM rad.Appointments a WHERE a.ImagingOrderId = o.Id);

INSERT INTO rad.Appointments (ImagingOrderId, Modality, ScheduledAt, Status)
SELECT o.Id, N'CT', DATEADD(HOUR, 48, o.OrderedAt), N'Scheduled'
FROM rad.ImagingOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc3)
  AND NOT EXISTS (SELECT 1 FROM rad.Appointments a WHERE a.ImagingOrderId = o.Id);

INSERT INTO rad.Appointments (ImagingOrderId, Modality, ScheduledAt, Status)
SELECT o.Id, N'MR', DATEADD(HOUR, 72, o.OrderedAt), N'Scheduled'
FROM rad.ImagingOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc4)
  AND NOT EXISTS (SELECT 1 FROM rad.Appointments a WHERE a.ImagingOrderId = o.Id);

/* ---------- rad.Reports ---------- */
INSERT INTO rad.Reports (ImagingOrderId, ReportNo, ReleasedAt)
SELECT o.Id, N'RPT' + FORMAT(CAST(o.OrderedAt AS DATE), N'yyyyMMdd') + CAST(o.Id AS NVARCHAR), DATEADD(HOUR, 48, o.OrderedAt)
FROM rad.ImagingOrders o
WHERE o.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc2)
  AND NOT EXISTS (SELECT 1 FROM rad.Reports r WHERE r.ImagingOrderId = o.Id);

/* =========================================================================
   第八部分：药品 (PHA)
   ========================================================================= */

/* ---------- pha.StorageLocations ---------- */
-- 中心药库已在 900 中创建
DECLARE @CentralStore BIGINT = (SELECT Id FROM pha.StorageLocations WHERE CampusId = @Campus1 AND Name = N'中心药库');

MERGE INTO pha.StorageLocations AS t
USING (VALUES
    (@Campus1, N'门诊药房', N'Retail'),
    (@Campus1, N'急诊药房', N'Emergency'),
    (@Campus1, N'住院药房', N'Inpatient'),
    (@Campus1, N'中药房',   N'TCM'),
    (@Campus2, N'东院药房', N'Central')
) AS s(CampusId, Name, LocationType)
ON t.CampusId = s.CampusId AND t.Name = s.Name
WHEN NOT MATCHED THEN INSERT (CampusId, Name, LocationType) VALUES (s.CampusId, s.Name, s.LocationType);

DECLARE @OutpatientPharm BIGINT = (SELECT Id FROM pha.StorageLocations WHERE CampusId = @Campus1 AND Name = N'门诊药房');
DECLARE @InpatientPharm BIGINT = (SELECT Id FROM pha.StorageLocations WHERE CampusId = @Campus1 AND Name = N'住院药房');

/* ---------- pha.Drugs ---------- */
MERGE INTO pha.Drugs AS t
USING (VALUES
    (@Campus1, N'DRG_001', N'阿莫西林胶囊',       N'0.25g*24粒',   N'盒', 0),
    (@Campus1, N'DRG_002', N'头孢克肟片',         N'50mg*12片',    N'盒', 0),
    (@Campus1, N'DRG_003', N'布洛芬缓释胶囊',     N'0.3g*20粒',    N'盒', 0),
    (@Campus1, N'DRG_004', N'硝苯地平片',         N'10mg*100片',   N'瓶', 0),
    (@Campus1, N'DRG_005', N'阿托伐他汀钙片',     N'20mg*7片',     N'盒', 0),
    (@Campus1, N'DRG_006', N'盐酸二甲双胍片',     N'0.5g*20片',    N'盒', 0),
    (@Campus1, N'DRG_007', N'奥美拉唑肠溶胶囊',   N'20mg*14粒',    N'盒', 0),
    (@Campus1, N'DRG_008', N'氯沙坦钾片',         N'50mg*7片',     N'盒', 0),
    (@Campus1, N'DRG_009', N'左氧氟沙星片',       N'0.5g*7片',     N'盒', 0),
    (@Campus1, N'DRG_010', N'盐酸氨溴索片',       N'30mg*20片',    N'盒', 0),
    (@Campus1, N'DRG_011', N'阿司匹林肠溶片',     N'100mg*30片',   N'瓶', 0),
    (@Campus1, N'DRG_012', N'盐酸小檗碱片',       N'0.1g*100片',   N'瓶', 0),
    (@Campus1, N'DRG_013', N'地西泮注射液',       N'10mg*10支',    N'盒', 1),  -- 管控药品
    (@Campus1, N'DRG_014', N'盐酸哌替啶注射液',   N'100mg*5支',    N'盒', 1),  -- 管控药品
    (@Campus1, N'DRG_015', N'注射用青霉素钠',     N'160万U*10瓶',  N'盒', 0),
    (@Campus1, N'DRG_016', N'葡萄糖注射液(5%)',   N'250ml/袋',     N'袋', 0),
    (@Campus1, N'DRG_017', N'氯化钠注射液(0.9%)', N'500ml/瓶',     N'瓶', 0),
    (@Campus1, N'DRG_018', N'胰岛素注射液',       N'400U/10ml',    N'支', 0),
    (@Campus1, N'DRG_019', N'蒙脱石散',           N'3g*10袋',      N'盒', 0),
    (@Campus1, N'DRG_020', N'盐酸雷尼替丁胶囊',   N'0.15g*30粒',   N'瓶', 0)
) AS s(CampusId, Code, Name, Spec, Unit, IsControlled)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, Code, Name, Spec, Unit, IsControlled)
    VALUES (s.CampusId, s.Code, s.Name, s.Spec, s.Unit, s.IsControlled);

DECLARE @DrugAmox   BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_001' AND CampusId = @Campus1);
DECLARE @DrugCef    BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_002' AND CampusId = @Campus1);
DECLARE @DrugIbu    BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_003' AND CampusId = @Campus1);
DECLARE @DrugNif    BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_004' AND CampusId = @Campus1);
DECLARE @DrugAtor   BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_005' AND CampusId = @Campus1);
DECLARE @DrugMetf   BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_006' AND CampusId = @Campus1);
DECLARE @DrugOme    BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_007' AND CampusId = @Campus1);
DECLARE @DrugLos    BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_008' AND CampusId = @Campus1);
DECLARE @DrugLevo   BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_009' AND CampusId = @Campus1);
DECLARE @DrugAmbro  BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_010' AND CampusId = @Campus1);
DECLARE @DrugDiaz   BIGINT = (SELECT Id FROM pha.Drugs WHERE Code = N'DRG_013' AND CampusId = @Campus1);

/* ---------- pha.DrugBatches ---------- */
MERGE INTO pha.DrugBatches AS t
USING (VALUES
    (@DrugAmox,  @Campus1, N'20250601', DATEADD(YEAR, 2, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugAmox,  @Campus1, N'20250602', DATEADD(YEAR, 2, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugCef,   @Campus1, N'20250501', DATEADD(YEAR, 1, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugIbu,   @Campus1, N'20250601', DATEADD(YEAR, 3, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugNif,   @Campus1, N'20250501', DATEADD(YEAR, 2, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugAtor,  @Campus1, N'20250601', DATEADD(YEAR, 2, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugMetf,  @Campus1, N'20250615', DATEADD(YEAR, 2, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugOme,   @Campus1, N'20250501', DATEADD(YEAR, 1, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugLos,   @Campus1, N'20250601', DATEADD(YEAR, 2, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugLevo,  @Campus1, N'20250501', DATEADD(YEAR, 1, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugAmbro, @Campus1, N'20250601', DATEADD(YEAR, 2, CAST(SYSUTCDATETIME() AS DATE))),
    (@DrugDiaz,  @Campus1, N'20250501', DATEADD(YEAR, 3, CAST(SYSUTCDATETIME() AS DATE)))
) AS s(DrugId, CampusId, BatchNo, ExpiryDate)
ON t.DrugId = s.DrugId AND t.CampusId = s.CampusId AND t.BatchNo = s.BatchNo
WHEN NOT MATCHED THEN INSERT (DrugId, CampusId, BatchNo, ExpiryDate) VALUES (s.DrugId, s.CampusId, s.BatchNo, s.ExpiryDate);

/* ---------- pha.InventoryLots ---------- */
DECLARE @BatchAmox1 BIGINT = (SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugAmox AND BatchNo = N'20250601' ORDER BY Id);
DECLARE @BatchAmox2 BIGINT = (SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugAmox AND BatchNo = N'20250602' ORDER BY Id);

MERGE INTO pha.InventoryLots AS t
USING (VALUES
    (@BatchAmox1, @CentralStore,     500),
    (@BatchAmox1, @OutpatientPharm,  200),
    (@BatchAmox2, @CentralStore,     300),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugCef ORDER BY Id), @CentralStore, 400),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugIbu ORDER BY Id), @OutpatientPharm, 150),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugNif ORDER BY Id), @CentralStore, 1000),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugAtor ORDER BY Id), @CentralStore, 300),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugMetf ORDER BY Id), @OutpatientPharm, 200),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugOme ORDER BY Id), @CentralStore, 400),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugLos ORDER BY Id), @OutpatientPharm, 250),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugLevo ORDER BY Id), @CentralStore, 300),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugAmbro ORDER BY Id), @OutpatientPharm, 180),
    ((SELECT TOP 1 Id FROM pha.DrugBatches WHERE DrugId = @DrugDiaz ORDER BY Id), @CentralStore, 50)
) AS s(DrugBatchId, StorageLocationId, QtyOnHand)
ON t.DrugBatchId = s.DrugBatchId AND t.StorageLocationId = s.StorageLocationId
WHEN NOT MATCHED THEN INSERT (DrugBatchId, StorageLocationId, QtyOnHand) VALUES (s.DrugBatchId, s.StorageLocationId, s.QtyOnHand);

/* ---------- pha.InventoryTransactions ---------- */
DECLARE @UserIdPharm BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'admin');
INSERT INTO pha.InventoryTransactions (InventoryLotId, TxnType, Qty, RefDocNo, CreatedByUserId)
SELECT Id, N'In', QtyOnHand, N'INIT_STOCK', @UserIdPharm
FROM pha.InventoryLots
WHERE NOT EXISTS (SELECT 1 FROM pha.InventoryTransactions t WHERE t.InventoryLotId = pha.InventoryLots.Id AND t.TxnType = N'In');

/* =========================================================================
   第九部分：处方 (PHA PRESCRIPTIONS)
   ========================================================================= */

/* ---------- pha.Prescriptions ---------- */
INSERT INTO pha.Prescriptions (CampusId, OutpatientEncounterId, PrescriptionNo, Status, PrescribedByStaffId, PrescribedAt)
SELECT
    @Campus1,
    e.Id,
    N'RX' + FORMAT(CAST(e.StartedAt AS DATE), N'yyyyMMdd') + CAST(ROW_NUMBER() OVER (ORDER BY e.Id) AS NVARCHAR(10)),
    N'Active',
    e.StaffId,
    DATEADD(MINUTE, 45, e.StartedAt)
FROM enc.OutpatientEncounters e
WHERE e.Status = N'Closed' AND e.StaffId IN (@StaffDoc1, @StaffDoc2, @StaffDoc3, @StaffDoc4)
  AND NOT EXISTS (SELECT 1 FROM pha.Prescriptions p WHERE p.OutpatientEncounterId = e.Id);

/* ---------- pha.PrescriptionLines ---------- */
-- 高血压患者处方：硝苯地平 + 阿托伐他汀
INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugId, Dose, Frequency, Days, Route, Qty)
SELECT p.Id, @DrugNif, N'10mg', N'QD', 30, N'PO', 30
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc1)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugId = @DrugNif);

INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugId, Dose, Frequency, Days, Route, Qty)
SELECT p.Id, @DrugAtor, N'20mg', N'QN', 30, N'PO', 30
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc1)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugId = @DrugAtor);

-- 肺炎患者处方：阿莫西林 + 氨溴索
INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugId, Dose, Frequency, Days, Route, Qty)
SELECT p.Id, @DrugAmox, N'0.5g', N'TID', 7, N'PO', 42
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc2)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugId = @DrugAmox);

INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugId, Dose, Frequency, Days, Route, Qty)
SELECT p.Id, @DrugAmbro, N'30mg', N'TID', 7, N'PO', 42
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc2)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugId = @DrugAmbro);

-- 阑尾炎处方：头孢克肟 + 奥美拉唑
INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugId, Dose, Frequency, Days, Route, Qty)
SELECT p.Id, @DrugCef, N'100mg', N'BID', 5, N'PO', 10
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc3)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugId = @DrugCef);

INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugId, Dose, Frequency, Days, Route, Qty)
SELECT p.Id, @DrugOme, N'20mg', N'QD', 14, N'PO', 14
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc3)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugId = @DrugOme);

-- 腰椎间盘突出处方：布洛芬
INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugId, Dose, Frequency, Days, Route, Qty)
SELECT p.Id, @DrugIbu, N'0.3g', N'BID', 14, N'PO', 28
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc4)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugId = @DrugIbu);

/* =========================================================================
   第十部分：发药 (PHA DISPENSE)
   ========================================================================= */

/* ---------- pha.Dispenses ---------- */
INSERT INTO pha.Dispenses (CampusId, PrescriptionId, Status, CreatedAt)
SELECT @Campus1, p.Id, N'Pending', p.PrescribedAt
FROM pha.Prescriptions p
WHERE NOT EXISTS (SELECT 1 FROM pha.Dispenses d WHERE d.PrescriptionId = p.Id);

/* ---------- pha.DispenseLines ---------- */
INSERT INTO pha.DispenseLines (DispenseId, PrescriptionLineId, InventoryLotId, Qty)
SELECT d.Id, pl.Id,
    (SELECT TOP 1 Id FROM pha.InventoryLots WHERE StorageLocationId = @OutpatientPharm AND DrugBatchId IN (SELECT Id FROM pha.DrugBatches WHERE DrugId = pl.DrugId) ORDER BY Id),
    pl.Qty
FROM pha.Dispenses d
INNER JOIN pha.PrescriptionLines pl ON d.PrescriptionId = pl.PrescriptionId
WHERE NOT EXISTS (SELECT 1 FROM pha.DispenseLines dl WHERE dl.DispenseId = d.Id AND dl.PrescriptionLineId = pl.Id)
  AND (SELECT TOP 1 Id FROM pha.InventoryLots WHERE StorageLocationId = @OutpatientPharm AND DrugBatchId IN (SELECT Id FROM pha.DrugBatches WHERE DrugId = pl.DrugId) ORDER BY Id) IS NOT NULL;

/* =========================================================================
   第十一部分：管控药品核对 (部分处方涉及)
   ========================================================================= */

/* ---------- pha.ControlledDrugWitness ---------- */
-- 仅管控药品发药需要核对
INSERT INTO pha.ControlledDrugWitness (DispenseLineId, WitnessUserId, WitnessedAt)
SELECT dl.Id, @UserIdPharm, SYSUTCDATETIME()
FROM pha.DispenseLines dl
INNER JOIN pha.Dispenses d ON dl.DispenseId = d.Id
INNER JOIN pha.PrescriptionLines pl ON dl.PrescriptionLineId = pl.Id
INNER JOIN pha.Drugs dr ON pl.DrugId = dr.Id
WHERE dr.IsControlled = 1
  AND NOT EXISTS (SELECT 1 FROM pha.ControlledDrugWitness w WHERE w.DispenseLineId = dl.Id);

/* =========================================================================
   第十二部分：生命体征 (MON)
   ========================================================================= */

/* ---------- mon.VitalSignSets ---------- */
INSERT INTO mon.VitalSignSets (OutpatientEncounterId, RecordedAt, Source)
SELECT e.Id, DATEADD(MINUTE, 5, e.StartedAt), N'Manual'
FROM enc.OutpatientEncounters e
WHERE e.Status IN (N'Open', N'Closed')
  AND NOT EXISTS (SELECT 1 FROM mon.VitalSignSets v WHERE v.OutpatientEncounterId = e.Id);

/* ---------- mon.VitalSignItems ---------- */
INSERT INTO mon.VitalSignItems (SetId, Code, Value, Unit)
SELECT v.Id, N'T',  N'36.5', N'℃' FROM mon.VitalSignSets v
WHERE NOT EXISTS (SELECT 1 FROM mon.VitalSignItems i WHERE i.SetId = v.Id AND i.Code = N'T')
UNION ALL
SELECT v.Id, N'PR', N'78',   N'bpm' FROM mon.VitalSignSets v
WHERE NOT EXISTS (SELECT 1 FROM mon.VitalSignItems i WHERE i.SetId = v.Id AND i.Code = N'PR')
UNION ALL
SELECT v.Id, N'RR', N'18',   N'/min' FROM mon.VitalSignSets v
WHERE NOT EXISTS (SELECT 1 FROM mon.VitalSignItems i WHERE i.SetId = v.Id AND i.Code = N'RR')
UNION ALL
SELECT v.Id, N'BP', N'135/85', N'mmHg' FROM mon.VitalSignSets v
WHERE NOT EXISTS (SELECT 1 FROM mon.VitalSignItems i WHERE i.SetId = v.Id AND i.Code = N'BP')
UNION ALL
SELECT v.Id, N'SpO2', N'98', N'%' FROM mon.VitalSignSets v
WHERE NOT EXISTS (SELECT 1 FROM mon.VitalSignItems i WHERE i.SetId = v.Id AND i.Code = N'SpO2');

/* ---------- mon.CriticalValues ---------- */
INSERT INTO mon.CriticalValues (SourceSystem, RefId, PatientId, AcknowledgedAt, ClosedAt)
SELECT N'LIS', N'LAB_REF_001', @Pat1, DATEADD(MINUTE, 15, SYSUTCDATETIME()), DATEADD(HOUR, 2, SYSUTCDATETIME())
WHERE NOT EXISTS (SELECT 1 FROM mon.CriticalValues WHERE SourceSystem = N'LIS' AND RefId = N'LAB_REF_001')
UNION ALL
SELECT N'LIS', N'LAB_REF_002', @Pat2, DATEADD(MINUTE, 10, SYSUTCDATETIME()), DATEADD(HOUR, 1, SYSUTCDATETIME())
WHERE NOT EXISTS (SELECT 1 FROM mon.CriticalValues WHERE SourceSystem = N'LIS' AND RefId = N'LAB_REF_002')
UNION ALL
SELECT N'PACS', N'IMG_REF_001', @Pat3, DATEADD(MINUTE, 30, SYSUTCDATETIME()), DATEADD(HOUR, 3, SYSUTCDATETIME())
WHERE NOT EXISTS (SELECT 1 FROM mon.CriticalValues WHERE SourceSystem = N'PACS' AND RefId = N'IMG_REF_001');

/* ---------- mon.RemoteDevices ---------- */
INSERT INTO mon.RemoteDevices (PatientId, DeviceUid)
SELECT @Pat1, N'DEVICE_HB_001'
WHERE NOT EXISTS (SELECT 1 FROM mon.RemoteDevices WHERE DeviceUid = N'DEVICE_HB_001')
UNION ALL
SELECT @Pat6, N'DEVICE_HB_002'
WHERE NOT EXISTS (SELECT 1 FROM mon.RemoteDevices WHERE DeviceUid = N'DEVICE_HB_002')
UNION ALL
SELECT @Pat8, N'DEVICE_HB_003'
WHERE NOT EXISTS (SELECT 1 FROM mon.RemoteDevices WHERE DeviceUid = N'DEVICE_HB_003');

PRINT N'902_seed_data_clinical.sql 执行完成。';
GO

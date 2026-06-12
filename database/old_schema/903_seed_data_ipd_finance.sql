/*
  903_seed_data_ipd_finance.sql
  全量正式测试数据 - 住院、设备、财务结算、报表。
  依赖：901_seed_data.sql + 902_seed_data_clinical.sql 已执行完毕。
  可重复执行：使用 IF NOT EXISTS / MERGE 判断。
*/
USE [Hospital];
GO

SET NOCOUNT ON;

-- =========================================================================
-- 变量声明（独立运行需确保 901+902 已先执行）
-- =========================================================================
DECLARE @Campus1 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'ZONGYUAN');
DECLARE @Campus2 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'DONGYUAN');
DECLARE @StaffDoc1 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0002' AND CampusId = @Campus1);
DECLARE @StaffDoc4 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0005' AND CampusId = @Campus1);
DECLARE @StaffNurse1 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0006' AND CampusId = @Campus1);
DECLARE @StaffCashier1 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0016' AND CampusId = @Campus1);
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
DECLARE @StaffDoc6 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0010' AND CampusId = @Campus1);
DECLARE @RegFeeItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'REG_FEE' AND CampusId = @Campus1);
DECLARE @UserIdAdmin BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'admin');
DECLARE @UserIdCashier1 BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'cashier1');

/* =========================================================================
   第十三部分：住院 (IPD)
   ========================================================================= */

/* ---------- ipd.Admissions ---------- */
MERGE INTO ipd.Admissions AS t
USING (VALUES
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250001'), N'IP20250001',
     (SELECT Id FROM mdm.Departments WHERE Code = N'NK_XH' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'01' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_NK_XH')), N'InHospital'),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250003'), N'IP20250002',
     (SELECT Id FROM mdm.Departments WHERE Code = N'WK_PT' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'02' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_WK_PT')), N'InHospital'),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250006'), N'IP20250003',
     (SELECT Id FROM mdm.Departments WHERE Code = N'GUK' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'03' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_GUK')), N'InHospital'),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250008'), N'IP20250004',
     (SELECT Id FROM mdm.Departments WHERE Code = N'NK_XH' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'04' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_NK_XH')), N'InHospital'),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250010'), N'IP20250005',
     (SELECT Id FROM mdm.Departments WHERE Code = N'EK' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'05' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_EK')), N'InHospital'),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250002'), N'IP20250006',
     (SELECT Id FROM mdm.Departments WHERE Code = N'FK_FC' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'06' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_FK')), N'InHospital'),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250004'), N'IP20250007',
     (SELECT Id FROM mdm.Departments WHERE Code = N'WK_PT' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'07' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_WK_PT')), N'Discharged',
     DATEADD(DAY, -5, SYSUTCDATETIME()), DATEADD(DAY, -2, SYSUTCDATETIME())),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250007'), N'IP20250008',
     (SELECT Id FROM mdm.Departments WHERE Code = N'NK_HX' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'08' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_NK_XH')), N'Discharged',
     DATEADD(DAY, -10, SYSUTCDATETIME()), DATEADD(DAY, -3, SYSUTCDATETIME())),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250009'), N'IP20250009',
     (SELECT Id FROM mdm.Departments WHERE Code = N'FK_FC' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'09' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_FK')), N'InHospital'),
    (@Campus1, (SELECT Id FROM pat.Patients WHERE PatientNo = N'P20250005'), N'IP20250010',
     (SELECT Id FROM mdm.Departments WHERE Code = N'GUK' AND CampusId = @Campus1),
     (SELECT Id FROM mdm.Beds WHERE BedNo = N'10' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_GUK')), N'InHospital')
) AS s(CampusId, PatientId, AdmissionNo, DepartmentId, BedId, Status, AdmittedAt, DischargedAt)
ON t.CampusId = s.CampusId AND t.AdmissionNo = s.AdmissionNo
WHEN NOT MATCHED THEN INSERT (CampusId, PatientId, AdmissionNo, DepartmentId, BedId, Status, AdmittedAt, DischargedAt)
    VALUES (s.CampusId, s.PatientId, s.AdmissionNo, s.DepartmentId, s.BedId, s.Status,
            ISNULL(s.AdmittedAt, SYSUTCDATETIME()), s.DischargedAt);

-- Update bed status for occupied beds
UPDATE mdm.Beds SET Status = N'Occupied'
WHERE Id IN (SELECT BedId FROM ipd.Admissions WHERE Status = N'InHospital' AND BedId IS NOT NULL);

/* ---------- ipd.AdmissionTransfers ---------- */
INSERT INTO ipd.AdmissionTransfers (AdmissionId, FromBedId, ToBedId, ToDepartmentId, TransferredAt)
SELECT a.Id,
    NULL,
    a.BedId,
    a.DepartmentId,
    a.AdmittedAt
FROM ipd.Admissions a
WHERE NOT EXISTS (SELECT 1 FROM ipd.AdmissionTransfers t WHERE t.AdmissionId = a.Id AND t.ToBedId = a.BedId);

-- Add a transfer record for a discharged patient
INSERT INTO ipd.AdmissionTransfers (AdmissionId, FromBedId, ToBedId, ToDepartmentId)
SELECT a.Id,
    (SELECT Id FROM mdm.Beds WHERE BedNo = N'07' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_WK_PT')),
    (SELECT Id FROM mdm.Beds WHERE BedNo = N'11' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_WK_PT')),
    a.DepartmentId
FROM ipd.Admissions a WHERE a.AdmissionNo = N'IP20250007'
  AND NOT EXISTS (SELECT 1 FROM ipd.AdmissionTransfers t WHERE t.AdmissionId = a.Id AND t.ToBedId = (SELECT Id FROM mdm.Beds WHERE BedNo = N'11' AND WardId IN (SELECT Id FROM mdm.Wards WHERE Code = N'W_WK_PT')));

/* ---------- ipd.DepositTransactions ---------- */
INSERT INTO ipd.DepositTransactions (AdmissionId, Amount, TxnType, OccurredAt)
SELECT a.Id, 5000.00, N'Deposit', a.AdmittedAt
FROM ipd.Admissions a WHERE a.Status = N'InHospital'
  AND NOT EXISTS (SELECT 1 FROM ipd.DepositTransactions d WHERE d.AdmissionId = a.Id AND d.TxnType = N'Deposit');

INSERT INTO ipd.DepositTransactions (AdmissionId, Amount, TxnType)
SELECT a.Id, 2000.00, N'Deposit'
FROM ipd.Admissions a WHERE a.AdmissionNo IN (N'IP20250001', N'IP20250003', N'IP20250005')
  AND NOT EXISTS (SELECT 1 FROM ipd.DepositTransactions d WHERE d.AdmissionId = a.Id AND d.Amount = 2000.00);

/* ---------- ipd.InpatientOrders ---------- */
INSERT INTO ipd.InpatientOrders (AdmissionId, OrderType, Status, StartTime, OrderedByStaffId, CreatedAt)
SELECT a.Id, N'Medication', N'Active', a.AdmittedAt, @StaffDoc1, a.AdmittedAt
FROM ipd.Admissions a WHERE a.Status = N'InHospital' AND a.DepartmentId IN (SELECT Id FROM mdm.Departments WHERE DeptType = N'Clinical')
  AND NOT EXISTS (SELECT 1 FROM ipd.InpatientOrders o WHERE o.AdmissionId = a.Id AND o.OrderType = N'Medication');

INSERT INTO ipd.InpatientOrders (AdmissionId, OrderType, Status, StartTime, OrderedByStaffId, CreatedAt)
SELECT a.Id, N'Nursing', N'Active', a.AdmittedAt, @StaffDoc1, a.AdmittedAt
FROM ipd.Admissions a WHERE a.Status = N'InHospital'
  AND NOT EXISTS (SELECT 1 FROM ipd.InpatientOrders o WHERE o.AdmissionId = a.Id AND o.OrderType = N'Nursing');

INSERT INTO ipd.InpatientOrders (AdmissionId, OrderType, Status, StartTime, OrderedByStaffId, CreatedAt)
SELECT a.Id, N'LabTest', N'Stopped', a.AdmittedAt, @StaffDoc1, a.AdmittedAt
FROM ipd.Admissions a WHERE a.Status = N'Discharged'
  AND NOT EXISTS (SELECT 1 FROM ipd.InpatientOrders o WHERE o.AdmissionId = a.Id AND o.OrderType = N'LabTest');

/* ---------- ipd.InpatientOrderLines ---------- */
DECLARE @ChargeDrug003 BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'DRG_003');
DECLARE @ChargeDrug004 BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'DRG_004');
DECLARE @ChargeBedGen BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'BED_GEN');

INSERT INTO ipd.InpatientOrderLines (InpatientOrderId, LineNo, ItemText, ChargeItemId)
SELECT o.Id, 1, N'硝苯地平片 10mg 每日一次', @ChargeDrug004
FROM ipd.InpatientOrders o WHERE o.OrderType = N'Medication'
  AND NOT EXISTS (SELECT 1 FROM ipd.InpatientOrderLines l WHERE l.InpatientOrderId = o.Id AND l.ItemText LIKE N'%硝苯地平%');

INSERT INTO ipd.InpatientOrderLines (InpatientOrderId, LineNo, ItemText, ChargeItemId)
SELECT o.Id, 1, N'布洛芬缓释胶囊 0.3g 每日两次', @ChargeDrug003
FROM ipd.InpatientOrders o WHERE o.OrderType = N'Nursing'
  AND NOT EXISTS (SELECT 1 FROM ipd.InpatientOrderLines l WHERE l.InpatientOrderId = o.Id AND l.ItemText LIKE N'%布洛芬%');

INSERT INTO ipd.InpatientOrderLines (InpatientOrderId, LineNo, ItemText, ChargeItemId)
SELECT o.Id, 1, N'普通床位费', @ChargeBedGen
FROM ipd.InpatientOrders o
WHERE NOT EXISTS (SELECT 1 FROM ipd.InpatientOrderLines l WHERE l.InpatientOrderId = o.Id AND l.ItemText = N'普通床位费');

/* ---------- ipd.OrderExecutions ---------- */
INSERT INTO ipd.OrderExecutions (InpatientOrderId, ExecutedAt, ExecutorUserId, Barcode)
SELECT o.Id, DATEADD(HOUR, 1, o.CreatedAt), @UserIdAdmin, N'BARCODE_EXEC_' + CAST(o.Id AS NVARCHAR)
FROM ipd.InpatientOrders o
WHERE NOT EXISTS (SELECT 1 FROM ipd.OrderExecutions e WHERE e.InpatientOrderId = o.Id);

/* ---------- ipd.NursingRecords ---------- */
INSERT INTO ipd.NursingRecords (AdmissionId, RecordType, ContentJson, RecordedAt)
SELECT a.Id, N'护理评估',
    N'{"意识":"清醒","皮肤":"完整","饮食":"普食","活动":"自理","跌倒风险":"低"}',
    DATEADD(HOUR, 2, a.AdmittedAt)
FROM ipd.Admissions a WHERE a.Status = N'InHospital'
  AND NOT EXISTS (SELECT 1 FROM ipd.NursingRecords n WHERE n.AdmissionId = a.Id AND n.RecordType = N'护理评估');

INSERT INTO ipd.NursingRecords (AdmissionId, RecordType, ContentJson, RecordedAt)
SELECT a.Id, N'交班记录',
    N'{"病情":"平稳","特殊处理":"无","交班护士":"刘洋"}',
    DATEADD(HOUR, 8, a.AdmittedAt)
FROM ipd.Admissions a WHERE a.Status = N'InHospital'
  AND NOT EXISTS (SELECT 1 FROM ipd.NursingRecords n WHERE n.AdmissionId = a.Id AND n.RecordType = N'交班记录');

/* ---------- ipd.TemperatureSheetEntries ---------- */
DECLARE @ChartDate DATE;
DECLARE @AdmId BIGINT;
DECLARE temp_cursor CURSOR LOCAL FOR SELECT Id, CAST(AdmittedAt AS DATE) FROM ipd.Admissions WHERE Status = N'InHospital';
OPEN temp_cursor;
FETCH NEXT FROM temp_cursor INTO @AdmId, @ChartDate;
WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @DayCount INT = 0;
    WHILE @DayCount < 3 AND @ChartDate <= CAST(SYSUTCDATETIME() AS DATE)
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM ipd.TemperatureSheetEntries WHERE AdmissionId = @AdmId AND ChartDate = @ChartDate)
            INSERT INTO ipd.TemperatureSheetEntries (AdmissionId, ChartDate, PointsJson)
            VALUES (@AdmId, @ChartDate, N'[{"time":"06:00","temp":36.5,"pulse":78},{"time":"14:00","temp":36.8,"pulse":80}]');
        SET @ChartDate = DATEADD(DAY, 1, @ChartDate);
        SET @DayCount = @DayCount + 1;
    END
    FETCH NEXT FROM temp_cursor INTO @AdmId, @ChartDate;
END
CLOSE temp_cursor;
DEALLOCATE temp_cursor;

/* =========================================================================
   第十四部分：设备资产 (EQP)
   ========================================================================= */

/* ---------- eqp.Assets ---------- */
MERGE INTO eqp.Assets AS t
USING (VALUES
    (@Campus1, N'EQP-CT-001',  N'GE 64排CT机',       N'影像设备', N'GE Medical', N'Revolution CT',  N'InUse'),
    (@Campus1, N'EQP-MR-001',  N'Siemens 3.0T MRI',  N'影像设备', N'Siemens',     N'MAGNETOM Skyra', N'InUse'),
    (@Campus1, N'EQP-DR-001',  N'飞利浦DR机',        N'影像设备', N'Philips',     N'DigitalDiagnost',N'InUse'),
    (@Campus1, N'EQP-US-001',  N'GE彩超诊断仪',      N'超声设备', N'GE Medical',  N'LOGIQ E9',       N'InUse'),
    (@Campus1, N'EQP-ECG-001', N'12导联心电图机',    N'心电设备', N'福田电子',    N'FX-7402',        N'InUse'),
    (@Campus1, N'EQP-LAB-001', N'全自动生化分析仪',  N'检验设备', N'Beckman',     N'AU5800',          N'InUse'),
    (@Campus1, N'EQP-LAB-002', N'血常规分析仪',      N'检验设备', N'Sysmex',      N'XN-9000',         N'InUse'),
    (@Campus1, N'EQP-VENT-001',N'呼吸机',             N'生命支持', N'Maquet',      N'Servo-i',         N'InUse'),
    (@Campus1, N'EQP-MON-001', N'心电监护仪',         N'监护设备', N'Philips',     N'IntelliVue MP30', N'InUse'),
    (@Campus1, N'EQP-MON-002', N'心电监护仪',         N'监护设备', N'Mindray',     N'iMEC 12',         N'InUse'),
    (@Campus1, N'EQP-DEF-001', N'除颤仪',             N'急救设备', N'Philips',     N'HeartStart XL+',  N'Maintenance'),
    (@Campus1, N'EQP-PUMP-001',N'输液泵',             N'治疗设备', N'Baxter',      N'Colleague 3',     N'InUse')
) AS s(CampusId, AssetCode, Name, Category, Manufacturer, Model, Status)
ON t.CampusId = s.CampusId AND t.AssetCode = s.AssetCode
WHEN NOT MATCHED THEN INSERT (CampusId, AssetCode, Name, Category, Manufacturer, Model, Status)
    VALUES (s.CampusId, s.AssetCode, s.Name, s.Category, s.Manufacturer, s.Model, s.Status);

/* ---------- eqp.AssetMovements ---------- */
INSERT INTO eqp.AssetMovements (AssetId, MovementType, FromDeptId, ToDeptId, Remark)
SELECT a.Id, N'Allocate', NULL,
    (SELECT Id FROM mdm.Departments WHERE Code = N'FSK' AND CampusId = @Campus1), N'放射科CT安装'
FROM eqp.Assets a WHERE a.AssetCode = N'EQP-CT-001'
  AND NOT EXISTS (SELECT 1 FROM eqp.AssetMovements m WHERE m.AssetId = a.Id);

INSERT INTO eqp.AssetMovements (AssetId, MovementType, FromDeptId, ToDeptId, Remark)
SELECT a.Id, N'Allocate', NULL,
    (SELECT Id FROM mdm.Departments WHERE Code = N'JYK' AND CampusId = @Campus1), N'检验科设备安装'
FROM eqp.Assets a WHERE a.AssetCode IN (N'EQP-LAB-001', N'EQP-LAB-002')
  AND NOT EXISTS (SELECT 1 FROM eqp.AssetMovements m WHERE m.AssetId = a.Id AND m.ToDeptId = (SELECT Id FROM mdm.Departments WHERE Code = N'JYK' AND CampusId = @Campus1));

INSERT INTO eqp.AssetMovements (AssetId, MovementType, FromDeptId, ToDeptId, Remark)
SELECT a.Id, N'Allocate', NULL,
    (SELECT Id FROM mdm.Departments WHERE Code = N'JZ' AND CampusId = @Campus1), N'急诊科设备分配'
FROM eqp.Assets a WHERE a.AssetCode IN (N'EQP-VENT-001', N'EQP-MON-001', N'EQP-DEF-001')
  AND NOT EXISTS (SELECT 1 FROM eqp.AssetMovements m WHERE m.AssetId = a.Id AND m.ToDeptId = (SELECT Id FROM mdm.Departments WHERE Code = N'JZ' AND CampusId = @Campus1));

/* ---------- eqp.InspectionTasks ---------- */
INSERT INTO eqp.InspectionTasks (AssetId, PlanDate, Status)
SELECT a.Id, CAST(DATEADD(DAY, 1, SYSUTCDATETIME()) AS DATE), N'Pending'
FROM eqp.Assets a WHERE a.Status = N'InUse'
  AND NOT EXISTS (SELECT 1 FROM eqp.InspectionTasks t WHERE t.AssetId = a.Id AND t.PlanDate = CAST(DATEADD(DAY, 1, SYSUTCDATETIME()) AS DATE));

INSERT INTO eqp.InspectionTasks (AssetId, PlanDate, Status)
SELECT a.Id, CAST(DATEADD(DAY, -7, SYSUTCDATETIME()) AS DATE), N'Completed'
FROM eqp.Assets a WHERE a.Status = N'InUse'
  AND NOT EXISTS (SELECT 1 FROM eqp.InspectionTasks t WHERE t.AssetId = a.Id AND t.PlanDate = CAST(DATEADD(DAY, -7, SYSUTCDATETIME()) AS DATE));

/* ---------- eqp.InspectionResults ---------- */
INSERT INTO eqp.InspectionResults (TaskId, Result, Notes, InspectedAt, InspectorId)
SELECT t.Id, N'Normal', N'设备运行正常，无异常。', DATEADD(HOUR, 10, CAST(t.PlanDate AS DATETIME2)),
    (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0018' AND CampusId = @Campus1)
FROM eqp.InspectionTasks t WHERE t.Status = N'Completed'
  AND NOT EXISTS (SELECT 1 FROM eqp.InspectionResults r WHERE r.TaskId = t.Id);

/* ---------- eqp.WorkOrders ---------- */
INSERT INTO eqp.WorkOrders (AssetId, Title, Status, ReportedAt, CompletedAt)
SELECT a.Id, N'CT机软件升级', N'Completed', DATEADD(DAY, -30, SYSUTCDATETIME()), DATEADD(DAY, -28, SYSUTCDATETIME())
FROM eqp.Assets a WHERE a.AssetCode = N'EQP-CT-001'
  AND NOT EXISTS (SELECT 1 FROM eqp.WorkOrders w WHERE w.AssetId = a.Id AND w.Title = N'CT机软件升级');

INSERT INTO eqp.WorkOrders (AssetId, Title, Status, ReportedAt)
SELECT a.Id, N'监护仪屏幕闪烁', N'Open', DATEADD(DAY, -2, SYSUTCDATETIME())
FROM eqp.Assets a WHERE a.AssetCode = N'EQP-MON-002'
  AND NOT EXISTS (SELECT 1 FROM eqp.WorkOrders w WHERE w.AssetId = a.Id AND w.Title = N'监护仪屏幕闪烁');

INSERT INTO eqp.WorkOrders (AssetId, Title, Status, ReportedAt)
SELECT a.Id, N'除颤仪电池老化', N'Open', DATEADD(DAY, -5, SYSUTCDATETIME())
FROM eqp.Assets a WHERE a.AssetCode = N'EQP-DEF-001'
  AND NOT EXISTS (SELECT 1 FROM eqp.WorkOrders w WHERE w.AssetId = a.Id AND w.Title = N'除颤仪电池老化');

/* ---------- eqp.CalibrationRecords ---------- */
INSERT INTO eqp.CalibrationRecords (AssetId, CalibDate, NextDueDate, CertificateRef)
SELECT a.Id, DATEADD(MONTH, -6, CAST(SYSUTCDATETIME() AS DATE)), DATEADD(MONTH, 6, CAST(SYSUTCDATETIME() AS DATE)), N'CERT_' + a.AssetCode + N'_2025'
FROM eqp.Assets a WHERE a.Category IN (N'影像设备', N'检验设备')
  AND NOT EXISTS (SELECT 1 FROM eqp.CalibrationRecords c WHERE c.AssetId = a.Id);

/* =========================================================================
   第十五部分：财务结算 (FIN)
   ========================================================================= */

/* ---------- fin.Invoices ---------- */
-- Create invoices for the encounters that have paid registrations
INSERT INTO fin.Invoices (CampusId, PayerPatientId, TotalAmount, SettledAt, Status)
SELECT DISTINCT @Campus1, r.PatientId, r.FeeAmount, r.PaidAt, N'Settled'
FROM opd.Registrations r
WHERE r.CampusId = @Campus1 AND r.PaidAt IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM fin.Invoices i WHERE i.CampusId = @Campus1 AND i.PayerPatientId = r.PatientId
    AND i.TotalAmount = r.FeeAmount AND i.SettledAt = r.PaidAt);

/* ---------- fin.ChargeLines ---------- */
INSERT INTO fin.ChargeLines (CampusId, InvoiceId, OutpatientEncounterId, ChargeItemId, Qty, Amount, Status)
SELECT @Campus1, i.Id, e.Id, @RegFeeItem, 1, r.FeeAmount, N'Posted'
FROM opd.Registrations r
INNER JOIN enc.OutpatientEncounters e ON r.Id = e.RegistrationId
INNER JOIN fin.Invoices i ON i.PayerPatientId = r.PatientId AND i.TotalAmount = r.FeeAmount
WHERE r.CampusId = @Campus1 AND r.PaidAt IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM fin.ChargeLines cl WHERE cl.InvoiceId = i.Id AND cl.OutpatientEncounterId = e.Id);

-- Add prescription charges
INSERT INTO fin.ChargeLines (CampusId, InvoiceId, OutpatientEncounterId, PrescriptionLineId, ChargeItemId, Qty, Amount, Status)
SELECT @Campus1, i.Id, e.Id, pl.Id,
    CASE WHEN dr.Code IN (N'DRG_001', N'DRG_002', N'DRG_009') THEN @DrugCef ELSE @ChargeDrug004 END,
    pl.Qty,
    CASE WHEN dr.Code IN (N'DRG_001', N'DRG_002', N'DRG_009') THEN 25.00 * pl.Qty ELSE 12.50 * pl.Qty END,
    N'Posted'
FROM pha.Prescriptions p
INNER JOIN enc.OutpatientEncounters e ON p.OutpatientEncounterId = e.Id
INNER JOIN pha.PrescriptionLines pl ON p.Id = pl.PrescriptionId
INNER JOIN pha.Drugs dr ON pl.DrugId = dr.Id
INNER JOIN fin.Invoices i ON i.PayerPatientId = e.PatientId
WHERE NOT EXISTS (SELECT 1 FROM fin.ChargeLines cl WHERE cl.PrescriptionLineId = pl.Id);

/* ---------- fin.Payments ---------- */
INSERT INTO fin.Payments (InvoiceId, PayMethod, Amount, PaidAt)
SELECT i.Id, N'WECHAT', i.TotalAmount, i.SettledAt
FROM fin.Invoices i WHERE i.SettledAt IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM fin.Payments p WHERE p.InvoiceId = i.Id AND p.PayMethod = N'WECHAT');

/* ---------- fin.InvoiceBridgeLogs ---------- */
INSERT INTO fin.InvoiceBridgeLogs (InvoiceId, RequestJson, ResponseJson, Status)
SELECT i.Id, N'{"action":"issue"}', N'{"code":"0","msg":"success","invoiceNo":"EINV' + CAST(i.Id AS NVARCHAR) + N'"}', N'Success'
FROM fin.Invoices i WHERE i.SettledAt IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM fin.InvoiceBridgeLogs l WHERE l.InvoiceId = i.Id);

/* ---------- fin.InsuranceReads ---------- */
INSERT INTO fin.InsuranceReads (PatientId, InsuredArea, RawPayloadJson)
SELECT @Pat1, N'上海市黄浦区', N'{"cardType":"社保卡","insuredType":"职工医保","balance":5000}'
WHERE NOT EXISTS (SELECT 1 FROM fin.InsuranceReads WHERE PatientId = @Pat1)
UNION ALL
SELECT @Pat2, N'上海市浦东新区', N'{"cardType":"社保卡","insuredType":"居民医保","balance":3000}'
WHERE NOT EXISTS (SELECT 1 FROM fin.InsuranceReads WHERE PatientId = @Pat2)
UNION ALL
SELECT @Pat3, N'北京市朝阳区', N'{"cardType":"社保卡","insuredType":"职工医保","balance":8000}'
WHERE NOT EXISTS (SELECT 1 FROM fin.InsuranceReads WHERE PatientId = @Pat3);

/* ---------- fin.InsuranceSettlements ---------- */
INSERT INTO fin.InsuranceSettlements (InvoiceId, InsTxnId, FundPay, SelfPay, SettledAt)
SELECT i.Id, N'INS_TXN_' + CAST(i.Id AS NVARCHAR), i.TotalAmount * 0.7, i.TotalAmount * 0.3, i.SettledAt
FROM fin.Invoices i WHERE i.SettledAt IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM fin.InsuranceSettlements s WHERE s.InvoiceId = i.Id);

/* ---------- fin.InsuranceReconcileBatches ---------- */
INSERT INTO fin.InsuranceReconcileBatches (CampusId, PeriodStart, PeriodEnd, Status)
SELECT @Campus1, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE)), CAST(SYSUTCDATETIME() AS DATE), N'Open'
WHERE NOT EXISTS (SELECT 1 FROM fin.InsuranceReconcileBatches WHERE CampusId = @Campus1 AND Status = N'Open');

/* ---------- fin.InsuranceReconcileLines ---------- */
INSERT INTO fin.InsuranceReconcileLines (BatchId, SettlementId, DiffAmount, Resolution)
SELECT b.Id, s.Id, 0.00, N'一致'
FROM fin.InsuranceReconcileBatches b
CROSS JOIN fin.InsuranceSettlements s
INNER JOIN fin.Invoices i ON s.InvoiceId = i.Id
WHERE b.CampusId = @Campus1 AND i.CampusId = @Campus1
  AND NOT EXISTS (SELECT 1 FROM fin.InsuranceReconcileLines l WHERE l.BatchId = b.Id AND l.SettlementId = s.Id);

/* =========================================================================
   第十六部分：报表 (RPT)
   ========================================================================= */

/* ---------- rpt.ReportDefinitions ---------- */
MERGE INTO rpt.ReportDefinitions AS t
USING (VALUES
    (N'RPT_OPD_001', N'门诊挂号日报表',    N'/reports/opd/daily_reg',   1),
    (N'RPT_OPD_002', N'门诊医生工作量统计', N'/reports/opd/doc_workload',1),
    (N'RPT_IPD_001', N'住院患者统计表',    N'/reports/ipd/patient_stats',1),
    (N'RPT_IPD_002', N'床位使用率报表',    N'/reports/ipd/bed_usage',   1),
    (N'RPT_PHA_001', N'药品库存统计表',    N'/reports/pha/inventory',   1),
    (N'RPT_PHA_002', N'药品消耗分析报表',  N'/reports/pha/consumption', 1),
    (N'RPT_FIN_001', N'收费日报表',        N'/reports/fin/daily_cashier',1),
    (N'RPT_FIN_002', N'医保结算汇总表',    N'/reports/fin/insurance_summary',1),
    (N'RPT_FIN_003', N'欠费患者明细表',    N'/reports/fin/arrears',    1),
    (N'RPT_MDM_001', N'科室人员统计表',    N'/reports/mdm/staff_dept', 1),
    (N'RPT_EQP_001', N'设备资产台账',      N'/reports/eqp/assets',     1),
    (N'RPT_EQP_002', N'设备巡检记录表',    N'/reports/eqp/inspection', 1)
) AS s(Code, Name, ReportServerPath, IsActive)
ON t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (Code, Name, ReportServerPath, IsActive) VALUES (s.Code, s.Name, s.ReportServerPath, s.IsActive);

/* ---------- rpt.ExportJobs ---------- */
INSERT INTO rpt.ExportJobs (RequestedByUserId, Status, FileStorageKey, CreatedAt, CompletedAt)
SELECT @UserIdAdmin, N'Completed', N'exports/opd_daily_reg_20260101.xlsx', DATEADD(DAY, -2, SYSUTCDATETIME()), DATEADD(DAY, -2, SYSUTCDATETIME())
WHERE NOT EXISTS (SELECT 1 FROM rpt.ExportJobs WHERE FileStorageKey = N'exports/opd_daily_reg_20260101.xlsx')
UNION ALL
SELECT @UserIdAdmin, N'Completed', N'exports/fin_daily_20260101.xlsx', DATEADD(DAY, -1, SYSUTCDATETIME()), DATEADD(DAY, -1, SYSUTCDATETIME())
WHERE NOT EXISTS (SELECT 1 FROM rpt.ExportJobs WHERE FileStorageKey = N'exports/fin_daily_20260101.xlsx')
UNION ALL
SELECT @UserIdAdmin, N'Queued', NULL, SYSUTCDATETIME(), NULL
WHERE NOT EXISTS (SELECT 1 FROM rpt.ExportJobs WHERE RequestedByUserId = @UserIdAdmin AND Status = N'Queued');

/* =========================================================================
   第十七部分：回溯更新外键 - 床位占用
   ========================================================================= */

-- Update Beds.OccupiedByAdmissionId for active admissions
UPDATE mdm.Beds SET OccupiedByAdmissionId = a.Id
FROM ipd.Admissions a
INNER JOIN mdm.Beds b ON a.BedId = b.Id
WHERE a.Status = N'InHospital' AND b.OccupiedByAdmissionId IS NULL;

/* =========================================================================
   第十八部分：急诊就诊 (部分数据)
   ========================================================================= */

/* ---------- enc.EmergencyEncounters ---------- */
INSERT INTO enc.EmergencyEncounters (CampusId, PatientId, DepartmentId, StaffId, Status, StartedAt)
SELECT @Campus1, @Pat8, (SELECT Id FROM mdm.Departments WHERE Code = N'JZ' AND CampusId = @Campus1), @StaffDoc6, N'Open', SYSUTCDATETIME()
WHERE NOT EXISTS (SELECT 1 FROM enc.EmergencyEncounters WHERE PatientId = @Pat8 AND Status = N'Open');

INSERT INTO enc.EmergencyEncounters (CampusId, PatientId, DepartmentId, StaffId, Status, StartedAt, EndedAt)
SELECT @Campus1, @Pat10, (SELECT Id FROM mdm.Departments WHERE Code = N'JZ' AND CampusId = @Campus1), @StaffDoc6, N'Closed', DATEADD(HOUR, -4, SYSUTCDATETIME()), DATEADD(HOUR, -2, SYSUTCDATETIME())
WHERE NOT EXISTS (SELECT 1 FROM enc.EmergencyEncounters WHERE PatientId = @Pat10 AND Status = N'Closed');

/* ---------- mon.VitalSignSets for ER ---------- */
-- Vital signs for ER encounters (need to reference EmergencyEncounterId)
INSERT INTO mon.VitalSignSets (EmergencyEncounterId, RecordedAt, Source)
SELECT e.Id, e.StartedAt, N'Manual'
FROM enc.EmergencyEncounters e
WHERE NOT EXISTS (SELECT 1 FROM mon.VitalSignSets v WHERE v.EmergencyEncounterId = e.Id);

INSERT INTO mon.VitalSignItems (SetId, Code, Value, Unit)
SELECT v.Id, N'T', N'37.2', N'℃' FROM mon.VitalSignSets v WHERE v.EmergencyEncounterId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM mon.VitalSignItems i WHERE i.SetId = v.Id AND i.Code = N'T')
UNION ALL
SELECT v.Id, N'PR', N'88', N'bpm' FROM mon.VitalSignSets v WHERE v.EmergencyEncounterId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM mon.VitalSignItems i WHERE i.SetId = v.Id AND i.Code = N'PR')
UNION ALL
SELECT v.Id, N'BP', N'140/90', N'mmHg' FROM mon.VitalSignSets v WHERE v.EmergencyEncounterId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM mon.VitalSignItems i WHERE i.SetId = v.Id AND i.Code = N'BP');

/* ---------- IcuWaveformSessions ---------- */
-- For empty beds (not occupied), create a waveform session
INSERT INTO mon.IcuWaveformSessions (AdmissionId, BedId, StartedAt, EndedAt, StorageKey)
SELECT a.Id, a.BedId, a.AdmittedAt, NULL, N'waveforms/' + a.AdmissionNo + N'_' + FORMAT(a.AdmittedAt, N'yyyyMMdd') + N'.dat'
FROM ipd.Admissions a WHERE a.BedId IS NOT NULL AND a.Status = N'InHospital'
  AND NOT EXISTS (SELECT 1 FROM mon.IcuWaveformSessions s WHERE s.AdmissionId = a.Id);

/* ---------- pat.PatientMergeLogs ---------- */
INSERT INTO pat.PatientMergeLogs (SurvivorPatientId, MergedPatientId, MergedByUserId, PayloadJson)
SELECT @Pat1, @Pat10, @UserIdAdmin, N'{"survivor":"P20250001","merged":"P20250010","reason":"重复档案合并","fields":{"name":"张明"}}'
WHERE NOT EXISTS (SELECT 1 FROM pat.PatientMergeLogs WHERE SurvivorPatientId = @Pat1 AND MergedPatientId = @Pat10);

PRINT N'903_seed_data_ipd_finance.sql 执行完成。';
GO

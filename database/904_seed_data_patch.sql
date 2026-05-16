/*
  904_seed_data_patch.sql
  种子数据补丁：补充不足 10 行的表数据。
  依赖：901~903 已执行完毕。
*/
USE [Hospital];
GO

SET NOCOUNT ON;

-- 重新声明变量（因 GO 批处理分隔，变量不在跨文件保留）
DECLARE @Campus1 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'ZONGYUAN');
DECLARE @StaffDoc6 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0010' AND CampusId = @Campus1);
DECLARE @UserIdAdmin BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'admin');

/* --- 补充 pat.PatientMergeLogs --- */
INSERT INTO pat.PatientMergeLogs (SurvivorPatientId, MergedPatientId, MergedByUserId, PayloadJson)
SELECT s.Id, m.Id, u.Id, N'{"reason":"门诊合并"}'
FROM pat.Patients s CROSS JOIN pat.Patients m CROSS JOIN sec.Users u
WHERE s.PatientNo = N'P20250002' AND m.PatientNo = N'P20250005' AND u.LoginName = N'admin'
  AND NOT EXISTS (SELECT 1 FROM pat.PatientMergeLogs WHERE SurvivorPatientId = s.Id AND MergedPatientId = m.Id);

INSERT INTO pat.PatientMergeLogs (SurvivorPatientId, MergedPatientId, MergedByUserId, PayloadJson)
SELECT s.Id, m.Id, u.Id, N'{"reason":"医保合并"}'
FROM pat.Patients s CROSS JOIN pat.Patients m CROSS JOIN sec.Users u
WHERE s.PatientNo = N'P20250004' AND m.PatientNo = N'P20250009' AND u.LoginName = N'admin'
  AND NOT EXISTS (SELECT 1 FROM pat.PatientMergeLogs WHERE SurvivorPatientId = s.Id AND MergedPatientId = m.Id);

/* --- 补充 enc.EmergencyEncounters --- */
INSERT INTO enc.EmergencyEncounters (CampusId, PatientId, DepartmentId, StaffId, Status, StartedAt, EndedAt)
SELECT @Campus1, p.Id, (SELECT Id FROM mdm.Departments WHERE Code = N'JZ' AND CampusId = @Campus1), @StaffDoc6, N'Closed',
    DATEADD(HOUR, -n, SYSUTCDATETIME()), DATEADD(HOUR, -n+1, SYSUTCDATETIME())
FROM pat.Patients p
CROSS JOIN (VALUES (8),(12),(24),(48),(72)) AS hours(n)
WHERE p.PatientNo IN (N'P20250001', N'P20250003', N'P20250005', N'P20250007', N'P20250009')
  AND NOT EXISTS (SELECT 1 FROM enc.EmergencyEncounters e
    WHERE e.PatientId = p.Id AND e.StartedAt = DATEADD(HOUR, -n, SYSUTCDATETIME()));

/* --- 补充 rad.Appointments (10+ rows) --- */
INSERT INTO rad.Appointments (ImagingOrderId, Modality, ScheduledAt, Status)
SELECT o.Id, N'CT', DATEADD(DAY, 7, o.OrderedAt), N'Scheduled'
FROM rad.ImagingOrders o
WHERE NOT EXISTS (SELECT 1 FROM rad.Appointments a WHERE a.ImagingOrderId = o.Id)
UNION ALL
SELECT o.Id, N'MR', DATEADD(DAY, 14, o.OrderedAt), N'Scheduled'
FROM rad.ImagingOrders o
WHERE NOT EXISTS (SELECT 1 FROM rad.Appointments a WHERE a.ImagingOrderId = o.Id AND a.Modality = N'MR');

/* --- 补充 rad.Registrations --- */
INSERT INTO rad.Registrations (AppointmentId, ArrivedAt)
SELECT a.Id, DATEADD(MINUTE, 30, a.ScheduledAt)
FROM rad.Appointments a
WHERE a.Status = N'Scheduled'
  AND NOT EXISTS (SELECT 1 FROM rad.Registrations r WHERE r.AppointmentId = a.Id);

/* --- 补充 rad.Reports --- */
INSERT INTO rad.Reports (ImagingOrderId, ReportNo, PdfUrl, ReleasedAt)
SELECT o.Id, N'RPT' + FORMAT(CAST(o.OrderedAt AS DATE), N'yyyyMMdd') + CAST(o.Id + 100 AS NVARCHAR),
    N'/reports/rad/' + CAST(o.Id AS NVARCHAR) + N'.pdf',
    DATEADD(HOUR, 72, o.OrderedAt)
FROM rad.ImagingOrders o
WHERE NOT EXISTS (SELECT 1 FROM rad.Reports r WHERE r.ImagingOrderId = o.Id);

/* --- 补充 mon.CriticalValues --- */
INSERT INTO mon.CriticalValues (SourceSystem, RefId, PatientId, AcknowledgedAt, ClosedAt)
SELECT N'LIS', N'LAB_CRIT_00' + CAST(n AS NVARCHAR), p.Id,
    DATEADD(MINUTE, 15, SYSUTCDATETIME()),
    DATEADD(HOUR, 2, SYSUTCDATETIME())
FROM pat.Patients p
CROSS JOIN (SELECT 3 n UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL SELECT 10) nums
WHERE p.PatientNo = N'P20250001'
  AND NOT EXISTS (SELECT 1 FROM mon.CriticalValues WHERE RefId = N'LAB_CRIT_00' + CAST(n AS NVARCHAR));

/* --- 补充 mon.RemoteDevices --- */
INSERT INTO mon.RemoteDevices (PatientId, DeviceUid)
SELECT p.Id, N'DEVICE_HB_00' + CAST(n AS NVARCHAR)
FROM pat.Patients p
CROSS JOIN (SELECT 4 n UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7) nums
WHERE p.PatientNo IN (N'P20250002', N'P20250003', N'P20250004', N'P20250005')
  AND NOT EXISTS (SELECT 1 FROM mon.RemoteDevices WHERE DeviceUid = N'DEVICE_HB_00' + CAST(n AS NVARCHAR));

/* --- 补充 fin.Refunds --- */
INSERT INTO fin.Refunds (OriginalPaymentId, Amount, Reason, ApprovedByUserId, RefundedAt)
SELECT TOP 3 p.Id, p.Amount * 0.5, N'患者退费申请', @UserIdAdmin, DATEADD(DAY, 1, p.PaidAt)
FROM fin.Payments p
WHERE NOT EXISTS (SELECT 1 FROM fin.Refunds r WHERE r.OriginalPaymentId = p.Id);

/* --- 补充 fin.InsuranceReconcileBatches (历史批次) --- */
INSERT INTO fin.InsuranceReconcileBatches (CampusId, PeriodStart, PeriodEnd, Status)
SELECT @Campus1, DATEADD(MONTH, -n, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE))), DATEADD(MONTH, -n, CAST(SYSUTCDATETIME() AS DATE)), N'Reconciled'
FROM (VALUES (1),(2),(3),(4),(5),(6)) nums(n)
WHERE NOT EXISTS (SELECT 1 FROM fin.InsuranceReconcileBatches b
    WHERE b.CampusId = @Campus1 AND b.PeriodStart = DATEADD(MONTH, -n, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE))));

/* --- 补充 mon.IcuWaveformSessions (更多会话) --- */
INSERT INTO mon.IcuWaveformSessions (BedId, StartedAt, EndedAt, StorageKey)
SELECT b.Id, DATEADD(HOUR, -n, SYSUTCDATETIME()), DATEADD(HOUR, -n+2, SYSUTCDATETIME()),
    N'waveforms/session_bed' + CAST(b.Id AS NVARCHAR) + N'_' + CAST(n AS NVARCHAR) + N'.dat'
FROM mdm.Beds b
CROSS JOIN (VALUES (1),(2),(3),(4),(5)) nums(n)
WHERE b.Status = N'Empty'
  AND NOT EXISTS (SELECT 1 FROM mon.IcuWaveformSessions s WHERE s.BedId = b.Id AND s.StorageKey LIKE N'%session_bed' + CAST(b.Id AS NVARCHAR) + N'_' + CAST(n AS NVARCHAR) + N'%');

PRINT N'补丁数据已全部写入。';
GO

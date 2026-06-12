/*
  901_seed_data.sql
  EF Core 模型的全量测试数据：覆盖全部 27 张表的 10+ 行示例数据。
  依赖：900_seed_minimal.sql 已执行完毕。
  可重复执行：使用 MERGE / IF NOT EXISTS 判断。
  注意：Status 等枚举字段使用中文名称字符串（HasConversion<string> 存储）。
*/
USE [Hospital];
GO

SET NOCOUNT ON;
SET DATEFIRST 1;

/* ===================================================================
   第一部分：院区与科室（基于 900 已有数据继续扩充）
   =================================================================== */
DECLARE @Campus1 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'ZONGYUAN');

-- 创建更多院区
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'DONGYUAN')
    INSERT INTO mdm.Campuses (Code, Name, Address, IsActive) VALUES (N'DONGYUAN', N'东院', N'北京市朝阳区', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'XIYUAN')
    INSERT INTO mdm.Campuses (Code, Name, Address, IsActive) VALUES (N'XIYUAN', N'西院', N'北京市西城区', 1);

DECLARE @Campus2 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'DONGYUAN');
DECLARE @Campus3 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'XIYUAN');

-- 东院/西院根科室
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus2)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive) VALUES (@Campus2, NULL, N'ROOT', N'东院根科室', N'Admin', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus3)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive) VALUES (@Campus3, NULL, N'ROOT', N'西院根科室', N'Admin', 1);

DECLARE @C2Root BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus2);
DECLARE @C3Root BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus3);

-- 获取 900 中已创建的科室 ID
DECLARE @DeptNK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'NK' AND CampusId = @Campus1);
DECLARE @DeptWK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'WK' AND CampusId = @Campus1);

/* ===================================================================
   第二部分：人员（扩充到 10+ 人）
   =================================================================== */
-- 获得 900 中已创建的人员 ID，用于判定
DECLARE @DoctorId BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0002');

-- 额外创建医生和护士
MERGE INTO mdm.Staff AS t
USING (VALUES
    (@Campus1, @DeptNK, N'E0006', N'刘洋',     N'Female', N'13800138006', N'执业医师', N'110101199205056789', 1),
    (@Campus1, @DeptNK, N'E0007', N'陈芳',     N'Female', N'13800138007', N'执业护士', N'110101199103152890', 1),
    (@Campus1, @DeptWK, N'E0008', N'王强',     N'Male',   N'13800138008', N'执业医师', N'110101197803031456', 1),
    (@Campus1, @DeptWK, N'E0009', N'黄勇',     N'Male',   N'13800138009', N'执业医师', N'110101198612093456', 1),
    (@Campus1, @DeptNK, N'E0010', N'周磊',     N'Male',   N'13800138010', N'执业医师', N'110101198209214567', 1),
    (@Campus1, @DeptNK, N'E0011', N'吴秀英',   N'Female', N'13800138011', N'执业护士', N'110101199508181234', 1)
) AS s(CampusId, DepartmentId, EmployeeNo, FullName, Gender, Phone, StaffCategory, LicenseNo, IsActive)
ON t.EmployeeNo = s.EmployeeNo AND t.CampusId = s.CampusId
WHEN NOT MATCHED THEN INSERT (CampusId, DepartmentId, EmployeeNo, FullName, Gender, Phone, StaffCategory, LicenseNo, IsActive)
    VALUES (s.CampusId, s.DepartmentId, s.EmployeeNo, s.FullName, s.Gender, s.Phone, s.StaffCategory, s.LicenseNo, s.IsActive);

DECLARE @StaffDoc2 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0006');
DECLARE @StaffDoc3 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0008');
DECLARE @StaffDoc4 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0009');

/* ===================================================================
   第三部分：患者（20 人含标识与知情同意）
   =================================================================== */
MERGE INTO pat.Patients AS t
USING (VALUES
    (N'P20250001', N'110101199003151234', N'张明',   N'Male',   '1990-03-15', N'13800138001', N'青霉素过敏'),
    (N'P20250002', N'110101198507202345', N'李芳',   N'Female', '1985-07-20', N'13800138002', NULL),
    (N'P20250003', N'110101197811113456', N'王建国', N'Male',   '1978-11-11', N'13800138003', N'磺胺类过敏'),
    (N'P20250004', N'110101199208084567', N'赵秀英', N'Female', '1992-08-08', N'13800138004', NULL),
    (N'P20250005', N'110101200105055678', N'刘浩然', N'Male',   '2001-05-05', N'13800138005', NULL),
    (N'P20250006', N'110101196512256789', N'陈德明', N'Male',   '1965-12-25', N'13800138006', N'阿司匹林过敏'),
    (N'P20250007', N'110101199509152345', N'杨雪',   N'Female', '1995-09-15', N'13800138007', NULL),
    (N'P20250008', N'110101198203308901', N'黄海波', N'Male',   '1982-03-30', N'13800138008', NULL),
    (N'P20250009', N'110101197609092345', N'周玉兰', N'Female', '1976-09-09', N'13800138009', N'头孢类过敏'),
    (N'P20250010', N'110101199808186789', N'吴磊',   N'Male',   '1998-08-18', N'13800138010', NULL)
) AS s(PatientNo, IdCardNo, Name, Gender, BirthDate, Phone, AllergiesText)
ON t.PatientNo = s.PatientNo
WHEN NOT MATCHED THEN INSERT (PatientNo, IdCardNo, Name, Gender, BirthDate, Phone, AllergiesText)
    VALUES (s.PatientNo, s.IdCardNo, s.Name, s.Gender, s.BirthDate, s.Phone, s.AllergiesText);

-- 患者 ID 变量
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

/* ---------- 患者标识 ---------- */
MERGE INTO pat.PatientIdentifiers AS t
USING (VALUES
    (@Pat1, N'社保卡', N'SH110101199003151234', 1), (@Pat1, N'就诊卡', N'HOS_CARD_00001', 0),
    (@Pat2, N'社保卡', N'SH110101198507202345', 1), (@Pat2, N'就诊卡', N'HOS_CARD_00002', 0),
    (@Pat3, N'医保卡', N'YB110101197811113456', 1),
    (@Pat4, N'社保卡', N'SH110101199208084567', 1),
    (@Pat5, N'医保卡', N'YB110101200105055678', 1),
    (@Pat6, N'社保卡', N'SH110101196512256789', 1),
    (@Pat7, N'医保卡', N'YB110101199509152345', 1),
    (@Pat8, N'社保卡', N'SH110101198203308901', 1),
    (@Pat9, N'医保卡', N'YB110101197609092345', 1),
    (@Pat10, N'就诊卡', N'HOS_CARD_00010', 1)
) AS s(PatientId, IdType, IdValue, IsPrimary)
ON t.PatientId = s.PatientId AND t.IdType = s.IdType AND t.IdValue = s.IdValue
WHEN NOT MATCHED THEN INSERT (PatientId, IdType, IdValue, IsPrimary)
    VALUES (s.PatientId, s.IdType, s.IdValue, s.IsPrimary);

/* ---------- 患者知情同意 ---------- */
MERGE INTO pat.PatientConsents AS t
USING (VALUES
    (@Pat1, N'PrivacyPolicy', DATEADD(DAY, -30, SYSUTCDATETIME()), NULL),
    (@Pat2, N'PrivacyPolicy', DATEADD(DAY, -60, SYSUTCDATETIME()), NULL),
    (@Pat3, N'DataResearch',  DATEADD(DAY, -15, SYSUTCDATETIME()), DATEADD(YEAR, 1, SYSUTCDATETIME())),
    (@Pat4, N'PrivacyPolicy', DATEADD(DAY, -5, SYSUTCDATETIME()), NULL),
    (@Pat5, N'PrivacyPolicy', DATEADD(DAY, -45, SYSUTCDATETIME()), NULL),
    (@Pat6, N'DataResearch',  DATEADD(DAY, -20, SYSUTCDATETIME()), DATEADD(YEAR, 2, SYSUTCDATETIME())),
    (@Pat7, N'PrivacyPolicy', DATEADD(DAY, -10, SYSUTCDATETIME()), NULL),
    (@Pat8, N'PrivacyPolicy', DATEADD(DAY, -90, SYSUTCDATETIME()), NULL),
    (@Pat9, N'DataResearch',  DATEADD(DAY, -30, SYSUTCDATETIME()), DATEADD(YEAR, 1, SYSUTCDATETIME())),
    (@Pat10, N'PrivacyPolicy', DATEADD(DAY, -1, SYSUTCDATETIME()), NULL)
) AS s(PatientId, ConsentType, GrantedAt, ExpiresAt)
ON t.PatientId = s.PatientId AND t.ConsentType = s.ConsentType
WHEN NOT MATCHED THEN INSERT (PatientId, ConsentType, GrantedAt, ExpiresAt)
    VALUES (s.PatientId, s.ConsentType, s.GrantedAt, s.ExpiresAt);

/* ===================================================================
   第四部分：排班模板与时段
   为每位医生创建过去 14 天到未来 14 天的排班
   =================================================================== */
DECLARE @CurDate DATE = CAST(SYSDATETIME() AS DATE);
DECLARE @LoopDate DATE = DATEADD(DAY, -14, @CurDate);

WHILE @LoopDate <= DATEADD(DAY, 14, @CurDate)
BEGIN
    IF DATEPART(WEEKDAY, @LoopDate) BETWEEN 2 AND 6  -- 周一到周五
    BEGIN
        -- 医生 1（内科）
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleTemplates WHERE DoctorId = @DoctorId AND ScheduleDate = @LoopDate)
            INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
            VALUES (@DoctorId, @DeptNK, @Campus1, @LoopDate, N'已发布');
        -- 医生 2（内科）
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleTemplates WHERE DoctorId = @StaffDoc2 AND ScheduleDate = @LoopDate)
            INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
            VALUES (@StaffDoc2, @DeptNK, @Campus1, @LoopDate, N'已发布');
        -- 医生 3（外科）
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleTemplates WHERE DoctorId = @StaffDoc3 AND ScheduleDate = @LoopDate)
            INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
            VALUES (@StaffDoc3, @DeptWK, @Campus1, @LoopDate, N'已发布');
    END
    SET @LoopDate = DATEADD(DAY, 1, @LoopDate);
END

-- 为医生创建时段
DECLARE @TmplId BIGINT, @TmplDoctorId BIGINT;
DECLARE tmpl_cursor CURSOR LOCAL FOR
    SELECT Id, DoctorId FROM opd.ScheduleTemplates
    WHERE CampusId = @Campus1 AND DoctorId IN (@DoctorId, @StaffDoc2, @StaffDoc3)
      AND ScheduleDate BETWEEN DATEADD(DAY, -14, @CurDate) AND DATEADD(DAY, 14, @CurDate);

OPEN tmpl_cursor;
FETCH NEXT FROM tmpl_cursor INTO @TmplId, @TmplDoctorId;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'上午')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'上午', '08:00', '12:00', 30, 0);
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'下午')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'下午', '14:00', '17:00', 20, 0);
    FETCH NEXT FROM tmpl_cursor INTO @TmplId, @TmplDoctorId;
END
CLOSE tmpl_cursor;
DEALLOCATE tmpl_cursor;

/* ===================================================================
   第五部分：挂号（过去 7 天 + 今天）
   =================================================================== */
DECLARE @RegDate DATE, @Seq INT = 1;
DECLARE reg_cursor CURSOR LOCAL FOR
    SELECT DATEADD(DAY, -n, @CurDate) FROM (VALUES(0),(1),(2),(3),(4),(5),(6)) nums(n);

OPEN reg_cursor;
FETCH NEXT FROM reg_cursor INTO @RegDate;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF DATEPART(WEEKDAY, @RegDate) BETWEEN 2 AND 6
    BEGIN
        DECLARE @SlotAM BIGINT, @SlotPM BIGINT;
        -- 取医生1上午时段
        SELECT TOP 1 @SlotAM = ss.Id FROM opd.ScheduleSlots ss
            INNER JOIN opd.ScheduleTemplates st ON ss.TemplateId = st.Id
            WHERE st.DoctorId = @DoctorId AND st.ScheduleDate = @RegDate AND ss.SlotType = N'上午';
        -- 取医生2下午时段
        SELECT TOP 1 @SlotPM = ss.Id FROM opd.ScheduleSlots ss
            INNER JOIN opd.ScheduleTemplates st ON ss.TemplateId = st.Id
            WHERE st.DoctorId = @StaffDoc2 AND st.ScheduleDate = @RegDate AND ss.SlotType = N'下午';

        IF @RegDate < @CurDate  -- 过去的日期：已就诊
        BEGIN
            IF @SlotAM IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @SlotAM AND PatientId = @Pat1)
                INSERT INTO opd.Registrations (PatientId, SlotId, DoctorId, DeptId, CampusId, CreatedAt, QueueNo, SlotName, Status)
                VALUES (@Pat1, @SlotAM, @DoctorId, @DeptNK, @Campus1, DATEADD(HOUR, 8, CAST(@RegDate AS DATETIME2)), @Seq, N'上午', N'已就诊');
            SET @Seq = @Seq + 1;
            IF @SlotPM IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @SlotPM AND PatientId = @Pat2)
                INSERT INTO opd.Registrations (PatientId, SlotId, DoctorId, DeptId, CampusId, CreatedAt, QueueNo, SlotName, Status)
                VALUES (@Pat2, @SlotPM, @StaffDoc2, @DeptNK, @Campus1, DATEADD(HOUR, 14, CAST(@RegDate AS DATETIME2)), @Seq, N'下午', N'已就诊');
            SET @Seq = @Seq + 1;
        END
        ELSE  -- 今天：已挂号待就诊
        BEGIN
            IF @SlotAM IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @SlotAM AND PatientId = @Pat3)
                INSERT INTO opd.Registrations (PatientId, SlotId, DoctorId, DeptId, CampusId, CreatedAt, QueueNo, SlotName, Status)
                VALUES (@Pat3, @SlotAM, @DoctorId, @DeptNK, @Campus1, SYSDATETIME(), @Seq, N'上午', N'已挂号');
            SET @Seq = @Seq + 1;
            IF @SlotPM IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @SlotPM AND PatientId = @Pat4)
                INSERT INTO opd.Registrations (PatientId, SlotId, DoctorId, DeptId, CampusId, CreatedAt, QueueNo, SlotName, Status)
                VALUES (@Pat4, @SlotPM, @StaffDoc2, @DeptNK, @Campus1, SYSDATETIME(), @Seq, N'下午', N'已挂号');
            SET @Seq = @Seq + 1;
        END
    END
    FETCH NEXT FROM reg_cursor INTO @RegDate;
END
CLOSE reg_cursor;
DEALLOCATE reg_cursor;

/* ===================================================================
   第六部分：就诊、诊断、病历
   =================================================================== */
-- 获取已就诊的挂号
INSERT INTO enc.OutpatientEncounters (PatientId, StaffId, DepartmentId, CampusId, RegistrationId, Status, StartedAt, EndedAt)
SELECT r.PatientId, r.DoctorId, r.DeptId, r.CampusId, r.Id, N'已完成',
    DATEADD(MINUTE, 30, r.CreatedAt), DATEADD(MINUTE, 90, r.CreatedAt)
FROM opd.Registrations r
WHERE r.Status = N'已就诊'
  AND NOT EXISTS (SELECT 1 FROM enc.OutpatientEncounters e WHERE e.RegistrationId = r.Id);

-- 获取已挂号的（今天）就诊
INSERT INTO enc.OutpatientEncounters (PatientId, StaffId, DepartmentId, CampusId, RegistrationId, Status, StartedAt)
SELECT r.PatientId, r.DoctorId, r.DeptId, r.CampusId, r.Id, N'就诊中', SYSDATETIME()
FROM opd.Registrations r
WHERE r.Status = N'已挂号'
  AND NOT EXISTS (SELECT 1 FROM enc.OutpatientEncounters e WHERE e.RegistrationId = r.Id);

-- 诊断
INSERT INTO enc.Diagnoses (OutpatientEncounterId, DiagnosisType, IcdCode, IcdName, IsPrimary)
SELECT e.Id, N'主要诊断', N'I10.x05', N'高血压病2级', 1
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @DoctorId AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'I10.x05');

INSERT INTO enc.Diagnoses (OutpatientEncounterId, DiagnosisType, IcdCode, IcdName, IsPrimary)
SELECT e.Id, N'主要诊断', N'J15.901', N'细菌性肺炎', 1
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @StaffDoc2 AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'J15.901');

INSERT INTO enc.Diagnoses (OutpatientEncounterId, DiagnosisType, IcdCode, IcdName, IsPrimary)
SELECT e.Id, N'次要诊断', N'I10.x05', N'高血压', 0
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @DoctorId AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IsPrimary = 0 AND d.IcdCode = N'I10.x05');

-- 电子病历（DocType = RecordStatus 枚举名称："草稿"/"终稿"/"已修改"）
INSERT INTO enc.EmrDocuments (OutpatientEncounterId, ContentJson, DocType, Version)
SELECT e.Id, N'{"主诉":"胸闷气促1周","诊断":"高血压","处理":"继续降压治疗"}', N'终稿', 1
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @DoctorId AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.EmrDocuments m WHERE m.OutpatientEncounterId = e.Id);

INSERT INTO enc.EmrDocuments (OutpatientEncounterId, ContentJson, DocType, Version)
SELECT e.Id, N'{"主诉":"咳嗽咳痰3天","诊断":"肺炎","处理":"抗感染治疗"}', N'终稿', 1
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @StaffDoc2 AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.EmrDocuments m WHERE m.OutpatientEncounterId = e.Id AND m.ContentJson LIKE N'%肺炎%');

/* ===================================================================
   第七部分：处方
   =================================================================== */
INSERT INTO pha.Prescriptions (OutpatientEncounterId, PrescribedByStaffId, Status)
SELECT e.Id, e.StaffId, N'已发药'
FROM enc.OutpatientEncounters e
WHERE e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM pha.Prescriptions p WHERE p.OutpatientEncounterId = e.Id);

-- 处方明细
INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugName, Spec, Form, Frequency, Dose, Days, Qty, Note)
SELECT p.Id, N'硝苯地平片', N'10mg*100片', N'片剂', N'QD', N'10mg', 30, 30, N'口服'
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @DoctorId)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugName = N'硝苯地平片');

INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugName, Spec, Form, Frequency, Dose, Days, Qty, Note)
SELECT p.Id, N'阿莫西林胶囊', N'0.25g*24粒', N'胶囊', N'TID', N'0.5g', 7, 42, N'口服'
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc2)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugName = N'阿莫西林胶囊');

/* ===================================================================
   第八部分：检验检查
   =================================================================== */
INSERT INTO lab.LabOrders (OutpatientEncounterId, ItemCode, ItemName, Status)
SELECT e.Id, N'CBC', N'血常规', N'已开单'
FROM enc.OutpatientEncounters e
WHERE e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM lab.LabOrders o WHERE o.OutpatientEncounterId = e.Id AND o.ItemCode = N'CBC');

INSERT INTO lab.LabOrders (OutpatientEncounterId, ItemCode, ItemName, Status)
SELECT e.Id, N'LIVER_FUNC', N'肝功能', N'已开单'
FROM enc.OutpatientEncounters e
WHERE e.Status = N'已完成' AND e.StaffId = @DoctorId
  AND NOT EXISTS (SELECT 1 FROM lab.LabOrders o WHERE o.OutpatientEncounterId = e.Id AND o.ItemCode = N'LIVER_FUNC');

INSERT INTO rad.ImagingOrders (OutpatientEncounterId, ItemCode, ItemName, Status)
SELECT e.Id, N'CHEST_XRAY', N'胸部X线检查', N'已开单'
FROM enc.OutpatientEncounters e
WHERE e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM rad.ImagingOrders o WHERE o.OutpatientEncounterId = e.Id AND o.ItemCode = N'CHEST_XRAY');

/* ===================================================================
   第九部分：发票（Invoices）及收费明细、支付
   =================================================================== */
INSERT INTO fin.Invoices (PayerPatientId, PatientName, TotalAmount, Status, CreatedAt, SettledAt)
SELECT DISTINCT r.PatientId, p.Name, 10.00, N'已缴', r.CreatedAt, DATEADD(MINUTE, 5, r.CreatedAt)
FROM opd.Registrations r
INNER JOIN pat.Patients p ON r.PatientId = p.Id
WHERE r.Status = N'已就诊'
  AND NOT EXISTS (SELECT 1 FROM fin.Invoices i WHERE i.PayerPatientId = r.PatientId AND i.CreatedAt = r.CreatedAt);

DECLARE @Inv1 BIGINT = (SELECT TOP 1 Id FROM fin.Invoices ORDER BY Id);
DECLARE @Inv2 BIGINT = (SELECT TOP 1 Id FROM fin.Invoices WHERE Id > @Inv1 ORDER BY Id);

IF @Inv1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM fin.ChargeLines WHERE BillingId = @Inv1)
    INSERT INTO fin.ChargeLines (BillingId, ItemType, ItemName, Amount)
    VALUES (@Inv1, N'Registration', N'普通挂号费', 10.00);
IF @Inv2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM fin.ChargeLines WHERE BillingId = @Inv2)
    INSERT INTO fin.ChargeLines (BillingId, ItemType, ItemName, Amount)
    VALUES (@Inv2, N'Registration', N'普通挂号费', 10.00);

IF @Inv1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM fin.Payments WHERE BillingId = @Inv1)
    INSERT INTO fin.Payments (BillingId, PayMethod, Amount, TransactionRef, PaidAt)
    VALUES (@Inv1, N'WECHAT', 10.00, N'wx_' + CAST(@Inv1 AS NVARCHAR), DATEADD(MINUTE, 1, (SELECT CreatedAt FROM fin.Invoices WHERE Id = @Inv1)));
IF @Inv2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM fin.Payments WHERE BillingId = @Inv2)
    INSERT INTO fin.Payments (BillingId, PayMethod, Amount, TransactionRef, PaidAt)
    VALUES (@Inv2, N'CASH', 10.00, NULL, DATEADD(MINUTE, 1, (SELECT CreatedAt FROM fin.Invoices WHERE Id = @Inv2)));

/* ===================================================================
   第十部分：药品批次与发药
   =================================================================== */
-- 补充药品批次
MERGE INTO pha.DrugBatches AS t
USING (VALUES
    (N'硝苯地平片', N'10mg*100片', N'20250601', '2027-06-01', 500, 500),
    (N'阿莫西林胶囊', N'0.25g*24粒', N'20250602', '2027-06-01', 800, 800),
    (N'头孢克肟片', N'50mg*12片', N'20250601', '2027-05-01', 400, 400),
    (N'布洛芬缓释胶囊', N'0.3g*20粒', N'20250601', '2028-06-01', 300, 300)
) AS s(DrugName, Spec, BatchNo, ExpiryDate, TotalQuantity, AvailableQuantity)
ON t.DrugCode = s.DrugName AND t.BatchNo = s.BatchNo
WHEN NOT MATCHED THEN INSERT (DrugCode, DrugName, Spec, BatchNo, ExpiryDate, TotalQuantity, AvailableQuantity)
    VALUES (s.DrugName, s.DrugName, s.Spec, s.BatchNo, s.ExpiryDate, s.TotalQuantity, s.AvailableQuantity);

-- 发药记录
INSERT INTO pha.Dispenses (PrescriptionId, DispensedBy, Status, CreatedAt)
SELECT p.Id, @DoctorId, N'已发药', SYSDATETIME()
FROM pha.Prescriptions p
WHERE p.Status = N'已发药'
  AND NOT EXISTS (SELECT 1 FROM pha.Dispenses d WHERE d.PrescriptionId = p.Id);

DECLARE @DispenseId BIGINT = (SELECT TOP 1 Id FROM pha.Dispenses ORDER BY Id);
DECLARE @Batch1 BIGINT = (SELECT TOP 1 Id FROM pha.DrugBatches ORDER BY Id);

IF @DispenseId IS NOT NULL AND @Batch1 IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM pha.DispenseLines WHERE DispensingId = @DispenseId)
    INSERT INTO pha.DispenseLines (DispensingId, InventoryLotId, DrugName, Spec, Qty)
    VALUES (@DispenseId, @Batch1, N'硝苯地平片', N'10mg*100片', 30);

/* ===================================================================
   第十一部分：审计日志
   =================================================================== */
IF NOT EXISTS (SELECT 1 FROM sec.AuditLogs WHERE Action = N'Login' AND UserName = N'admin')
    INSERT INTO sec.AuditLogs (UserId, UserName, Action, EntityType, EntityId, IpAddress, OccurredAt)
    VALUES (1, N'admin', N'Login', N'Session', 1, N'192.168.1.100', SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM sec.AuditLogs WHERE Action = N'Login' AND UserName = N'doctor')
    INSERT INTO sec.AuditLogs (UserId, UserName, Action, EntityType, EntityId, IpAddress, OccurredAt)
    VALUES (2, N'doctor', N'Login', N'Session', 2, N'192.168.1.101', DATEADD(HOUR, -1, SYSDATETIME()));

PRINT N'901_seed_data.sql 执行完成。';
GO

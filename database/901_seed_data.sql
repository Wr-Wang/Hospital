/*
  901_seed_data.sql
  全量正式测试数据：每表 10+ 行真实中文医疗数据。
  依赖：000~015 全部建表脚本已执行完毕，900_seed_minimal.sql 已执行。
  可重复执行：使用 IF NOT EXISTS / MERGE 判断。
  执行顺序：按表依赖层级由底向上。
*/
USE [Hospital];
GO

SET NOCOUNT ON;
SET DATEFIRST 1; -- 周一为一周起始

/* =========================================================================
   第一部分：主数据 (MDM)
   ========================================================================= */

/* ---------- mdm.Organizations ---------- */
IF NOT EXISTS (SELECT 1 FROM mdm.Organizations WHERE Code = N'RENJI_GROUP')
    INSERT INTO mdm.Organizations (Code, Name) VALUES (N'RENJI_GROUP', N'仁济医疗集团');
IF NOT EXISTS (SELECT 1 FROM mdm.Organizations WHERE Code = N'HUADONG_GROUP')
    INSERT INTO mdm.Organizations (Code, Name) VALUES (N'HUADONG_GROUP', N'华东医疗集团');

DECLARE @OrgId1 BIGINT = (SELECT Id FROM mdm.Organizations WHERE Code = N'DEMO_ORG');
DECLARE @OrgId2 BIGINT = (SELECT Id FROM mdm.Organizations WHERE Code = N'RENJI_GROUP');
DECLARE @OrgId3 BIGINT = (SELECT Id FROM mdm.Organizations WHERE Code = N'HUADONG_GROUP');

/* ---------- mdm.Campuses ---------- */
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'ZONGYUAN')
    INSERT INTO mdm.Campuses (OrganizationId, Code, Name) VALUES (@OrgId1, N'ZONGYUAN', N'总院');
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'DONGYUAN')
    INSERT INTO mdm.Campuses (OrganizationId, Code, Name) VALUES (@OrgId1, N'DONGYUAN', N'东院');
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'XIYUAN')
    INSERT INTO mdm.Campuses (OrganizationId, Code, Name) VALUES (@OrgId2, N'XIYUAN', N'西院');
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'NANYUAN')
    INSERT INTO mdm.Campuses (OrganizationId, Code, Name) VALUES (@OrgId2, N'NANYUAN', N'南院');
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'BEIYUAN')
    INSERT INTO mdm.Campuses (OrganizationId, Code, Name) VALUES (@OrgId2, N'BEIYUAN', N'北院');
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'GU_BEI')
    INSERT INTO mdm.Campuses (OrganizationId, Code, Name) VALUES (@OrgId3, N'GU_BEI', N'古北院区');
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'PU_DONG')
    INSERT INTO mdm.Campuses (OrganizationId, Code, Name) VALUES (@OrgId3, N'PU_DONG', N'浦东院区');

DECLARE @Campus1 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'ZONGYUAN'); -- DEMO_CAMPUS might be ZONGYUAN, check
DECLARE @Campus2 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'DONGYUAN');
DECLARE @Campus3 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'XIYUAN');
DECLARE @Campus4 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'NANYUAN');
DECLARE @Campus5 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'BEIYUAN');
DECLARE @Campus6 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'GU_BEI');
DECLARE @Campus7 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'PU_DONG');

/* ---------- mdm.Departments (科室树) ---------- */
-- 根科室已在 900 中创建，此处补充层级结构
DECLARE @RootDept BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus1);

MERGE INTO mdm.Departments AS t
USING (VALUES
    (@Campus1, @RootDept, N'NK',      N'内科',        N'Clinical', 1),
    (@Campus1, @RootDept, N'WK',      N'外科',        N'Clinical', 1),
    (@Campus1, @RootDept, N'EK',      N'儿科',        N'Clinical', 1),
    (@Campus1, @RootDept, N'FK',      N'妇产科',      N'Clinical', 1),
    (@Campus1, @RootDept, N'GUK',     N'骨科',        N'Clinical', 1),
    (@Campus1, @RootDept, N'KFK',     N'康复科',      N'Clinical', 1),
    (@Campus1, @RootDept, N'YPK',     N'药品科',      N'Pharmacy', 0),
    (@Campus1, @RootDept, N'JYK',     N'检验科',      N'Lab',      0),
    (@Campus1, @RootDept, N'FSK',     N'放射科',      N'Radiology',0),
    (@Campus1, @RootDept, N'SFK',     N'收费处',      N'Admin',    0),
    (@Campus1, @RootDept, N'GHS',     N'挂号室',      N'Admin',    0),
    (@Campus1, @RootDept, N'JZ',      N'急诊科',      N'Clinical', 1),
    (@Campus1, @RootDept, N'PFK',     N'皮肤科',      N'Clinical', 1),
    (@Campus1, @RootDept, N'YK',      N'眼科',        N'Clinical', 1),
    (@Campus1, @RootDept, N'EBHK',    N'耳鼻喉科',    N'Clinical', 1)
) AS s(CampusId, ParentId, Code, Name, DeptType, IsClinical)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, ParentId, Code, Name, DeptType, IsClinical) VALUES (s.CampusId, s.ParentId, s.Code, s.Name, s.DeptType, s.IsClinical);

-- 内科子科室
DECLARE @NK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'NK' AND CampusId = @Campus1);
MERGE INTO mdm.Departments AS t
USING (VALUES
    (@Campus1, @NK, N'NK_XH',  N'心血管内科', N'Clinical', 1),
    (@Campus1, @NK, N'NK_HX',  N'呼吸内科',   N'Clinical', 1),
    (@Campus1, @NK, N'NK_XH2', N'消化内科',   N'Clinical', 1),
    (@Campus1, @NK, N'NK_SN',  N'肾内科',     N'Clinical', 1),
    (@Campus1, @NK, N'NK_NF',  N'内分泌科',   N'Clinical', 1),
    (@Campus1, @NK, N'NK_SJ',  N'神经内科',   N'Clinical', 1)
) AS s(CampusId, ParentId, Code, Name, DeptType, IsClinical)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, ParentId, Code, Name, DeptType, IsClinical) VALUES (s.CampusId, s.ParentId, s.Code, s.Name, s.DeptType, s.IsClinical);

-- 外科子科室
DECLARE @WK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'WK' AND CampusId = @Campus1);
MERGE INTO mdm.Departments AS t
USING (VALUES
    (@Campus1, @WK, N'WK_PT',  N'普外科',     N'Clinical', 1),
    (@Campus1, @WK, N'WK_XW',  N'胸外科',     N'Clinical', 1),
    (@Campus1, @WK, N'WK_NS',  N'脑外科',     N'Clinical', 1),
    (@Campus1, @WK, N'WK_MN',  N'泌尿外科',   N'Clinical', 1)
) AS s(CampusId, ParentId, Code, Name, DeptType, IsClinical)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, ParentId, Code, Name, DeptType, IsClinical) VALUES (s.CampusId, s.ParentId, s.Code, s.Name, s.DeptType, s.IsClinical);

-- 妇产科子科室
DECLARE @FK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'FK' AND CampusId = @Campus1);
MERGE INTO mdm.Departments AS t
USING (VALUES
    (@Campus1, @FK, N'FK_FC',  N'妇科产科',   N'Clinical', 1),
    (@Campus1, @FK, N'FK_CC',  N'产科',       N'Clinical', 1)
) AS s(CampusId, ParentId, Code, Name, DeptType, IsClinical)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, ParentId, Code, Name, DeptType, IsClinical) VALUES (s.CampusId, s.ParentId, s.Code, s.Name, s.DeptType, s.IsClinical);

-- 东院复制部分科室
DECLARE @C2Root BIGINT;
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus2)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsClinical) VALUES (@Campus2, NULL, N'ROOT', N'东院根科室', N'Admin', 0);
SET @C2Root = (SELECT Id FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus2);

MERGE INTO mdm.Departments AS t
USING (VALUES
    (@Campus2, @C2Root, N'NK',   N'内科',     N'Clinical', 1),
    (@Campus2, @C2Root, N'WK',   N'外科',     N'Clinical', 1),
    (@Campus2, @C2Root, N'EK',   N'儿科',     N'Clinical', 1),
    (@Campus2, @C2Root, N'GUK',  N'骨科',     N'Clinical', 1),
    (@Campus2, @C2Root, N'JYK',  N'检验科',   N'Lab',      0),
    (@Campus2, @C2Root, N'FSK',  N'放射科',   N'Radiology',0),
    (@Campus2, @C2Root, N'YPK',  N'药品科',   N'Pharmacy', 0),
    (@Campus2, @C2Root, N'SFK',  N'收费处',   N'Admin',    0)
) AS s(CampusId, ParentId, Code, Name, DeptType, IsClinical)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, ParentId, Code, Name, DeptType, IsClinical) VALUES (s.CampusId, s.ParentId, s.Code, s.Name, s.DeptType, s.IsClinical);

/* ---------- mdm.Wards ---------- */
MERGE INTO mdm.Wards AS t
USING (VALUES
    (@Campus1, (SELECT Id FROM mdm.Departments WHERE Code = N'NK_XH' AND CampusId = @Campus1), N'W_NK_XH', N'心血管内科病区'),
    (@Campus1, (SELECT Id FROM mdm.Departments WHERE Code = N'WK_PT' AND CampusId = @Campus1), N'W_WK_PT', N'普外科病区'),
    (@Campus1, (SELECT Id FROM mdm.Departments WHERE Code = N'GUK' AND CampusId = @Campus1), N'W_GUK',    N'骨科病区'),
    (@Campus1, (SELECT Id FROM mdm.Departments WHERE Code = N'FK_FC' AND CampusId = @Campus1), N'W_FK',     N'妇产科病区'),
    (@Campus1, (SELECT Id FROM mdm.Departments WHERE Code = N'EK' AND CampusId = @Campus1), N'W_EK',     N'儿科病区'),
    (@Campus1, (SELECT Id FROM mdm.Departments WHERE Code = N'JZ' AND CampusId = @Campus1), N'W_JZ',     N'急诊观察病区'),
    (@Campus2, (SELECT Id FROM mdm.Departments WHERE Code = N'NK' AND CampusId = @Campus2), N'W_NK',    N'内科病区'),
    (@Campus2, (SELECT Id FROM mdm.Departments WHERE Code = N'WK' AND CampusId = @Campus2), N'W_WK',    N'外科病区')
) AS s(CampusId, DepartmentId, Code, Name)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, DepartmentId, Code, Name) VALUES (s.CampusId, s.DepartmentId, s.Code, s.Name);

/* ---------- mdm.Beds ---------- */
-- 为每个病区生成 10-20 张床位
DECLARE @WardId BIGINT, @BedCode NVARCHAR(32), @i INT;

DECLARE ward_cursor CURSOR LOCAL FOR SELECT Id FROM mdm.Wards WHERE IsDeleted = 0;
OPEN ward_cursor;
FETCH NEXT FROM ward_cursor INTO @WardId;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @i = 1;
    WHILE @i <= 12
    BEGIN
        SET @BedCode = RIGHT('00' + CAST(@i AS NVARCHAR), 2);
        IF NOT EXISTS (SELECT 1 FROM mdm.Beds WHERE WardId = @WardId AND BedNo = @BedCode)
            INSERT INTO mdm.Beds (WardId, BedNo, Status) VALUES (@WardId, @BedCode, N'Empty');
        SET @i = @i + 1;
    END
    FETCH NEXT FROM ward_cursor INTO @WardId;
END
CLOSE ward_cursor;
DEALLOCATE ward_cursor;

/* ---------- mdm.Staff ---------- */
MERGE INTO mdm.Staff AS t
USING (VALUES
    (@Campus1, N'E0001', N'系统管理员', N'Admin',     NULL, NULL),
    (@Campus1, N'E0002', N'张伟',       N'Doctor',    N'110101199001011234', '20281231'),
    (@Campus1, N'E0003', N'李娜',       N'Doctor',    N'110101198505152345', '20271231'),
    (@Campus1, N'E0004', N'王强',       N'Doctor',    N'110101197803031456', '20261231'),
    (@Campus1, N'E0005', N'赵敏',       N'Doctor',    N'110101198812122367', '20291231'),
    (@Campus1, N'E0006', N'刘洋',       N'Nurse',     N'110101199205056789', '20301231'),
    (@Campus1, N'E0007', N'陈芳',       N'Nurse',     N'110101199103152890', '20281231'),
    (@Campus1, N'E0008', N'杨静',       N'Nurse',     N'110101199407082345', '20311231'),
    (@Campus1, N'E0009', N'黄勇',       N'Doctor',    N'110101198612093456', '20271231'),
    (@Campus1, N'E0010', N'周磊',       N'Doctor',    N'110101198209214567', '20261231'),
    (@Campus1, N'E0011', N'吴秀英',     N'Nurse',     N'110101199508181234', '20321231'),
    (@Campus1, N'E0012', N'孙卫东',     N'Doctor',    N'110101197611112345', '20281231'),
    (@Campus1, N'E0013', N'马丽',       N'Doctor',    N'110101198310055678', '20291231'),
    (@Campus1, N'E0014', N'朱建国',     N'Doctor',    N'110101198707162345', '20271231'),
    (@Campus1, N'E0015', N'徐婷',       N'Nurse',     N'110101199609092378', '20321231'),
    (@Campus1, N'E0016', N'何大明',     N'Cashier',   NULL, NULL),
    (@Campus1, N'E0017', N'林小红',     N'Cashier',   NULL, NULL),
    (@Campus1, N'E0018', N'郑建平',     N'Admin',     NULL, NULL),
    (@Campus1, N'E0019', N'梁晓东',     N'Pharmacy',  NULL, NULL),
    (@Campus1, N'E0020', N'唐敏',       N'Pharmacy',  NULL, NULL),
    (@Campus1, N'E0021', N'沈佳',       N'Doctor',    N'110101199011112345', '20281231'),
    (@Campus1, N'E0022', N'韩冰',       N'Doctor',    N'110101198812224567', '20291231'),
    (@Campus1, N'E0023', N'曹阳',       N'Nurse',     N'110101199307153456', '20301231'),
    (@Campus2, N'E0101', N'王磊',       N'Doctor',    N'310101198009011234', '20271231'),
    (@Campus2, N'E0102', N'张丽华',     N'Nurse',     N'310101199105152345', '20311231'),
    (@Campus2, N'E0103', N'李刚',       N'Doctor',    N'310101197512123456', '20261231')
) AS s(CampusId, EmployeeNo, FullName, StaffCategory, LicenseNo, LicenseExpireDate)
ON t.CampusId = s.CampusId AND t.EmployeeNo = s.EmployeeNo
WHEN NOT MATCHED THEN INSERT (CampusId, EmployeeNo, FullName, StaffCategory, LicenseNo, LicenseExpireDate)
    VALUES (s.CampusId, s.EmployeeNo, s.FullName, s.StaffCategory, s.LicenseNo, TRY_CAST(s.LicenseExpireDate AS DATE));

DECLARE @StaffAdmin    BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0001' AND CampusId = @Campus1);
DECLARE @StaffDoc1     BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0002' AND CampusId = @Campus1);
DECLARE @StaffDoc2     BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0003' AND CampusId = @Campus1);
DECLARE @StaffDoc3     BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0004' AND CampusId = @Campus1);
DECLARE @StaffDoc4     BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0005' AND CampusId = @Campus1);
DECLARE @StaffDoc5     BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0009' AND CampusId = @Campus1);
DECLARE @StaffDoc6     BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0010' AND CampusId = @Campus1);
DECLARE @StaffNurse1   BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0006' AND CampusId = @Campus1);
DECLARE @StaffNurse2   BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0007' AND CampusId = @Campus1);
DECLARE @StaffNurse3   BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0008' AND CampusId = @Campus1);
DECLARE @StaffCashier1 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0016' AND CampusId = @Campus1);
DECLARE @StaffCashier2 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0017' AND CampusId = @Campus1);

/* ---------- mdm.StaffDepartments ---------- */
MERGE INTO mdm.StaffDepartments AS t
USING (VALUES
    (@StaffAdmin,    (SELECT Id FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus1), 1),
    (@StaffDoc1,     (SELECT Id FROM mdm.Departments WHERE Code = N'NK_XH' AND CampusId = @Campus1), 1),
    (@StaffDoc2,     (SELECT Id FROM mdm.Departments WHERE Code = N'NK_HX' AND CampusId = @Campus1), 1),
    (@StaffDoc3,     (SELECT Id FROM mdm.Departments WHERE Code = N'WK_PT' AND CampusId = @Campus1), 1),
    (@StaffDoc4,     (SELECT Id FROM mdm.Departments WHERE Code = N'GUK' AND CampusId = @Campus1),   1),
    (@StaffDoc5,     (SELECT Id FROM mdm.Departments WHERE Code = N'FK_FC' AND CampusId = @Campus1), 1),
    (@StaffDoc6,     (SELECT Id FROM mdm.Departments WHERE Code = N'EK' AND CampusId = @Campus1),    1),
    (@StaffNurse1,   (SELECT Id FROM mdm.Departments WHERE Code = N'WK_PT' AND CampusId = @Campus1), 1),
    (@StaffNurse2,   (SELECT Id FROM mdm.Departments WHERE Code = N'NK_XH' AND CampusId = @Campus1), 1),
    (@StaffNurse3,   (SELECT Id FROM mdm.Departments WHERE Code = N'FK_FC' AND CampusId = @Campus1), 1),
    (@StaffCashier1, (SELECT Id FROM mdm.Departments WHERE Code = N'SFK' AND CampusId = @Campus1),   1),
    (@StaffCashier2, (SELECT Id FROM mdm.Departments WHERE Code = N'SFK' AND CampusId = @Campus1),   1)
) AS s(StaffId, DepartmentId, IsPrimary)
ON t.StaffId = s.StaffId AND t.DepartmentId = s.DepartmentId AND t.IsPrimary = 1
WHEN NOT MATCHED THEN INSERT (StaffId, DepartmentId, IsPrimary) VALUES (s.StaffId, s.DepartmentId, s.IsPrimary);

/* ---------- mdm.DictionaryTypes ---------- */
MERGE INTO mdm.DictionaryTypes AS t
USING (VALUES
    (N'GENDER',        N'性别',       1),
    (N'SPECIMEN',      N'标本类型',   1),
    (N'FREQUENCY',     N'用药频次',   1),
    (N'ROUTE',         N'给药途径',   1),
    (N'DIAGNOSIS_TYPE',N'诊断类别',   1),
    (N'DEPT_TYPE',     N'科室类型',   1),
    (N'SCHEDULE_TYPE', N'号别类型',   1),
    (N'CHARGE_CAT',    N'收费分类',   1),
    (N'PAY_METHOD',    N'支付方式',   1),
    (N'STAFF_CAT',     N'人员类别',   1),
    (N'BED_STATUS',    N'床位状态',   1),
    (N'ENC_STATUS',    N'就诊状态',   1)
) AS s(Code, Name, IsSystem)
ON t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (Code, Name, IsSystem) VALUES (s.Code, s.Name, s.IsSystem);

/* ---------- mdm.DictionaryItems ---------- */
-- 先清空已存在的 GENDER 项保证完整
MERGE INTO mdm.DictionaryItems AS t
USING (VALUES
    (N'GENDER', N'M',    N'男',   1),
    (N'GENDER', N'F',    N'女',   2),
    (N'GENDER', N'U',    N'未知', 3),
    (N'SPECIMEN', N'BLOOD',     N'血液',   1),
    (N'SPECIMEN', N'URINE',     N'尿液',   2),
    (N'SPECIMEN', N'STOOL',     N'粪便',   3),
    (N'SPECIMEN', N'SPUTUM',    N'痰液',   4),
    (N'SPECIMEN', N'CSF',       N'脑脊液', 5),
    (N'SPECIMEN', N'TISSUE',    N'组织',   6),
    (N'SPECIMEN', N'SWAB',      N'拭子',   7),
    (N'FREQUENCY', N'QD',   N'每日一次',   1),
    (N'FREQUENCY', N'BID',  N'每日两次',   2),
    (N'FREQUENCY', N'TID',  N'每日三次',   3),
    (N'FREQUENCY', N'QID',  N'每日四次',   4),
    (N'FREQUENCY', N'Q6H',  N'每6小时一次',5),
    (N'FREQUENCY', N'Q8H',  N'每8小时一次',6),
    (N'FREQUENCY', N'Q12H', N'每12小时一次',7),
    (N'FREQUENCY', N'QN',   N'每晚一次',   8),
    (N'FREQUENCY', N'PRN',  N'必要时',     9),
    (N'FREQUENCY', N'STAT', N'立即',       10),
    (N'ROUTE', N'PO',  N'口服',       1),
    (N'ROUTE', N'IV',  N'静脉注射',   2),
    (N'ROUTE', N'IM',  N'肌肉注射',   3),
    (N'ROUTE', N'IH',  N'皮下注射',   4),
    (N'ROUTE', N'TOP', N'外用',       5),
    (N'ROUTE', N'PR',  N'直肠给药',   6),
    (N'ROUTE', N'SL',  N'舌下含服',   7),
    (N'ROUTE', N'INH', N'吸入',       8),
    (N'DIAGNOSIS_TYPE', N'ADMIT',  N'入院诊断',   1),
    (N'DIAGNOSIS_TYPE', N'DISCHARGE', N'出院诊断',2),
    (N'DIAGNOSIS_TYPE', N'OUTPATIENT', N'门诊诊断',3),
    (N'DIAGNOSIS_TYPE', N'OPERATION', N'手术诊断', 4),
    (N'DEPT_TYPE', N'Clinical',  N'临床科室',   1),
    (N'DEPT_TYPE', N'Admin',     N'行政科室',   2),
    (N'DEPT_TYPE', N'Lab',       N'检验科室',   3),
    (N'DEPT_TYPE', N'Radiology', N'放射科室',   4),
    (N'DEPT_TYPE', N'Pharmacy',  N'药剂科室',   5),
    (N'SCHEDULE_TYPE', N'REG',   N'普通号',     1),
    (N'SCHEDULE_TYPE', N'EXP',   N'专家号',     2),
    (N'SCHEDULE_TYPE', N'VIP',   N'特需号',     3),
    (N'SCHEDULE_TYPE', N'EMG',   N'急诊号',     4),
    (N'CHARGE_CAT', N'Registration',  N'挂号费',     1),
    (N'CHARGE_CAT', N'Consultation',  N'诊查费',     2),
    (N'CHARGE_CAT', N'Lab',           N'检验费',     3),
    (N'CHARGE_CAT', N'Imaging',       N'检查费',     4),
    (N'CHARGE_CAT', N'Medicine',      N'药品费',     5),
    (N'CHARGE_CAT', N'Treatment',     N'治疗费',     6),
    (N'CHARGE_CAT', N'Operation',     N'手术费',     7),
    (N'CHARGE_CAT', N'Bed',           N'床位费',     8),
    (N'CHARGE_CAT', N'Other',         N'其他',       9),
    (N'PAY_METHOD', N'CASH',     N'现金',     1),
    (N'PAY_METHOD', N'WECHAT',   N'微信支付', 2),
    (N'PAY_METHOD', N'ALIPAY',   N'支付宝',   3),
    (N'PAY_METHOD', N'MEDICARE', N'医保',     4),
    (N'PAY_METHOD', N'CARD',     N'银行卡',   5),
    (N'PAY_METHOD', N'UNIONPAY', N'银联',     6),
    (N'STAFF_CAT', N'Doctor',   N'医生',   1),
    (N'STAFF_CAT', N'Nurse',    N'护士',   2),
    (N'STAFF_CAT', N'Admin',    N'行政',   3),
    (N'STAFF_CAT', N'Cashier',  N'收费员', 4),
    (N'STAFF_CAT', N'Pharmacy', N'药剂师', 5),
    (N'STAFF_CAT', N'Tech',     N'技师',   6),
    (N'BED_STATUS', N'Empty',        N'空床',     1),
    (N'BED_STATUS', N'Occupied',     N'占用',     2),
    (N'BED_STATUS', N'Maintenance',  N'维护',     3),
    (N'BED_STATUS', N'Reserved',     N'预留',     4),
    (N'ENC_STATUS', N'Open',     N'接诊中',   1),
    (N'ENC_STATUS', N'Closed',   N'已结束',   2),
    (N'ENC_STATUS', N'Cancelled',N'已取消',   3)
) AS s(TypeCode, Value, DisplayName, SortOrder)
ON t.TypeCode = s.TypeCode AND t.Value = s.Value
WHEN NOT MATCHED THEN INSERT (TypeCode, Value, DisplayName, SortOrder) VALUES (s.TypeCode, s.Value, s.DisplayName, s.SortOrder);

/* ---------- mdm.ChargeItems ---------- */
DECLARE @RegFeeItem BIGINT = (SELECT Id FROM mdm.ChargeItems WHERE Code = N'REG_FEE' AND CampusId = @Campus1);

MERGE INTO mdm.ChargeItems AS t
USING (VALUES
    (@Campus1, N'REG_FEE',    N'普通挂号费',   N'次', 10.00,  N'Registration'),
    (@Campus1, N'EXP_FEE',    N'专家挂号费',   N'次', 30.00,  N'Registration'),
    (@Campus1, N'VIP_FEE',    N'特需挂号费',   N'次', 100.00, N'Registration'),
    (@Campus1, N'CONS_FEE',   N'普通诊查费',   N'次', 15.00,  N'Consultation'),
    (@Campus1, N'EXP_CONS',   N'专家诊查费',   N'次', 50.00,  N'Consultation'),
    (@Campus1, N'CBC',        N'血常规检查',   N'次', 25.00,  N'Lab'),
    (@Campus1, N'URINALYSIS', N'尿常规检查',   N'次', 15.00,  N'Lab'),
    (@Campus1, N'LIVER_FUNC', N'肝功能检查',   N'次', 80.00,  N'Lab'),
    (@Campus1, N'RENAL_FUNC', N'肾功能检查',   N'次', 60.00,  N'Lab'),
    (@Campus1, N'BLOOD_SUGAR',N'血糖测定',     N'次', 10.00,  N'Lab'),
    (@Campus1, N'CHEST_XRAY', N'胸部X线检查',  N'次', 80.00,  N'Imaging'),
    (@Campus1, N'ABDOMEN_CT', N'腹部CT检查',   N'次', 350.00, N'Imaging'),
    (@Campus1, N'HEAD_CT',    N'头颅CT检查',   N'次', 300.00, N'Imaging'),
    (@Campus1, N'MRI_LUMBAR', N'腰椎MRI检查',  N'次', 600.00, N'Imaging'),
    (@Campus1, N'ECG',        N'心电图检查',   N'次', 30.00,  N'Imaging'),
    (@Campus1, N'BED_GEN',    N'普通床位费',   N'天', 60.00,  N'Bed'),
    (@Campus1, N'BED_VIP',    N'VIP床位费',    N'天', 200.00, N'Bed'),
    (@Campus1, N'DRG_001',    N'阿莫西林胶囊', N'盒', 12.50,  N'Medicine'),
    (@Campus1, N'DRG_002',    N'头孢克肟片',   N'盒', 25.00,  N'Medicine'),
    (@Campus1, N'DRG_003',    N'布洛芬缓释胶囊',N'盒', 15.80, N'Medicine'),
    (@Campus1, N'DRG_004',    N'硝苯地平片',   N'瓶', 8.50,   N'Medicine'),
    (@Campus1, N'DRG_005',    N'阿托伐他汀钙片',N'盒', 35.00, N'Medicine'),
    (@Campus1, N'DRG_006',    N'盐酸二甲双胍片',N'盒', 18.00, N'Medicine'),
    (@Campus1, N'DRG_007',    N'奥美拉唑肠溶胶囊',N'盒',22.00, N'Medicine'),
    (@Campus1, N'DRG_008',    N'氯沙坦钾片',   N'盒', 28.00,  N'Medicine'),
    (@Campus1, N'DRG_009',    N'左氧氟沙星片', N'盒', 20.00,  N'Medicine'),
    (@Campus1, N'DRG_010',    N'盐酸氨溴索片', N'盒', 16.00,  N'Medicine'),
    (@Campus1, N'TREAT_001',  N'换药费',       N'次', 20.00,  N'Treatment'),
    (@Campus1, N'TREAT_002',  N'清创缝合费',   N'次', 100.00, N'Treatment')
) AS s(CampusId, Code, Name, Unit, Price, Category)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, Code, Name, Unit, Price, Category) VALUES (s.CampusId, s.Code, s.Name, s.Unit, s.Price, s.Category);

/* =========================================================================
   第二部分：安全与权限 (SEC)
   ========================================================================= */

/* ---------- sec.Users ---------- */
-- 密码哈希占位符，生产需替换为真实 bcrypt 哈希
MERGE INTO sec.Users AS t
USING (VALUES
    (N'admin',   N'__REPLACE_WITH_REAL_HASH__', N'系统管理员', @StaffAdmin),
    (N'doctor1', N'__REPLACE_WITH_REAL_HASH__', N'张伟医生',   @StaffDoc1),
    (N'doctor2', N'__REPLACE_WITH_REAL_HASH__', N'李娜医生',   @StaffDoc2),
    (N'doctor3', N'__REPLACE_WITH_REAL_HASH__', N'王强医生',   @StaffDoc3),
    (N'doctor4', N'__REPLACE_WITH_REAL_HASH__', N'赵敏医生',   @StaffDoc4),
    (N'doctor5', N'__REPLACE_WITH_REAL_HASH__', N'黄勇医生',   @StaffDoc5),
    (N'doctor6', N'__REPLACE_WITH_REAL_HASH__', N'周磊医生',   @StaffDoc6),
    (N'nurse1',  N'__REPLACE_WITH_REAL_HASH__', N'刘洋护士',   @StaffNurse1),
    (N'nurse2',  N'__REPLACE_WITH_REAL_HASH__', N'陈芳护士',   @StaffNurse2),
    (N'nurse3',  N'__REPLACE_WITH_REAL_HASH__', N'杨静护士',   @StaffNurse3),
    (N'cashier1',N'__REPLACE_WITH_REAL_HASH__', N'何大明',     @StaffCashier1),
    (N'cashier2',N'__REPLACE_WITH_REAL_HASH__', N'林小红',     @StaffCashier2)
) AS s(LoginName, PasswordHash, DisplayName, StaffId)
ON t.LoginName = s.LoginName
WHEN NOT MATCHED THEN INSERT (LoginName, PasswordHash, DisplayName, StaffId) VALUES (s.LoginName, s.PasswordHash, s.DisplayName, s.StaffId);

DECLARE @UserIdAdmin   BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'admin');
DECLARE @UserIdDoc1    BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'doctor1');
DECLARE @UserIdDoc2    BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'doctor2');
DECLARE @UserIdDoc3    BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'doctor3');
DECLARE @UserIdNurse1  BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'nurse1');
DECLARE @UserIdCashier1 BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'cashier1');

/* ---------- sec.Permissions ---------- */
MERGE INTO sec.Permissions AS t
USING (VALUES
    (N'sys.shell.use',     N'sys', N'使用主壳'),
    (N'mdm.campus.manage', N'mdm', N'院区管理'),
    (N'mdm.dept.manage',   N'mdm', N'科室管理'),
    (N'mdm.staff.manage',  N'mdm', N'人员管理'),
    (N'mdm.dict.manage',   N'mdm', N'字典管理'),
    (N'opd.schedule',      N'opd', N'排班管理'),
    (N'opd.register',      N'opd', N'挂号操作'),
    (N'opd.register.work', N'opd', N'挂号工作台'),
    (N'enc.workbench',     N'enc', N'接诊工作台'),
    (N'enc.diagnosis',     N'enc', N'诊断录入'),
    (N'enc.prescribe',     N'enc', N'开立处方'),
    (N'enc.laborder',      N'enc', N'开立检验'),
    (N'enc.imagingorder',  N'enc', N'开立检查'),
    (N'pha.dispense',      N'pha', N'发药操作'),
    (N'fin.cashier',       N'fin', N'收费操作'),
    (N'ipd.admission',     N'ipd', N'住院管理'),
    (N'ipd.order',         N'ipd', N'住院医嘱'),
    (N'sec.userrole',      N'sec', N'用户角色管理')
) AS s(Code, Module, Description)
ON t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (Code, Module, Description) VALUES (s.Code, s.Module, s.Description);

/* ---------- sec.Roles ---------- */
MERGE INTO sec.Roles AS t
USING (VALUES
    (@Campus1, N'ADMIN',      N'系统管理员'),
    (@Campus1, N'DOCTOR',     N'门诊医生'),
    (@Campus1, N'NURSE',      N'护士'),
    (@Campus1, N'CASHIER',    N'收费员'),
    (@Campus1, N'PHARMACY',   N'药剂师'),
    (@Campus2, N'ADMIN',      N'东院管理员'),
    (@Campus2, N'DOCTOR',     N'东院医生')
) AS s(CampusId, Code, Name)
ON t.CampusId = s.CampusId AND t.Code = s.Code
WHEN NOT MATCHED THEN INSERT (CampusId, Code, Name) VALUES (s.CampusId, s.Code, s.Name);

DECLARE @RoleAdmin    BIGINT = (SELECT Id FROM sec.Roles WHERE Code = N'ADMIN'   AND CampusId = @Campus1);
DECLARE @RoleDoctor   BIGINT = (SELECT Id FROM sec.Roles WHERE Code = N'DOCTOR'  AND CampusId = @Campus1);
DECLARE @RoleNurse    BIGINT = (SELECT Id FROM sec.Roles WHERE Code = N'NURSE'   AND CampusId = @Campus1);
DECLARE @RoleCashier  BIGINT = (SELECT Id FROM sec.Roles WHERE Code = N'CASHIER' AND CampusId = @Campus1);
DECLARE @RolePharmacy BIGINT = (SELECT Id FROM sec.Roles WHERE Code = N'PHARMACY' AND CampusId = @Campus1);

/* ---------- sec.RolePermissions ---------- */
-- ADMIN: 所有权限
INSERT INTO sec.RolePermissions (RoleId, PermissionId)
SELECT @RoleAdmin, p.Id FROM sec.Permissions p
WHERE NOT EXISTS (SELECT 1 FROM sec.RolePermissions rp WHERE rp.RoleId = @RoleAdmin AND rp.PermissionId = p.Id);

-- DOCTOR: 接诊、诊断、处方、检验、检查
DECLARE @DocPerms TABLE (Code NVARCHAR(200));
INSERT INTO @DocPerms VALUES (N'sys.shell.use'),(N'enc.workbench'),(N'enc.diagnosis'),(N'enc.prescribe'),(N'enc.laborder'),(N'enc.imagingorder'),(N'opd.schedule');
INSERT INTO sec.RolePermissions (RoleId, PermissionId)
SELECT @RoleDoctor, p.Id FROM sec.Permissions p
INNER JOIN @DocPerms dp ON p.Code = dp.Code
WHERE NOT EXISTS (SELECT 1 FROM sec.RolePermissions rp WHERE rp.RoleId = @RoleDoctor AND rp.PermissionId = p.Id);

-- NURSE: 发药、接诊
INSERT INTO sec.RolePermissions (RoleId, PermissionId)
SELECT @RoleNurse, p.Id FROM sec.Permissions p
WHERE p.Code IN (N'sys.shell.use', N'pha.dispense', N'enc.workbench')
AND NOT EXISTS (SELECT 1 FROM sec.RolePermissions rp WHERE rp.RoleId = @RoleNurse AND rp.PermissionId = p.Id);

-- CASHIER: 收费
INSERT INTO sec.RolePermissions (RoleId, PermissionId)
SELECT @RoleCashier, p.Id FROM sec.Permissions p
WHERE p.Code IN (N'sys.shell.use', N'fin.cashier')
AND NOT EXISTS (SELECT 1 FROM sec.RolePermissions rp WHERE rp.RoleId = @RoleCashier AND rp.PermissionId = p.Id);

-- PHARMACY: 发药
INSERT INTO sec.RolePermissions (RoleId, PermissionId)
SELECT @RolePharmacy, p.Id FROM sec.Permissions p
WHERE p.Code IN (N'sys.shell.use', N'pha.dispense')
AND NOT EXISTS (SELECT 1 FROM sec.RolePermissions rp WHERE rp.RoleId = @RolePharmacy AND rp.PermissionId = p.Id);

/* ---------- sec.UserRoles ---------- */
MERGE INTO sec.UserRoles AS t
USING (VALUES
    (@UserIdAdmin,   @RoleAdmin,   @Campus1),
    (@UserIdDoc1,    @RoleDoctor,  @Campus1),
    (@UserIdDoc2,    @RoleDoctor,  @Campus1),
    (@UserIdDoc3,    @RoleDoctor,  @Campus1),
    (@UserIdNurse1,  @RoleNurse,   @Campus1),
    (@UserIdCashier1,@RoleCashier, @Campus1)
) AS s(UserId, RoleId, CampusId)
ON t.UserId = s.UserId AND t.RoleId = s.RoleId AND t.CampusId = s.CampusId
WHEN NOT MATCHED THEN INSERT (UserId, RoleId, CampusId) VALUES (s.UserId, s.RoleId, s.CampusId);

/* ---------- sec.SystemParameters ---------- */
MERGE INTO sec.SystemParameters AS t
USING (VALUES
    (@Campus1, N'SiteName',    N'仁济医院总院',         N'String'),
    (@Campus1, N'RegStartHour', N'8',                  N'Number'),
    (@Campus1, N'RegEndHour',   N'17',                 N'Number'),
    (@Campus1, N'MaxAdvanceDays', N'30',               N'Number'),
    (@Campus1, N'ApptCancelDeadlineHours', N'2',       N'Number'),
    (@Campus1, N'InsuranceEnabled', N'true',           N'String'),
    (@Campus1, N'EnableEMRSign', N'true',              N'String'),
    (@Campus1, N'PharmacyOpenHourStart', N'8:00',      N'String'),
    (@Campus1, N'PharmacyOpenHourEnd', N'18:00',       N'String'),
    (@Campus1, N'MaxDrugPerPrescription', N'5',        N'Number'),
    (NULL,      N'GlobalOrgName', N'演示医疗集团',      N'String'),
    (NULL,      N'DefaultTheme', N'Default',           N'String')
) AS s(CampusId, ParamKey, ParamValue, ValueType)
ON t.CampusId = s.CampusId AND t.ParamKey = s.ParamKey
WHEN NOT MATCHED THEN INSERT (CampusId, ParamKey, ParamValue, ValueType) VALUES (s.CampusId, s.ParamKey, s.ParamValue, s.ValueType);

/* ---------- sec.IntegrationEndpoints ---------- */
MERGE INTO sec.IntegrationEndpoints AS t
USING (VALUES
    (N'HIS-EMPI',     N'http://empi.hospital.local/api', N'Basic',    N'{"timeout":5000}', 1),
    (N'HIS-LIS',      N'http://lis.hospital.local/api',  N'Basic',    N'{"timeout":10000}',1),
    (N'HIS-PACS',     N'http://pacs.hospital.local/api', N'Basic',    N'{"timeout":15000}',1),
    (N'医保接口',     N'http://ins.hospital.local/api',  N'Certificate', N'{"cert":"medicare.p12"}',1),
    (N'电子发票',     N'http://einvoice.hospital.local', N'OAuth2',   N'{"clientId":"his_001"}',1),
    (N'短信网关',     N'http://sms.hospital.local/send', N'Basic',    N'{}', 1)
) AS s(Name, BaseUrl, AuthType, ConfigJson, IsActive)
ON t.Name = s.Name
WHEN NOT MATCHED THEN INSERT (Name, BaseUrl, AuthType, ConfigJson, IsActive) VALUES (s.Name, s.BaseUrl, s.AuthType, s.ConfigJson, s.IsActive);

/* ---------- sec.AuditLogs ---------- */
INSERT INTO sec.AuditLogs (UserId, Action, EntityType, EntityId, IpAddress)
SELECT @UserIdAdmin, N'Login',   N'Session', N'admin',     N'192.168.1.100' UNION ALL
SELECT @UserIdAdmin, N'Login',   N'Session', N'admin',     N'192.168.1.100' UNION ALL
SELECT @UserIdDoc1,  N'Login',   N'Session', N'doctor1',   N'192.168.1.101' UNION ALL
SELECT @UserIdDoc2,  N'Login',   N'Session', N'doctor2',   N'192.168.1.102' UNION ALL
SELECT @UserIdNurse1,N'Login',   N'Session', N'nurse1',    N'192.168.1.103' UNION ALL
SELECT @UserIdAdmin, N'Update',  N'User',    N'3',         N'192.168.1.100' UNION ALL
SELECT @UserIdAdmin, N'Create',  N'Role',    N'4',         N'192.168.1.100' UNION ALL
SELECT @UserIdAdmin, N'Update',  N'Patient', N'5',         N'192.168.1.100' UNION ALL
SELECT @UserIdDoc1,  N'Create',  N'Prescription', N'10',   N'192.168.1.101' UNION ALL
SELECT @UserIdCashier1, N'Login', N'Session', N'cashier1', N'192.168.1.104' UNION ALL
SELECT @UserIdCashier1, N'Create', N'Invoice', N'15',      N'192.168.1.104';
GO

/* =========================================================================
   第三部分：患者主索引 (PAT)
   ========================================================================= */

/* ---------- pat.Patients ---------- */
MERGE INTO pat.Patients AS t
USING (VALUES
    (N'P20250001', N'110101199003151234', N'张明',    N'M', '1990-03-15', N'13800138001', N'青霉素过敏'),
    (N'P20250002', N'110101198507202345', N'李芳',    N'F', '1985-07-20', N'13800138002', NULL),
    (N'P20250003', N'110101197811113456', N'王建国',  N'M', '1978-11-11', N'13800138003', N'磺胺类药物过敏'),
    (N'P20250004', N'110101199208084567', N'赵秀英',  N'F', '1992-08-08', N'13800138004', NULL),
    (N'P20250005', N'110101200105055678', N'刘浩然',  N'M', '2001-05-05', N'13800138005', NULL),
    (N'P20250006', N'110101196512256789', N'陈德明',  N'M', '1965-12-25', N'13800138006', N'阿司匹林过敏'),
    (N'P20250007', N'110101199509152345', N'杨雪',    N'F', '1995-09-15', N'13800138007', NULL),
    (N'P20250008', N'110101198203308901', N'黄海波',  N'M', '1982-03-30', N'13800138008', NULL),
    (N'P20250009', N'110101197609092345', N'周玉兰',  N'F', '1976-09-09', N'13800138009', N'头孢类过敏'),
    (N'P20250010', N'110101199808186789', N'吴磊',    N'M', '1998-08-18', N'13800138010', NULL),
    (N'P20250011', N'110101196704142345', N'孙桂英',  N'F', '1967-04-14', N'13800138011', NULL),
    (N'P20250012', N'110101198912305678', N'马超',    N'M', '1989-12-30', N'13800138012', N'海鲜过敏'),
    (N'P20250013', N'110101200306061234', N'朱丽丽',  N'F', '2003-06-06', N'13800138013', NULL),
    (N'P20250014', N'110101197412124567', N'徐志强',  N'M', '1974-12-12', N'13800138014', NULL),
    (N'P20250015', N'110101198608168901', N'何小红',  N'F', '1986-08-16', N'13800138015', NULL),
    (N'P20250016', N'110101199102287890', N'林枫',    N'M', '1991-02-28', N'13800138016', NULL),
    (N'P20250017', N'110101198410102345', N'唐敏华',  N'F', '1984-10-10', N'13800138017', NULL),
    (N'P20250018', N'110101200507071234', N'沈俊杰',  N'M', '2005-07-07', N'13800138018', NULL),
    (N'P20250019', N'110101197003204567', N'韩素芬',  N'F', '1970-03-20', N'13800138019', N'胰岛素过敏'),
    (N'P20250020', N'110101199712256789', N'曹阳',    N'M', '1997-12-25', N'13800138020', NULL)
) AS s(PatientNo, IdCardNo, Name, Gender, BirthDate, Phone, AllergiesText)
ON t.PatientNo = s.PatientNo
WHEN NOT MATCHED THEN INSERT (PatientNo, IdCardNo, Name, Gender, BirthDate, Phone, AllergiesText)
    VALUES (s.PatientNo, s.IdCardNo, s.Name, s.Gender, s.BirthDate, s.Phone, s.AllergiesText);

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

/* ---------- pat.PatientIdentifiers ---------- */
MERGE INTO pat.PatientIdentifiers AS t
USING (VALUES
    (@Pat1,  N'社保卡', N'SH110101199003151234', 1),
    (@Pat2,  N'社保卡', N'SH110101198507202345', 1),
    (@Pat3,  N'社保卡', N'SH110101197811113456', 1),
    (@Pat4,  N'社保卡', N'SH110101199208084567', 1),
    (@Pat5,  N'医保卡', N'YB110101200105055678', 1),
    (@Pat6,  N'社保卡', N'SH110101196512256789', 1),
    (@Pat7,  N'医保卡', N'YB110101199509152345', 1),
    (@Pat8,  N'社保卡', N'SH110101198203308901', 1),
    (@Pat9,  N'医保卡', N'YB110101197609092345', 1),
    (@Pat10, N'社保卡', N'SH110101199808186789', 1),
    (@Pat1,  N'就诊卡', N'HOS_CARD_00001', 0),
    (@Pat2,  N'就诊卡', N'HOS_CARD_00002', 0),
    (@Pat5,  N'就诊卡', N'HOS_CARD_00005', 0),
    (@Pat8,  N'就诊卡', N'HOS_CARD_00008', 0)
) AS s(PatientId, IdType, IdValue, IsPrimary)
ON t.PatientId = s.PatientId AND t.IdType = s.IdType AND t.IdValue = s.IdValue
WHEN NOT MATCHED THEN INSERT (PatientId, IdType, IdValue, IsPrimary) VALUES (s.PatientId, s.IdType, s.IdValue, s.IsPrimary);

/* ---------- pat.PatientConsents ---------- */
MERGE INTO pat.PatientConsents AS t
USING (VALUES
    (@Pat1, N'PrivacyPolicy',  DATEADD(DAY, -30, SYSUTCDATETIME()), NULL, NULL),
    (@Pat2, N'PrivacyPolicy',  DATEADD(DAY, -60, SYSUTCDATETIME()), NULL, NULL),
    (@Pat3, N'PrivacyPolicy',  DATEADD(DAY, -15, SYSUTCDATETIME()), NULL, NULL),
    (@Pat4, N'DataResearch',   DATEADD(DAY, -90, SYSUTCDATETIME()), DATEADD(YEAR, 1, SYSUTCDATETIME()), NULL),
    (@Pat5, N'PrivacyPolicy',  DATEADD(DAY, -45, SYSUTCDATETIME()), NULL, NULL),
    (@Pat6, N'PrivacyPolicy',  DATEADD(DAY, -120, SYSUTCDATETIME()), NULL, NULL),
    (@Pat7, N'DataResearch',   DATEADD(DAY, -20, SYSUTCDATETIME()), DATEADD(YEAR, 2, SYSUTCDATETIME()), NULL),
    (@Pat8, N'PrivacyPolicy',  DATEADD(DAY, -5, SYSUTCDATETIME()), NULL, NULL),
    (@Pat9, N'PrivacyPolicy',  DATEADD(DAY, -200, SYSUTCDATETIME()), NULL, NULL),
    (@Pat10, N'PrivacyPolicy', DATEADD(DAY, -80, SYSUTCDATETIME()), NULL, NULL)
) AS s(PatientId, ConsentType, GrantedAt, ExpiresAt, DocumentRef)
ON t.PatientId = s.PatientId AND t.ConsentType = s.ConsentType
WHEN NOT MATCHED THEN INSERT (PatientId, ConsentType, GrantedAt, ExpiresAt, DocumentRef)
    VALUES (s.PatientId, s.ConsentType, s.GrantedAt, s.ExpiresAt, s.DocumentRef);

/* =========================================================================
   第四部分：排班与挂号 (OPD)
   ========================================================================= */

/* ---------- opd.ScheduleTemplates ---------- */
DECLARE @DeptXH BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'NK_XH' AND CampusId = @Campus1);
DECLARE @DeptHX BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'NK_HX' AND CampusId = @Campus1);
DECLARE @DeptPT BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'WK_PT' AND CampusId = @Campus1);
DECLARE @DeptGU BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'GUK' AND CampusId = @Campus1);
DECLARE @DeptFK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'FK_FC' AND CampusId = @Campus1);
DECLARE @DeptEK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'EK' AND CampusId = @Campus1);

MERGE INTO opd.ScheduleTemplates AS t
USING (VALUES
    (@Campus1, @DeptXH, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE)), NULL, N'心血管内科常规排班'),
    (@Campus1, @DeptHX, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE)), NULL, N'呼吸内科常规排班'),
    (@Campus1, @DeptPT, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE)), NULL, N'普外科常规排班'),
    (@Campus1, @DeptGU, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE)), NULL, N'骨科常规排班'),
    (@Campus1, @DeptFK, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE)), NULL, N'妇产科常规排班'),
    (@Campus1, @DeptEK, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE)), NULL, N'儿科常规排班')
) AS s(CampusId, DepartmentId, EffectiveFrom, EffectiveTo, Notes)
ON t.CampusId = s.CampusId AND t.DepartmentId = s.DepartmentId AND t.Notes = s.Notes
WHEN NOT MATCHED THEN INSERT (CampusId, DepartmentId, EffectiveFrom, EffectiveTo, Notes)
    VALUES (s.CampusId, s.DepartmentId, s.EffectiveFrom, s.EffectiveTo, s.Notes);

DECLARE @TmplXH BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleTemplates WHERE DepartmentId = @DeptXH ORDER BY Id);
DECLARE @TmplHX BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleTemplates WHERE DepartmentId = @DeptHX ORDER BY Id);
DECLARE @TmplPT BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleTemplates WHERE DepartmentId = @DeptPT ORDER BY Id);
DECLARE @TmplGU BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleTemplates WHERE DepartmentId = @DeptGU ORDER BY Id);
DECLARE @TmplFK BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleTemplates WHERE DepartmentId = @DeptFK ORDER BY Id);
DECLARE @TmplEK BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleTemplates WHERE DepartmentId = @DeptEK ORDER BY Id);

/* ---------- opd.ScheduleSlots ---------- */
-- Generate slots for the last 14 days and next 14 days for each doctor
DECLARE @SlotDate DATE, @DayOfWeek INT, @TemplateId BIGINT, @StaffId BIGINT, @SlotType NVARCHAR(64);
DECLARE @SlotCount INT = 0;

DECLARE slot_cursor CURSOR LOCAL FOR
    SELECT t.Id, s.Id, st.SlotType
    FROM (VALUES
        (@TmplXH, @StaffDoc1, N'EXP'),
        (@TmplHX, @StaffDoc2, N'EXP'),
        (@TmplPT, @StaffDoc3, N'EXP'),
        (@TmplGU, @StaffDoc4, N'EXP'),
        (@TmplFK, @StaffDoc5, N'REG'),
        (@TmplEK, @StaffDoc6, N'REG'),
        (@TmplXH, @StaffDoc1, N'REG'),
        (@TmplHX, @StaffDoc2, N'REG')
    ) AS t(TemplateId, StaffId, SlotType)
    CROSS APPLY (SELECT t.Id, s.Id AS StaffId, t.SlotType FROM (VALUES (@TmplXH)) AS t CROSS JOIN (SELECT @StaffDoc1 AS Id) AS s) AS x
    -- Actually let me use a simpler approach
;

-- Simpler: direct insert for each template
SET @SlotDate = DATEADD(DAY, -14, CAST(SYSUTCDATETIME() AS DATE));
WHILE @SlotDate <= DATEADD(DAY, 14, CAST(SYSUTCDATETIME() AS DATE))
BEGIN
    SET @DayOfWeek = DATEPART(WEEKDAY, @SlotDate);
    IF @DayOfWeek BETWEEN 1 AND 5  -- Weekdays only
    BEGIN
        -- Morning slots (08:00-12:00)
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc1 AND SlotDate = @SlotDate AND StartTime = '08:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplXH, @StaffDoc1, @SlotDate, '08:00', '12:00', 30, 0, N'EXP');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc2 AND SlotDate = @SlotDate AND StartTime = '08:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplHX, @StaffDoc2, @SlotDate, '08:00', '12:00', 25, 0, N'EXP');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc3 AND SlotDate = @SlotDate AND StartTime = '08:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplPT, @StaffDoc3, @SlotDate, '08:00', '12:00', 35, 0, N'REG');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc4 AND SlotDate = @SlotDate AND StartTime = '08:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplGU, @StaffDoc4, @SlotDate, '08:00', '12:00', 20, 0, N'REG');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc5 AND SlotDate = @SlotDate AND StartTime = '08:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplFK, @StaffDoc5, @SlotDate, '08:00', '12:00', 20, 0, N'REG');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc6 AND SlotDate = @SlotDate AND StartTime = '08:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplEK, @StaffDoc6, @SlotDate, '08:00', '12:00', 25, 0, N'REG');

        -- Afternoon slots (14:00-17:00)
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc1 AND SlotDate = @SlotDate AND StartTime = '14:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplXH, @StaffDoc1, @SlotDate, '14:00', '17:00', 20, 0, N'EXP');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc2 AND SlotDate = @SlotDate AND StartTime = '14:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplHX, @StaffDoc2, @SlotDate, '14:00', '17:00', 20, 0, N'REG');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc3 AND SlotDate = @SlotDate AND StartTime = '14:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplPT, @StaffDoc3, @SlotDate, '14:00', '17:00', 25, 0, N'REG');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc4 AND SlotDate = @SlotDate AND StartTime = '14:00')
            INSERT INTO opd.ScheduleSlots (TemplateId, StaffId, SlotDate, StartTime, EndTime, TotalQuota, BookedQuota, SlotType)
            VALUES (@TmplGU, @StaffDoc4, @SlotDate, '14:00', '17:00', 15, 0, N'REG');
    END
    SET @SlotDate = DATEADD(DAY, 1, @SlotDate);
END

/* ---------- opd.Appointments (预约) ---------- */
DECLARE @SlotId1 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc1 AND SlotDate >= CAST(SYSUTCDATETIME() AS DATE) ORDER BY SlotDate, StartTime);
DECLARE @SlotId2 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc2 AND SlotDate >= CAST(SYSUTCDATETIME() AS DATE) ORDER BY SlotDate, StartTime);
DECLARE @SlotId3 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc3 AND SlotDate >= CAST(SYSUTCDATETIME() AS DATE) ORDER BY SlotDate, StartTime);
DECLARE @SlotId4 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc4 AND SlotDate >= CAST(SYSUTCDATETIME() AS DATE) ORDER BY SlotDate, StartTime);
DECLARE @SlotId5 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc5 AND SlotDate >= CAST(SYSUTCDATETIME() AS DATE) ORDER BY SlotDate, StartTime);
DECLARE @SlotId6 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc6 AND SlotDate >= CAST(SYSUTCDATETIME() AS DATE) ORDER BY SlotDate, StartTime);

-- Only insert if slots exist and not already booked
IF @SlotId1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Appointments WHERE SlotId = @SlotId1 AND PatientId = @Pat1)
    INSERT INTO opd.Appointments (CampusId, PatientId, SlotId, Channel, Status) VALUES (@Campus1, @Pat1, @SlotId1, N'院内', N'Booked');
IF @SlotId1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Appointments WHERE SlotId = @SlotId1 AND PatientId = @Pat2)
    INSERT INTO opd.Appointments (CampusId, PatientId, SlotId, Channel, Status) VALUES (@Campus1, @Pat2, @SlotId1, N'电话', N'Booked');
IF @SlotId2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Appointments WHERE SlotId = @SlotId2 AND PatientId = @Pat3)
    INSERT INTO opd.Appointments (CampusId, PatientId, SlotId, Channel, Status) VALUES (@Campus1, @Pat3, @SlotId2, N'互联网', N'Booked');
IF @SlotId3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Appointments WHERE SlotId = @SlotId3 AND PatientId = @Pat4)
    INSERT INTO opd.Appointments (CampusId, PatientId, SlotId, Channel, Status) VALUES (@Campus1, @Pat4, @SlotId3, N'院内', N'Booked');
IF @SlotId4 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Appointments WHERE SlotId = @SlotId4 AND PatientId = @Pat5)
    INSERT INTO opd.Appointments (CampusId, PatientId, SlotId, Channel, Status) VALUES (@Campus1, @Pat5, @SlotId4, N'院内', N'Booked');
IF @SlotId5 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Appointments WHERE SlotId = @SlotId5 AND PatientId = @Pat6)
    INSERT INTO opd.Appointments (CampusId, PatientId, SlotId, Channel, Status) VALUES (@Campus1, @Pat6, @SlotId5, N'电话', N'Booked');
IF @SlotId6 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Appointments WHERE SlotId = @SlotId6 AND PatientId = @Pat7)
    INSERT INTO opd.Appointments (CampusId, PatientId, SlotId, Channel, Status) VALUES (@Campus1, @Pat7, @SlotId6, N'院内', N'Booked');

/* ---------- opd.Registrations (挂号) ---------- */
-- Historical registrations from the past 7 days
DECLARE @RegDate DATE;
DECLARE @RegSeq INT = 1;

DECLARE reg_cursor CURSOR LOCAL FOR
    SELECT DATEADD(DAY, -n, CAST(SYSUTCDATETIME() AS DATE)) AS d
    FROM (SELECT 0 n UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6) AS nums;

OPEN reg_cursor;
FETCH NEXT FROM reg_cursor INTO @RegDate;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @DayOfWeek = DATEPART(WEEKDAY, @RegDate);
    IF @DayOfWeek BETWEEN 1 AND 5
    BEGIN
        -- Get a slot for each doctor on this date
        DECLARE @SlotMorning BIGINT, @SlotAfternoon BIGINT;
        SELECT TOP 1 @SlotMorning = Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc1 AND SlotDate = @RegDate AND StartTime = '08:00';
        SELECT TOP 1 @SlotAfternoon = Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc1 AND SlotDate = @RegDate AND StartTime = '14:00';

        IF @SlotMorning IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE RegistrationNo = N'REG' + FORMAT(@RegDate, N'yyyyMMdd') + RIGHT('000' + CAST(@RegSeq AS NVARCHAR), 4))
                INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
                VALUES (@Campus1, @Pat1, @SlotMorning, N'REG' + FORMAT(@RegDate, N'yyyyMMdd') + RIGHT('000' + CAST(@RegSeq AS NVARCHAR), 4), N'Finished', 10.00, DATEADD(HOUR, 8, CAST(@RegDate AS DATETIME2)), DATEADD(HOUR, 8, CAST(@RegDate AS DATETIME2)));
            SET @RegSeq = @RegSeq + 1;
        END

        IF @SlotAfternoon IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE RegistrationNo = N'REG' + FORMAT(@RegDate, N'yyyyMMdd') + RIGHT('000' + CAST(@RegSeq AS NVARCHAR), 4))
                INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
                VALUES (@Campus1, @Pat2, @SlotAfternoon, N'REG' + FORMAT(@RegDate, N'yyyyMMdd') + RIGHT('000' + CAST(@RegSeq AS NVARCHAR), 4), N'Finished', 10.00, DATEADD(HOUR, 14, CAST(@RegDate AS DATETIME2)), DATEADD(HOUR, 14, CAST(@RegDate AS DATETIME2)));
            SET @RegSeq = @RegSeq + 1;
        END

        -- More patients for variety
        SELECT TOP 1 @SlotMorning = Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc2 AND SlotDate = @RegDate AND StartTime = '08:00';
        IF @SlotMorning IS NOT NULL AND @RegSeq <= 50
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE RegistrationNo = N'REG' + FORMAT(@RegDate, N'yyyyMMdd') + RIGHT('000' + CAST(@RegSeq AS NVARCHAR), 4))
                INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
                VALUES (@Campus1, @Pat3, @SlotMorning, N'REG' + FORMAT(@RegDate, N'yyyyMMdd') + RIGHT('000' + CAST(@RegSeq AS NVARCHAR), 4), N'Finished', 30.00, DATEADD(HOUR, 8, CAST(@RegDate AS DATETIME2)), DATEADD(HOUR, 8, CAST(@RegDate AS DATETIME2)));
            SET @RegSeq = @RegSeq + 1;
        END
    END
    FETCH NEXT FROM reg_cursor INTO @RegDate;
END
CLOSE reg_cursor;
DEALLOCATE reg_cursor;

-- Today's registrations (status = Registered for active encounters)
DECLARE @Today DATE = CAST(SYSUTCDATETIME() AS DATE);
DECLARE @TodaySlot1 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc1 AND SlotDate = @Today AND StartTime = '08:00');
DECLARE @TodaySlot2 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc2 AND SlotDate = @Today AND StartTime = '08:00');
DECLARE @TodaySlot3 BIGINT = (SELECT TOP 1 Id FROM opd.ScheduleSlots WHERE StaffId = @StaffDoc3 AND SlotDate = @Today AND StartTime = '08:00');

IF @TodaySlot1 IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @TodaySlot1 AND PatientId = @Pat4)
        INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
        VALUES (@Campus1, @Pat4, @TodaySlot1, N'REG' + FORMAT(@Today, N'yyyyMMdd') + 'T01', N'Registered', 10.00, SYSUTCDATETIME(), SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @TodaySlot1 AND PatientId = @Pat5)
        INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
        VALUES (@Campus1, @Pat5, @TodaySlot1, N'REG' + FORMAT(@Today, N'yyyyMMdd') + 'T02', N'Registered', 10.00, SYSUTCDATETIME(), SYSUTCDATETIME());
END
IF @TodaySlot2 IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @TodaySlot2 AND PatientId = @Pat6)
        INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
        VALUES (@Campus1, @Pat6, @TodaySlot2, N'REG' + FORMAT(@Today, N'yyyyMMdd') + 'T03', N'Registered', 30.00, SYSUTCDATETIME(), SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @TodaySlot2 AND PatientId = @Pat7)
        INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
        VALUES (@Campus1, @Pat7, @TodaySlot2, N'REG' + FORMAT(@Today, N'yyyyMMdd') + 'T04', N'Registered', 30.00, SYSUTCDATETIME(), SYSUTCDATETIME());
END
IF @TodaySlot3 IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @TodaySlot3 AND PatientId = @Pat8)
        INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
        VALUES (@Campus1, @Pat8, @TodaySlot3, N'REG' + FORMAT(@Today, N'yyyyMMdd') + 'T05', N'Registered', 10.00, SYSUTCDATETIME(), SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @TodaySlot3 AND PatientId = @Pat9)
        INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
        VALUES (@Campus1, @Pat9, @TodaySlot3, N'REG' + FORMAT(@Today, N'yyyyMMdd') + 'T06', N'Registered', 10.00, SYSUTCDATETIME(), SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @TodaySlot3 AND PatientId = @Pat10)
        INSERT INTO opd.Registrations (CampusId, PatientId, SlotId, RegistrationNo, Status, FeeAmount, PaidAt, CreatedAt)
        VALUES (@Campus1, @Pat10, @TodaySlot3, N'REG' + FORMAT(@Today, N'yyyyMMdd') + 'T07', N'Registered', 10.00, SYSUTCDATETIME(), SYSUTCDATETIME());
END

/* ---------- opd.TriageQueueEntries ---------- */
INSERT INTO opd.TriageQueueEntries (CampusId, RegistrationId, Priority, QueueNo, RoomId)
SELECT @Campus1, r.Id, 0,
    N'A' + CAST(ROW_NUMBER() OVER (ORDER BY r.Id) AS NVARCHAR),
    N'诊室101'
FROM opd.Registrations r
WHERE r.CampusId = @Campus1
  AND r.SlotId IN (SELECT Id FROM opd.ScheduleSlots WHERE StaffId IN (@StaffDoc1, @StaffDoc2, @StaffDoc3) AND SlotDate = @Today)
  AND r.Status = N'Registered'
  AND NOT EXISTS (SELECT 1 FROM opd.TriageQueueEntries t WHERE t.RegistrationId = r.Id);
GO

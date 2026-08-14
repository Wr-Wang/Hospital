/*
  02_seed_data.sql
  初始化种子数据（合并自 init_full.sql 第四部分种子数据 + 999_verify_seed_data.sql）。
  要求：先执行 01_create_schema.sql 建库建表，再执行本脚本写入数据。
  每小时排班时段（7 个时段，每时段配额 5），与线上 Hospital 库当前数据一致。
  脚本全部使用 IF NOT EXISTS 幂等写法，可重复执行。
*/
SET NOCOUNT ON;
GO
USE [Hospital];
GO
/* ===================================================================
   第四部分：种子数据
   =================================================================== */
PRINT N'开始写入种子数据...';

-- ==================== 院区 ====================
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'ZONGYUAN')
    INSERT INTO mdm.Campuses (Code, Name, Address, Phone, IsActive)
    VALUES (N'ZONGYUAN', N'总院区', N'北京市海淀区中关村大街1号', N'010-12345678', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'DONGYUAN')
    INSERT INTO mdm.Campuses (Code, Name, Address, IsActive)
    VALUES (N'DONGYUAN', N'东院', N'北京市朝阳区', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'XIYUAN')
    INSERT INTO mdm.Campuses (Code, Name, Address, IsActive)
    VALUES (N'XIYUAN', N'西院', N'北京市西城区', 1);

DECLARE @Campus1 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'ZONGYUAN');
DECLARE @Campus2 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'DONGYUAN');
DECLARE @Campus3 BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'XIYUAN');

-- ==================== 科室 ====================
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus1)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@Campus1, NULL, N'ROOT', N'根科室', N'Admin', 1);

DECLARE @RootDept BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus1);

IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'NK' AND CampusId = @Campus1)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@Campus1, @RootDept, N'NK', N'内科', N'Clinical', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'WK' AND CampusId = @Campus1)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@Campus1, @RootDept, N'WK', N'外科', N'Clinical', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'SFK' AND CampusId = @Campus1)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@Campus1, @RootDept, N'SFK', N'收费处', N'Admin', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'YPK' AND CampusId = @Campus1)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@Campus1, @RootDept, N'YPK', N'药品科', N'Pharmacy', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'GHS' AND CampusId = @Campus1)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@Campus1, @RootDept, N'GHS', N'挂号室', N'Admin', 1);

-- 东院/西院根科室
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus2)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@Campus2, NULL, N'ROOT', N'东院根科室', N'Admin', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @Campus3)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@Campus3, NULL, N'ROOT', N'西院根科室', N'Admin', 1);

DECLARE @DeptNK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'NK' AND CampusId = @Campus1);
DECLARE @DeptWK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'WK' AND CampusId = @Campus1);
DECLARE @DeptSFK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'SFK' AND CampusId = @Campus1);
DECLARE @DeptYPK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'YPK' AND CampusId = @Campus1);
DECLARE @DeptGHS BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'GHS' AND CampusId = @Campus1);

-- ==================== 人员 ====================
IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0001')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0001', N'系统管理员', N'Male', N'13800138001', @Campus1, @DeptNK, N'执业医师', N'000000000000000', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0002')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0002', N'张医生', N'Male', N'13800138002', @Campus1, @DeptNK, N'执业医师', N'110101199001011234', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0003')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0003', N'李挂号', N'Female', N'13800138003', @Campus1, @DeptGHS, N'执业护士', N'110101198505152345', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0004')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0004', N'赵收费', N'Female', N'13800138004', @Campus1, @DeptSFK, N'医技', N'110101197803031456', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0005')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0005', N'王药房', N'Male', N'13800138005', @Campus1, @DeptYPK, N'药师', N'110101198812122367', 1);

-- 额外人员（来自 901）
MERGE INTO mdm.Staff AS t
USING (VALUES
    (@Campus1, @DeptNK, N'E0006', N'刘洋',   N'Female', N'13800138006', N'执业医师', N'110101199205056789', 1),
    (@Campus1, @DeptNK, N'E0007', N'陈芳',   N'Female', N'13800138007', N'执业护士', N'110101199103152890', 1),
    (@Campus1, @DeptWK, N'E0008', N'王强',   N'Male',   N'13800138008', N'执业医师', N'110101197803031456', 1),
    (@Campus1, @DeptWK, N'E0009', N'黄勇',   N'Male',   N'13800138009', N'执业医师', N'110101198612093456', 1),
    (@Campus1, @DeptNK, N'E0010', N'周磊',   N'Male',   N'13800138010', N'执业医师', N'110101198209214567', 1),
    (@Campus1, @DeptNK, N'E0011', N'吴秀英', N'Female', N'13800138011', N'执业护士', N'110101199508181234', 1)
) AS s(CampusId, DepartmentId, EmployeeNo, FullName, Gender, Phone, StaffCategory, LicenseNo, IsActive)
ON t.EmployeeNo = s.EmployeeNo AND t.CampusId = s.CampusId
WHEN NOT MATCHED THEN INSERT (CampusId, DepartmentId, EmployeeNo, FullName, Gender, Phone, StaffCategory, LicenseNo, IsActive)
    VALUES (s.CampusId, s.DepartmentId, s.EmployeeNo, s.FullName, s.Gender, s.Phone, s.StaffCategory, s.LicenseNo, s.IsActive);

DECLARE @StaffAdmin BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0001');
DECLARE @StaffDoctor BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0002');
DECLARE @StaffDoc2 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0006');
DECLARE @StaffDoc3 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0008');
DECLARE @StaffDoc4 BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0009');

-- ==================== 角色 ====================
IF NOT EXISTS (SELECT 1 FROM sec.Roles WHERE Name = N'ADMIN')
    INSERT INTO sec.Roles (Name, Description) VALUES (N'ADMIN', N'系统管理员');
IF NOT EXISTS (SELECT 1 FROM sec.Roles WHERE Name = N'DOCTOR')
    INSERT INTO sec.Roles (Name, Description) VALUES (N'DOCTOR', N'门诊医生');
IF NOT EXISTS (SELECT 1 FROM sec.Roles WHERE Name = N'REGISTRATION')
    INSERT INTO sec.Roles (Name, Description) VALUES (N'REGISTRATION', N'挂号员');
IF NOT EXISTS (SELECT 1 FROM sec.Roles WHERE Name = N'CASHIER')
    INSERT INTO sec.Roles (Name, Description) VALUES (N'CASHIER', N'收费员');
IF NOT EXISTS (SELECT 1 FROM sec.Roles WHERE Name = N'PHARMACY')
    INSERT INTO sec.Roles (Name, Description) VALUES (N'PHARMACY', N'药剂师');

-- ==================== 系统用户 ====================
IF NOT EXISTS (SELECT 1 FROM sec.Users WHERE LoginName = N'admin')
    INSERT INTO sec.Users (LoginName, PasswordHash, DisplayName, CampusName, IsLocked)
    VALUES (N'admin', N'admin123', N'系统管理员', N'总院区', 0);
IF NOT EXISTS (SELECT 1 FROM sec.Users WHERE LoginName = N'doctor')
    INSERT INTO sec.Users (LoginName, PasswordHash, DisplayName, CampusName, IsLocked)
    VALUES (N'doctor', N'doctor123', N'张医生', N'总院区', 0);
IF NOT EXISTS (SELECT 1 FROM sec.Users WHERE LoginName = N'reg')
    INSERT INTO sec.Users (LoginName, PasswordHash, DisplayName, CampusName, IsLocked)
    VALUES (N'reg', N'reg123', N'李挂号', N'总院区', 0);
IF NOT EXISTS (SELECT 1 FROM sec.Users WHERE LoginName = N'pharm')
    INSERT INTO sec.Users (LoginName, PasswordHash, DisplayName, CampusName, IsLocked)
    VALUES (N'pharm', N'pharm123', N'王药房', N'总院区', 0);
IF NOT EXISTS (SELECT 1 FROM sec.Users WHERE LoginName = N'cash')
    INSERT INTO sec.Users (LoginName, PasswordHash, DisplayName, CampusName, IsLocked)
    VALUES (N'cash', N'cash123', N'赵收费', N'总院区', 0);

-- ==================== 字典类型 ====================
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'GENDER')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'GENDER', N'性别', N'', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'STAFF_CAT')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'STAFF_CAT', N'人员类别', N'', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'DEPT_TYPE')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'DEPT_TYPE', N'科室类型', N'', 1);

DECLARE @TypeGender BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'GENDER');
DECLARE @TypeStaffCat BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'STAFF_CAT');
DECLARE @TypeDeptType BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'DEPT_TYPE');

-- ==================== 字典项 ====================
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeGender AND Code = N'Male')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeGender, N'Male', N'男', 1, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeGender AND Code = N'Female')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeGender, N'Female', N'女', 2, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeGender AND Code = N'Other')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeGender, N'Other', N'未知', 3, 1);

IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeStaffCat AND Code = N'执业医师')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeStaffCat, N'执业医师', N'医生', 1, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeStaffCat AND Code = N'执业护士')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeStaffCat, N'执业护士', N'护士', 2, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeStaffCat AND Code = N'药师')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeStaffCat, N'药师', N'药剂师', 3, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeStaffCat AND Code = N'医技')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeStaffCat, N'医技', N'医技人员', 4, 1);

IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDeptType AND Code = N'Admin')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDeptType, N'Admin', N'行政科室', 1, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDeptType AND Code = N'Clinical')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDeptType, N'Clinical', N'临床科室', 2, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDeptType AND Code = N'Lab')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDeptType, N'Lab', N'检验科室', 3, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDeptType AND Code = N'Radiology')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDeptType, N'Radiology', N'放射科室', 4, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDeptType AND Code = N'Pharmacy')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDeptType, N'Pharmacy', N'药剂科室', 5, 1);

-- ==================== 门诊医生站参考字典 ====================
-- 供医生站在诊断/处方/检验 Tab 提供快捷选择、自动填充与占位示例
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'ICD_10')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'ICD_10', N'ICD-10 诊断', N'门诊诊断常用 ICD-10 编码', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'DRUG')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'DRUG', N'常用药品', N'门诊常用药品（名称 | 规格 | 剂型）', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'LAB_ITEM')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'LAB_ITEM', N'检验检查项目', N'常用检验检查项目', 1);

DECLARE @TypeIcd BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'ICD_10');
DECLARE @TypeDrug BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'DRUG');
DECLARE @TypeLab BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'LAB_ITEM');

-- ICD-10 诊断项
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'I10')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'I10', N'高血压病', 1, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'I10.x05')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'I10.x05', N'高血压病2级', 2, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'I11')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'I11', N'高血压性心脏病', 3, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'I63.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'I63.9', N'脑梗死', 4, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'I25.1')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'I25.1', N'冠状动脉粥样硬化性心脏病', 5, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'I20.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'I20.9', N'心绞痛', 6, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'I48')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'I48', N'心房颤动', 7, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'E11.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'E11.9', N'2型糖尿病', 8, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'E14.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'E14.9', N'糖尿病', 9, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'J06.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'J06.9', N'急性上呼吸道感染', 10, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'J20.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'J20.9', N'急性支气管炎', 11, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'J18.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'J18.9', N'肺炎', 12, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'J45.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'J45.9', N'支气管哮喘', 13, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'J02.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'J02.9', N'急性咽炎', 14, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'J03.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'J03.9', N'急性扁桃体炎', 15, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'J32.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'J32.9', N'慢性鼻窦炎', 16, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'J40')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'J40', N'慢性支气管炎', 17, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'K35.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'K35.9', N'急性阑尾炎', 18, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'K29.5')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'K29.5', N'慢性胃炎', 19, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'K21.0')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'K21.0', N'胃食管反流病', 20, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'K52.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'K52.9', N'急性胃肠炎', 21, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'N39.0')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'N39.0', N'泌尿道感染', 22, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'N20.0')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'N20.0', N'肾结石', 23, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'M54.5')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'M54.5', N'腰痛', 24, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'M17.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'M17.9', N'膝骨关节炎', 25, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'M10.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'M10.9', N'痛风', 26, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'B18.1')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'B18.1', N'乙型病毒性肝炎', 27, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'A09')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'A09', N'感染性腹泻', 28, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'F32.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'F32.9', N'抑郁症', 29, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'G43.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'G43.9', N'偏头痛', 30, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'G40.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'G40.9', N'癫痫', 31, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'H10.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'H10.9', N'结膜炎', 32, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'R05')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'R05', N'咳嗽', 33, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'R50.9')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'R50.9', N'发热', 34, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'R42')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'R42', N'头晕', 35, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeIcd AND Code = N'Z00.0')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeIcd, N'Z00.0', N'健康体检', 36, 1);

-- 常用药品项（Name 约定：药品名 | 规格 | 剂型 | 频次 | 每次剂量 | 天数 | 总量）
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'AMOXIL')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'AMOXIL', N'阿莫西林胶囊 | 0.25g*24粒 | 胶囊剂 | BID | 1粒 | 7 | 14', 1, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'CEFIXIME')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'CEFIXIME', N'头孢克肟片 | 50mg*12片 | 片剂 | BID | 1片 | 5 | 10', 2, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'IBUPROFEN')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'IBUPROFEN', N'布洛芬缓释胶囊 | 0.3g*20粒 | 缓释胶囊 | TID | 1粒 | 3 | 9', 3, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'NIFEDIPINE')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'NIFEDIPINE', N'硝苯地平片 | 10mg*100片 | 片剂 | QD | 10mg | 30 | 30', 4, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'PARACETAMOL')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'PARACETAMOL', N'对乙酰氨基酚片 | 0.5g*20片 | 片剂 | PRN | 1片 | 3 | 6', 5, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'METFORMIN')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'METFORMIN', N'二甲双胍片 | 0.5g*20片 | 片剂 | BID | 1片 | 30 | 60', 6, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'OMEPRAZOLE')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'OMEPRAZOLE', N'奥美拉唑肠溶胶囊 | 20mg*14粒 | 肠溶胶囊 | QD | 1粒 | 14 | 14', 7, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'LOSARTAN')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'LOSARTAN', N'氯沙坦钾片 | 50mg*7片 | 片剂 | QD | 1片 | 30 | 30', 8, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'ATORVASTATIN')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'ATORVASTATIN', N'阿托伐他汀钙片 | 10mg*7片 | 片剂 | QN | 1片 | 30 | 30', 9, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'ASPIRIN')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'ASPIRIN', N'阿司匹林肠溶片 | 0.1g*30片 | 肠溶片 | QD | 1片 | 30 | 30', 10, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'CLARITHROMYCIN')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'CLARITHROMYCIN', N'克拉霉素片 | 0.25g*6片 | 片剂 | BID | 2片 | 7 | 28', 11, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'AMBROXOL')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'AMBROXOL', N'盐酸氨溴索片 | 30mg*20片 | 片剂 | TID | 1片 | 5 | 15', 12, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'LORATADINE')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'LORATADINE', N'氯雷他定片 | 10mg*12片 | 片剂 | QD | 1片 | 7 | 7', 13, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'VITAMIN_C')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'VITAMIN_C', N'维生素C片 | 0.1g*100片 | 片剂 | QD | 1片 | 30 | 30', 14, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeDrug AND Code = N'AMPICILLIN')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeDrug, N'AMPICILLIN', N'氨苄西林胶囊 | 0.25g*24粒 | 胶囊剂 | QID | 2粒 | 7 | 56', 15, 1);

-- 检验检查项目项
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'CBC')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'CBC', N'血常规', 1, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'UAN')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'UAN', N'尿常规', 2, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'FOB')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'FOB', N'便常规', 3, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'LIVER_FUNC')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'LIVER_FUNC', N'肝功能', 4, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'RENAL')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'RENAL', N'肾功能', 5, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'FBS')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'FBS', N'空腹血糖', 6, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'LIPID')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'LIPID', N'血脂四项', 7, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'CRP')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'CRP', N'C反应蛋白', 8, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'HBA1C')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'HBA1C', N'糖化血红蛋白', 9, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'T3')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'T3', N'甲状腺功能三项', 10, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'ECG')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'ECG', N'心电图', 11, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'CHEST_XRAY')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'CHEST_XRAY', N'胸部X线检查', 12, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'ABDOMEN_US')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'ABDOMEN_US', N'腹部彩超', 13, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'CARDIAC_US')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'CARDIAC_US', N'心脏彩超', 14, 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeId = @TypeLab AND Code = N'HEPATITIS_B')
    INSERT INTO mdm.DictionaryItems (TypeId, Code, Name, SortOrder, IsActive) VALUES (@TypeLab, N'HEPATITIS_B', N'乙肝两对半', 15, 1);

-- ==================== 患者（12 人） ====================
-- 基础患者（来自 900）
IF NOT EXISTS (SELECT 1 FROM pat.Patients WHERE PatientNo = N'P00000001')
    INSERT INTO pat.Patients (PatientNo, Name, Gender, BirthDate, Phone, IdCardNo)
    VALUES (N'P00000001', N'张三', N'Male', '1990-01-01', N'13900000001', N'110101199001011234');
IF NOT EXISTS (SELECT 1 FROM pat.Patients WHERE PatientNo = N'P00000002')
    INSERT INTO pat.Patients (PatientNo, Name, Gender, BirthDate, Phone, IdCardNo)
    VALUES (N'P00000002', N'李四', N'Female', '1985-05-15', N'13900000002', N'110101198505152345');

-- 扩展患者（来自 901）
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

-- ==================== 患者标识 ====================
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

-- ==================== 患者知情同意 ====================
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

-- ==================== 清除旧数据（时段改为按小时后，旧数据不兼容） ====================
DELETE FROM enc.EmrDocuments;
DELETE FROM enc.Diagnoses;
DELETE FROM enc.OutpatientEncounters;
DELETE FROM opd.Registrations;
DELETE FROM opd.ScheduleSlots;
DELETE FROM opd.ScheduleTemplates;

-- ==================== 排班模板与时段 ====================
DECLARE @CurDate DATE = CAST(SYSDATETIME() AS DATE);
DECLARE @LoopDate DATE = DATEADD(DAY, -14, @CurDate);

WHILE @LoopDate <= DATEADD(DAY, 14, @CurDate)
BEGIN
    IF DATEPART(WEEKDAY, @LoopDate) BETWEEN 2 AND 6  -- 周一到周五
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleTemplates WHERE DoctorId = @StaffDoctor AND ScheduleDate = @LoopDate)
            INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
            VALUES (@StaffDoctor, @DeptNK, @Campus1, @LoopDate, N'已发布');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleTemplates WHERE DoctorId = @StaffDoc2 AND ScheduleDate = @LoopDate)
            INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
            VALUES (@StaffDoc2, @DeptNK, @Campus1, @LoopDate, N'已发布');
        IF NOT EXISTS (SELECT 1 FROM opd.ScheduleTemplates WHERE DoctorId = @StaffDoc3 AND ScheduleDate = @LoopDate)
            INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
            VALUES (@StaffDoc3, @DeptWK, @Campus1, @LoopDate, N'已发布');
    END
    SET @LoopDate = DATEADD(DAY, 1, @LoopDate);
END

-- 为所有排班模板创建时段
DECLARE @TmplId BIGINT, @TmplDoctorId BIGINT;
DECLARE tmpl_cursor CURSOR LOCAL FOR
    SELECT Id, DoctorId FROM opd.ScheduleTemplates
    WHERE CampusId = @Campus1 AND DoctorId IN (@StaffDoctor, @StaffDoc2, @StaffDoc3)
      AND ScheduleDate BETWEEN DATEADD(DAY, -14, @CurDate) AND DATEADD(DAY, 14, @CurDate);

OPEN tmpl_cursor;
FETCH NEXT FROM tmpl_cursor INTO @TmplId, @TmplDoctorId;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'08:00-09:00')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'08:00-09:00', '08:00', '09:00', 5, 0);
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'09:00-10:00')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'09:00-10:00', '09:00', '10:00', 5, 0);
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'10:00-11:00')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'10:00-11:00', '10:00', '11:00', 5, 0);
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'11:00-12:00')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'11:00-12:00', '11:00', '12:00', 5, 0);
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'14:00-15:00')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'14:00-15:00', '14:00', '15:00', 5, 0);
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'15:00-16:00')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'15:00-16:00', '15:00', '16:00', 5, 0);
    IF NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'16:00-17:00')
        INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
        VALUES (@TmplId, N'16:00-17:00', '16:00', '17:00', 5, 0);
    FETCH NEXT FROM tmpl_cursor INTO @TmplId, @TmplDoctorId;
END
CLOSE tmpl_cursor;
DEALLOCATE tmpl_cursor;

-- ==================== 挂号（过去 7 天 + 今天） ====================
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
        SELECT TOP 1 @SlotAM = ss.Id FROM opd.ScheduleSlots ss
            INNER JOIN opd.ScheduleTemplates st ON ss.TemplateId = st.Id
            WHERE st.DoctorId = @StaffDoctor AND st.ScheduleDate = @RegDate AND ss.SlotType = N'08:00-09:00';
        SELECT TOP 1 @SlotPM = ss.Id FROM opd.ScheduleSlots ss
            INNER JOIN opd.ScheduleTemplates st ON ss.TemplateId = st.Id
            WHERE st.DoctorId = @StaffDoc2 AND st.ScheduleDate = @RegDate AND ss.SlotType = N'14:00-15:00';

        IF @RegDate < @CurDate  -- 过去的日期：已就诊
        BEGIN
            IF @SlotAM IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @SlotAM AND PatientId = @Pat1)
                INSERT INTO opd.Registrations (PatientId, SlotId, DoctorId, DeptId, CampusId, CreatedAt, QueueNo, SlotName, Status)
                VALUES (@Pat1, @SlotAM, @StaffDoctor, @DeptNK, @Campus1, DATEADD(HOUR, 8, CAST(@RegDate AS DATETIME2)), @Seq, N'08:00-09:00', N'已就诊');
            SET @Seq = @Seq + 1;
            IF @SlotPM IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @SlotPM AND PatientId = @Pat2)
                INSERT INTO opd.Registrations (PatientId, SlotId, DoctorId, DeptId, CampusId, CreatedAt, QueueNo, SlotName, Status)
                VALUES (@Pat2, @SlotPM, @StaffDoc2, @DeptNK, @Campus1, DATEADD(HOUR, 14, CAST(@RegDate AS DATETIME2)), @Seq, N'14:00-15:00', N'已就诊');
            SET @Seq = @Seq + 1;
        END
        ELSE  -- 今天：已挂号待就诊
        BEGIN
            IF @SlotAM IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @SlotAM AND PatientId = @Pat3)
                INSERT INTO opd.Registrations (PatientId, SlotId, DoctorId, DeptId, CampusId, CreatedAt, QueueNo, SlotName, Status)
                VALUES (@Pat3, @SlotAM, @StaffDoctor, @DeptNK, @Campus1, SYSDATETIME(), @Seq, N'08:00-09:00', N'已挂号');
            SET @Seq = @Seq + 1;
            IF @SlotPM IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.Registrations WHERE SlotId = @SlotPM AND PatientId = @Pat4)
                INSERT INTO opd.Registrations (PatientId, SlotId, DoctorId, DeptId, CampusId, CreatedAt, QueueNo, SlotName, Status)
                VALUES (@Pat4, @SlotPM, @StaffDoc2, @DeptNK, @Campus1, SYSDATETIME(), @Seq, N'14:00-15:00', N'已挂号');
            SET @Seq = @Seq + 1;
        END
    END
    FETCH NEXT FROM reg_cursor INTO @RegDate;
END
CLOSE reg_cursor;
DEALLOCATE reg_cursor;

-- ==================== 就诊 ====================
INSERT INTO enc.OutpatientEncounters (PatientId, StaffId, DepartmentId, CampusId, RegistrationId, Status, StartedAt, EndedAt)
SELECT r.PatientId, r.DoctorId, r.DeptId, r.CampusId, r.Id, N'已完成',
    DATEADD(MINUTE, 30, r.CreatedAt), DATEADD(MINUTE, 90, r.CreatedAt)
FROM opd.Registrations r
WHERE r.Status = N'已就诊'
  AND NOT EXISTS (SELECT 1 FROM enc.OutpatientEncounters e WHERE e.RegistrationId = r.Id);

INSERT INTO enc.OutpatientEncounters (PatientId, StaffId, DepartmentId, CampusId, RegistrationId, Status, StartedAt)
SELECT r.PatientId, r.DoctorId, r.DeptId, r.CampusId, r.Id, N'就诊中', SYSDATETIME()
FROM opd.Registrations r
WHERE r.Status = N'已挂号'
  AND NOT EXISTS (SELECT 1 FROM enc.OutpatientEncounters e WHERE e.RegistrationId = r.Id);

-- ==================== 诊断 ====================
INSERT INTO enc.Diagnoses (OutpatientEncounterId, DiagnosisType, IcdCode, IcdName, IsPrimary)
SELECT e.Id, N'主要诊断', N'I10.x05', N'高血压病2级', 1
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @StaffDoctor AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'I10.x05');

INSERT INTO enc.Diagnoses (OutpatientEncounterId, DiagnosisType, IcdCode, IcdName, IsPrimary)
SELECT e.Id, N'主要诊断', N'J15.901', N'细菌性肺炎', 1
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @StaffDoc2 AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IcdCode = N'J15.901');

INSERT INTO enc.Diagnoses (OutpatientEncounterId, DiagnosisType, IcdCode, IcdName, IsPrimary)
SELECT e.Id, N'次要诊断', N'I10.x05', N'高血压', 0
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @StaffDoctor AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.Diagnoses d WHERE d.OutpatientEncounterId = e.Id AND d.IsPrimary = 0 AND d.IcdCode = N'I10.x05');

-- ==================== 电子病历 ====================
INSERT INTO enc.EmrDocuments (OutpatientEncounterId, ContentJson, DocType, Version)
SELECT e.Id, N'{"主诉":"胸闷气促1周","诊断":"高血压","处理":"继续降压治疗"}', N'终稿', 1
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @StaffDoctor AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.EmrDocuments m WHERE m.OutpatientEncounterId = e.Id);

INSERT INTO enc.EmrDocuments (OutpatientEncounterId, ContentJson, DocType, Version)
SELECT e.Id, N'{"主诉":"咳嗽咳痰3天","诊断":"肺炎","处理":"抗感染治疗"}', N'终稿', 1
FROM enc.OutpatientEncounters e
WHERE e.StaffId = @StaffDoc2 AND e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM enc.EmrDocuments m WHERE m.OutpatientEncounterId = e.Id AND m.ContentJson LIKE N'%肺炎%');

-- ==================== 处方 ====================
INSERT INTO pha.Prescriptions (OutpatientEncounterId, PrescribedByStaffId, Status)
SELECT e.Id, e.StaffId, N'已发药'
FROM enc.OutpatientEncounters e
WHERE e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM pha.Prescriptions p WHERE p.OutpatientEncounterId = e.Id);

-- 处方明细
INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugName, Spec, Form, Frequency, Dose, Days, Qty, Note)
SELECT p.Id, N'硝苯地平片', N'10mg*100片', N'片剂', N'QD', N'10mg', 30, 30, N'口服'
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoctor)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugName = N'硝苯地平片');

INSERT INTO pha.PrescriptionLines (PrescriptionId, DrugName, Spec, Form, Frequency, Dose, Days, Qty, Note)
SELECT p.Id, N'阿莫西林胶囊', N'0.25g*24粒', N'胶囊', N'TID', N'0.5g', 7, 42, N'口服'
FROM pha.Prescriptions p
WHERE p.OutpatientEncounterId IN (SELECT Id FROM enc.OutpatientEncounters WHERE StaffId = @StaffDoc2)
  AND NOT EXISTS (SELECT 1 FROM pha.PrescriptionLines l WHERE l.PrescriptionId = p.Id AND l.DrugName = N'阿莫西林胶囊');

-- ==================== 检验检查 ====================
-- 注意：Status 需与领域枚举 OrderStatus 的值一致（已开立），否则 EF 反序列化抛异常
INSERT INTO lab.LabOrders (OutpatientEncounterId, ItemCode, ItemName, Status)
SELECT e.Id, N'CBC', N'血常规', N'已开立'
FROM enc.OutpatientEncounters e
WHERE e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM lab.LabOrders o WHERE o.OutpatientEncounterId = e.Id AND o.ItemCode = N'CBC');

INSERT INTO lab.LabOrders (OutpatientEncounterId, ItemCode, ItemName, Status)
SELECT e.Id, N'LIVER_FUNC', N'肝功能', N'已开立'
FROM enc.OutpatientEncounters e
WHERE e.Status = N'已完成' AND e.StaffId = @StaffDoctor
  AND NOT EXISTS (SELECT 1 FROM lab.LabOrders o WHERE o.OutpatientEncounterId = e.Id AND o.ItemCode = N'LIVER_FUNC');

INSERT INTO rad.ImagingOrders (OutpatientEncounterId, ItemCode, ItemName, Status)
SELECT e.Id, N'CHEST_XRAY', N'胸部X线检查', N'已开立'
FROM enc.OutpatientEncounters e
WHERE e.Status = N'已完成'
  AND NOT EXISTS (SELECT 1 FROM rad.ImagingOrders o WHERE o.OutpatientEncounterId = e.Id AND o.ItemCode = N'CHEST_XRAY');

-- ==================== 发票及收费明细、支付 ====================
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

-- ==================== 药品批次 ====================
IF NOT EXISTS (SELECT 1 FROM pha.DrugBatches WHERE DrugCode = N'DRG001')
    INSERT INTO pha.DrugBatches (DrugCode, DrugName, Spec, BatchNo, ExpiryDate, TotalQuantity, AvailableQuantity)
    VALUES (N'DRG001', N'阿莫西林胶囊', N'0.25g*24粒', N'20250601', '2027-06-01', 1000, 1000);
IF NOT EXISTS (SELECT 1 FROM pha.DrugBatches WHERE DrugCode = N'DRG002')
    INSERT INTO pha.DrugBatches (DrugCode, DrugName, Spec, BatchNo, ExpiryDate, TotalQuantity, AvailableQuantity)
    VALUES (N'DRG002', N'头孢克肟片', N'50mg*12片', N'20250601', '2027-06-01', 500, 500);
IF NOT EXISTS (SELECT 1 FROM pha.DrugBatches WHERE DrugCode = N'DRG003')
    INSERT INTO pha.DrugBatches (DrugCode, DrugName, Spec, BatchNo, ExpiryDate, TotalQuantity, AvailableQuantity)
    VALUES (N'DRG003', N'布洛芬缓释胶囊', N'0.3g*20粒', N'20250601', '2027-06-01', 800, 800);

-- 补充药品批次（来自 901）
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

-- ==================== 发药 ====================
INSERT INTO pha.Dispenses (PrescriptionId, DispensedBy, Status, CreatedAt)
SELECT p.Id, @StaffDoctor, N'已发药', SYSDATETIME()
FROM pha.Prescriptions p
WHERE p.Status = N'已发药'
  AND NOT EXISTS (SELECT 1 FROM pha.Dispenses d WHERE d.PrescriptionId = p.Id);

DECLARE @DispenseId BIGINT = (SELECT TOP 1 Id FROM pha.Dispenses ORDER BY Id);
DECLARE @Batch1 BIGINT = (SELECT TOP 1 Id FROM pha.DrugBatches ORDER BY Id);

IF @DispenseId IS NOT NULL AND @Batch1 IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM pha.DispenseLines WHERE DispensingId = @DispenseId)
    INSERT INTO pha.DispenseLines (DispensingId, InventoryLotId, DrugName, Spec, Qty)
    VALUES (@DispenseId, @Batch1, N'硝苯地平片', N'10mg*100片', 30);

-- ==================== 审计日志 ====================
IF NOT EXISTS (SELECT 1 FROM sec.AuditLogs WHERE Action = N'Login' AND UserName = N'admin')
    INSERT INTO sec.AuditLogs (UserId, UserName, Action, EntityType, EntityId, IpAddress, OccurredAt)
    VALUES (1, N'admin', N'Login', N'Session', 1, N'192.168.1.100', SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM sec.AuditLogs WHERE Action = N'Login' AND UserName = N'doctor')
    INSERT INTO sec.AuditLogs (UserId, UserName, Action, EntityType, EntityId, IpAddress, OccurredAt)
    VALUES (2, N'doctor', N'Login', N'Session', 2, N'192.168.1.101', DATEADD(HOUR, -1, SYSDATETIME()));

PRINT N'种子数据写入完成。';
GO

/* ===================================================================
   第五部分：数据验证（合并自 999_verify_seed_data.sql）
   =================================================================== */
SET NOCOUNT ON;

DECLARE @MinRows INT = 10;

-- 豁免表（业务特性决定数据量较少为正常）
DECLARE @ExemptTables TABLE (SchemaName NVARCHAR(128), TableName NVARCHAR(128));
INSERT INTO @ExemptTables VALUES
    (N'sec', N'AuditLogs'),          -- 审计日志：随系统使用增加
    (N'pha', N'Dispenses'),          -- 发药：依赖处方
    (N'pha', N'DispenseLines'),      -- 发药明细：依赖发药
    (N'fin', N'Invoices'),           -- 发票：依赖挂号收费
    (N'fin', N'ChargeLines'),        -- 收费明细：依赖发票
    (N'fin', N'Payments'),           -- 支付记录：依赖发票
    (N'enc', N'OutpatientEncounters'), -- 就诊记录：依赖挂号
    (N'enc', N'Diagnoses'),          -- 诊断：依赖就诊
    (N'enc', N'EmrDocuments'),       -- 病历：依赖就诊
    (N'pha', N'Prescriptions'),      -- 处方：依赖就诊
    (N'pha', N'PrescriptionLines'),  -- 处方明细：依赖处方
    (N'lab', N'LabOrders'),          -- 检验：依赖就诊
    (N'rad', N'ImagingOrders'),      -- 检查：依赖就诊
    (N'opd', N'Registrations'),      -- 挂号：依赖排班
    (N'opd', N'ScheduleTemplates'),  -- 排班模板：数量取决于排班周期
    (N'opd', N'ScheduleSlots'),      -- 排班时段：数量取决于排班模板
    (N'pat', N'PatientIdentifiers'), -- 患者标识：依赖患者
    (N'pat', N'PatientConsents'),    -- 知情同意：依赖患者
    (N'sec', N'WeChatAccounts'),     -- 微信账号：随微信登录增加
    (N'sec', N'PatientRefreshTokens'); -- 微信 refresh_token：随登录增加

SELECT
    SCHEMA_NAME(t.schema_id) AS SchemaName,
    t.name AS TableName,
    p.rows AS [RowCount],
    CASE WHEN p.rows >= @MinRows THEN N'✅' ELSE N'❌' END AS Status,
    CASE
        WHEN p.rows >= @MinRows THEN N'通过'
        WHEN et.SchemaName IS NOT NULL THEN N'豁免（少量为正常）'
        ELSE N'未达标（< ' + CAST(@MinRows AS NVARCHAR) + N' 行）'
    END AS Remarks
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id AND i.index_id <= 1
CROSS APPLY (
    SELECT SUM(p.rows) AS rows
    FROM sys.partitions p
    WHERE p.object_id = t.object_id AND p.index_id = i.index_id
) p
LEFT JOIN @ExemptTables et ON SCHEMA_NAME(t.schema_id) = et.SchemaName AND t.name = et.TableName
WHERE t.is_ms_shipped = 0
ORDER BY SCHEMA_NAME(t.schema_id), t.name;

DECLARE @TotalTables INT, @Passed INT, @Failed INT, @Exempt INT;

SELECT
    @TotalTables = COUNT(*),
    @Passed = SUM(CASE WHEN p.rows >= @MinRows THEN 1 ELSE 0 END),
    @Failed = SUM(CASE WHEN p.rows < @MinRows AND et.SchemaName IS NULL THEN 1 ELSE 0 END),
    @Exempt = SUM(CASE WHEN et.SchemaName IS NOT NULL THEN 1 ELSE 0 END)
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id AND i.index_id <= 1
CROSS APPLY (
    SELECT SUM(p.rows) AS rows
    FROM sys.partitions p
    WHERE p.object_id = t.object_id AND p.index_id = i.index_id
) p
LEFT JOIN @ExemptTables et ON SCHEMA_NAME(t.schema_id) = et.SchemaName AND t.name = et.TableName
WHERE t.is_ms_shipped = 0;

PRINT N'';
PRINT N'========================================';
PRINT N'  种子数据验证汇总';
PRINT N'========================================';
PRINT N'  总表数:          ' + CAST(@TotalTables AS NVARCHAR);
PRINT N'  达标 (>= 10行):  ' + CAST(@Passed AS NVARCHAR);
PRINT N'  未达标:          ' + CAST(@Failed AS NVARCHAR);
PRINT N'  豁免:            ' + CAST(@Exempt AS NVARCHAR);
PRINT N'========================================';

IF @Failed > 0
    PRINT N'  ⚠ 存在未达标表，请检查上方 ❌ 标记的表。';
ELSE
    PRINT N'  ✅ 所有表均达到 10 行以上数据！';
GO

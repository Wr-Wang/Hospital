/*
  900_seed_minimal.sql
  EF Core 模型的最小演示数据（院区、科室、人员、用户、角色、字典）。
  对应 000_init_database.sql 中的 27 张表。
  可重复执行：使用 IF NOT EXISTS 判断。
  登录凭证与 LocalUserStore 一致：
    admin/admin123（系统管理员）、doctor/doctor123（门诊医生）
    reg/reg123（挂号员）、pharm/pharm123（药剂师）、cash/cash123（收费员）
*/
USE [Hospital];
GO

SET NOCOUNT ON;

/* ==================== 院区 ==================== */
IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'ZONGYUAN')
    INSERT INTO mdm.Campuses (Code, Name, Address, Phone, IsActive)
    VALUES (N'ZONGYUAN', N'总院区', N'北京市海淀区中关村大街1号', N'010-12345678', 1);

DECLARE @CampusId BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'ZONGYUAN');

/* ==================== 科室 ==================== */
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @CampusId)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@CampusId, NULL, N'ROOT', N'根科室', N'Admin', 1);

DECLARE @RootDept BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'ROOT' AND CampusId = @CampusId);

IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'NK' AND CampusId = @CampusId)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@CampusId, @RootDept, N'NK', N'内科', N'Clinical', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'WK' AND CampusId = @CampusId)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@CampusId, @RootDept, N'WK', N'外科', N'Clinical', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'SFK' AND CampusId = @CampusId)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@CampusId, @RootDept, N'SFK', N'收费处', N'Admin', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'YPK' AND CampusId = @CampusId)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@CampusId, @RootDept, N'YPK', N'药品科', N'Pharmacy', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE Code = N'GHS' AND CampusId = @CampusId)
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsActive)
    VALUES (@CampusId, @RootDept, N'GHS', N'挂号室', N'Admin', 1);

DECLARE @DeptNK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'NK' AND CampusId = @CampusId);
DECLARE @DeptWK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'WK' AND CampusId = @CampusId);
DECLARE @DeptSFK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'SFK' AND CampusId = @CampusId);
DECLARE @DeptYPK BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'YPK' AND CampusId = @CampusId);
DECLARE @DeptGHS BIGINT = (SELECT Id FROM mdm.Departments WHERE Code = N'GHS' AND CampusId = @CampusId);

/* ==================== 人员 ==================== */
-- LicenseNo 必须 15-20 位数字（LicenseNumber 值对象验证）
IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0001')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0001', N'系统管理员', N'Male', N'13800138001', @CampusId, @DeptNK, N'执业医师', N'000000000000000', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0002')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0002', N'张医生', N'Male', N'13800138002', @CampusId, @DeptNK, N'执业医师', N'110101199001011234', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0003')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0003', N'李挂号', N'Female', N'13800138003', @CampusId, @DeptGHS, N'执业护士', N'110101198505152345', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0004')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0004', N'赵收费', N'Female', N'13800138004', @CampusId, @DeptSFK, N'医技', N'110101197803031456', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE EmployeeNo = N'E0005')
    INSERT INTO mdm.Staff (EmployeeNo, FullName, Gender, Phone, CampusId, DepartmentId, StaffCategory, LicenseNo, IsActive)
    VALUES (N'E0005', N'王药房', N'Male', N'13800138005', @CampusId, @DeptYPK, N'药师', N'110101198812122367', 1);

DECLARE @StaffAdmin BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0001');
DECLARE @StaffDoctor BIGINT = (SELECT Id FROM mdm.Staff WHERE EmployeeNo = N'E0002');

/* ==================== 角色 ==================== */
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

/* ==================== 系统用户 ==================== */
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

/* ==================== 字典类型 ==================== */
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'GENDER')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'GENDER', N'性别', N'', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'STAFF_CAT')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'STAFF_CAT', N'人员类别', N'', 1);
IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'DEPT_TYPE')
    INSERT INTO mdm.DictionaryTypes (Code, Name, Description, IsActive) VALUES (N'DEPT_TYPE', N'科室类型', N'', 1);

DECLARE @TypeGender BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'GENDER');
DECLARE @TypeStaffCat BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'STAFF_CAT');
DECLARE @TypeDeptType BIGINT = (SELECT Id FROM mdm.DictionaryTypes WHERE Code = N'DEPT_TYPE');

/* ==================== 字典项 ==================== */
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

/* ==================== 排班模板 ==================== */
IF NOT EXISTS (SELECT 1 FROM opd.ScheduleTemplates WHERE DoctorId = @StaffDoctor AND CampusId = @CampusId AND ScheduleDate = CAST(SYSDATETIME() AS DATE))
    INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
    VALUES (@StaffDoctor, @DeptNK, @CampusId, CAST(SYSDATETIME() AS DATE), N'已发布');

DECLARE @TmplId BIGINT = (SELECT Id FROM opd.ScheduleTemplates WHERE DoctorId = @StaffDoctor AND CampusId = @CampusId AND ScheduleDate = CAST(SYSDATETIME() AS DATE));

/* ==================== 排班时段 ==================== */
IF @TmplId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'上午')
    INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
    VALUES (@TmplId, N'上午', '08:00', '12:00', 30, 0);
IF @TmplId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM opd.ScheduleSlots WHERE TemplateId = @TmplId AND SlotType = N'下午')
    INSERT INTO opd.ScheduleSlots (TemplateId, SlotType, StartTime, EndTime, TotalQuota, BookedQuota)
    VALUES (@TmplId, N'下午', '14:00', '17:00', 20, 0);

/* ==================== 患者 ==================== */
IF NOT EXISTS (SELECT 1 FROM pat.Patients WHERE PatientNo = N'P00000001')
    INSERT INTO pat.Patients (PatientNo, Name, Gender, BirthDate, Phone, IdCardNo)
    VALUES (N'P00000001', N'张三', N'Male', '1990-01-01', N'13900000001', N'110101199001011234');
IF NOT EXISTS (SELECT 1 FROM pat.Patients WHERE PatientNo = N'P00000002')
    INSERT INTO pat.Patients (PatientNo, Name, Gender, BirthDate, Phone, IdCardNo)
    VALUES (N'P00000002', N'李四', N'Female', '1985-05-15', N'13900000002', N'110101198505152345');

/* ==================== 药品批次（用于发药功能） ==================== */
IF NOT EXISTS (SELECT 1 FROM pha.DrugBatches WHERE DrugCode = N'DRG001')
    INSERT INTO pha.DrugBatches (DrugCode, DrugName, Spec, BatchNo, ExpiryDate, TotalQuantity, AvailableQuantity)
    VALUES (N'DRG001', N'阿莫西林胶囊', N'0.25g*24粒', N'20250601', '2027-06-01', 1000, 1000);
IF NOT EXISTS (SELECT 1 FROM pha.DrugBatches WHERE DrugCode = N'DRG002')
    INSERT INTO pha.DrugBatches (DrugCode, DrugName, Spec, BatchNo, ExpiryDate, TotalQuantity, AvailableQuantity)
    VALUES (N'DRG002', N'头孢克肟片', N'50mg*12片', N'20250601', '2027-06-01', 500, 500);
IF NOT EXISTS (SELECT 1 FROM pha.DrugBatches WHERE DrugCode = N'DRG003')
    INSERT INTO pha.DrugBatches (DrugCode, DrugName, Spec, BatchNo, ExpiryDate, TotalQuantity, AvailableQuantity)
    VALUES (N'DRG003', N'布洛芬缓释胶囊', N'0.3g*20粒', N'20250601', '2027-06-01', 800, 800);

PRINT N'900_seed_minimal.sql 执行完成。';
GO

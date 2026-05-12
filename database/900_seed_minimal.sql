/*
  900_seed_minimal.sql
  最小演示数据：机构、院区、科室、人员、登录用户、角色与权限样例。
  可重复执行：使用 IF NOT EXISTS 判断。
  默认管理员密码占位：请在上线前改为真实哈希（当前为占位字符串，不可用于生产）。
*/
USE [Hospital];
GO

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM mdm.Organizations WHERE Code = N'DEMO_ORG')
    INSERT INTO mdm.Organizations (Code, Name) VALUES (N'DEMO_ORG', N'演示医疗集团');

DECLARE @OrgId BIGINT = (SELECT Id FROM mdm.Organizations WHERE Code = N'DEMO_ORG');

IF NOT EXISTS (SELECT 1 FROM mdm.Campuses WHERE Code = N'DEMO_CAMPUS' AND OrganizationId = @OrgId)
    INSERT INTO mdm.Campuses (OrganizationId, Code, Name) VALUES (@OrgId, N'DEMO_CAMPUS', N'演示院区');

DECLARE @CampusId BIGINT = (SELECT Id FROM mdm.Campuses WHERE Code = N'DEMO_CAMPUS' AND OrganizationId = @OrgId);

IF NOT EXISTS (SELECT 1 FROM mdm.Departments WHERE CampusId = @CampusId AND Code = N'ROOT')
    INSERT INTO mdm.Departments (CampusId, ParentId, Code, Name, DeptType, IsClinical)
    VALUES (@CampusId, NULL, N'ROOT', N'根科室', N'Admin', 0);

DECLARE @DeptId BIGINT = (SELECT Id FROM mdm.Departments WHERE CampusId = @CampusId AND Code = N'ROOT');

IF NOT EXISTS (SELECT 1 FROM mdm.Staff WHERE CampusId = @CampusId AND EmployeeNo = N'E0001')
    INSERT INTO mdm.Staff (CampusId, EmployeeNo, FullName, StaffCategory)
    VALUES (@CampusId, N'E0001', N'系统管理员', N'Admin');

DECLARE @StaffId BIGINT = (SELECT Id FROM mdm.Staff WHERE CampusId = @CampusId AND EmployeeNo = N'E0001');

IF NOT EXISTS (SELECT 1 FROM sec.Users WHERE LoginName = N'admin')
    INSERT INTO sec.Users (LoginName, PasswordHash, DisplayName, StaffId)
    VALUES (N'admin', N'__REPLACE_WITH_REAL_HASH__', N'管理员', @StaffId);

DECLARE @UserId BIGINT = (SELECT Id FROM sec.Users WHERE LoginName = N'admin');

IF NOT EXISTS (SELECT 1 FROM sec.Permissions WHERE Code = N'sys.shell.use')
    INSERT INTO sec.Permissions (Code, Module, Description) VALUES
    (N'sys.shell.use', N'sys', N'使用主壳'),
    (N'mdm.campus.manage', N'mdm', N'院区管理'),
    (N'opd.register.work', N'opd', N'挂号工作台');

IF NOT EXISTS (SELECT 1 FROM sec.Roles WHERE Code = N'ADMIN' AND (CampusId = @CampusId OR CampusId IS NULL))
    INSERT INTO sec.Roles (CampusId, Code, Name) VALUES (@CampusId, N'ADMIN', N'系统管理员');

DECLARE @RoleId BIGINT = (SELECT TOP 1 Id FROM sec.Roles WHERE Code = N'ADMIN' AND CampusId = @CampusId ORDER BY Id);

IF NOT EXISTS (SELECT 1 FROM sec.UserRoles WHERE UserId = @UserId AND RoleId = @RoleId AND CampusId = @CampusId)
    INSERT INTO sec.UserRoles (UserId, RoleId, CampusId) VALUES (@UserId, @RoleId, @CampusId);

INSERT INTO sec.RolePermissions (RoleId, PermissionId)
SELECT @RoleId, p.Id FROM sec.Permissions p
WHERE NOT EXISTS (SELECT 1 FROM sec.RolePermissions rp WHERE rp.RoleId = @RoleId AND rp.PermissionId = p.Id);

IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryTypes WHERE Code = N'GENDER')
    INSERT INTO mdm.DictionaryTypes (Code, Name, IsSystem) VALUES (N'GENDER', N'性别', 1);

IF NOT EXISTS (SELECT 1 FROM mdm.DictionaryItems WHERE TypeCode = N'GENDER' AND Value = N'M')
    INSERT INTO mdm.DictionaryItems (TypeCode, Value, DisplayName, SortOrder) VALUES
    (N'GENDER', N'M', N'男', 1),
    (N'GENDER', N'F', N'女', 2);

IF NOT EXISTS (SELECT 1 FROM mdm.ChargeItems WHERE CampusId = @CampusId AND Code = N'REG_FEE')
    INSERT INTO mdm.ChargeItems (CampusId, Code, Name, Unit, Price, Category)
    VALUES (@CampusId, N'REG_FEE', N'普通挂号费', N'次', 10.00, N'Registration');

IF NOT EXISTS (SELECT 1 FROM pha.StorageLocations WHERE CampusId = @CampusId AND Name = N'中心药库')
    INSERT INTO pha.StorageLocations (CampusId, Name, LocationType) VALUES (@CampusId, N'中心药库', N'Central');

GO

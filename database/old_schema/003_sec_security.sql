/*
  003_sec_security.sql
  用户、角色、权限、参数、集成端点、审计日志。
  说明：使用架构 sec，避免与系统架构 sys 冲突。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'sec.Users', N'U') IS NULL
BEGIN
    CREATE TABLE sec.Users
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_sec_Users PRIMARY KEY,
        LoginName        NVARCHAR(128)  NOT NULL,
        PasswordHash     NVARCHAR(500)  NOT NULL,
        DisplayName      NVARCHAR(100)  NULL,
        StaffId          BIGINT         NULL CONSTRAINT FK_sec_Users_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        IsLocked         BIT            NOT NULL CONSTRAINT DF_sec_Users_IsLocked DEFAULT (0),
        LastLoginAt      DATETIME2(3)   NULL,
        CreatedAt        DATETIME2(3)   NOT NULL CONSTRAINT DF_sec_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt        DATETIME2(3)   NULL,
        CONSTRAINT UQ_sec_Users_LoginName UNIQUE (LoginName)
    );
    CREATE INDEX IX_sec_Users_StaffId ON sec.Users (StaffId);
END
GO
EXEC dbo.sp_AddDescription N'sec', N'Users', NULL, N'登录用户。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'LoginName', N'登录名，全局唯一。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'PasswordHash', N'密码哈希（勿存明文）。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'DisplayName', N'显示名。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'StaffId', N'关联人员主档，可空（如外部账号）。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'IsLocked', N'是否锁定。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'LastLoginAt', N'最后登录时间（UTC）。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'sec', N'Users', N'UpdatedAt', N'最后更新时间（UTC）。';
GO

IF OBJECT_ID(N'sec.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE sec.Roles
    (
        Id          BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_sec_Roles PRIMARY KEY,
        CampusId    BIGINT         NULL CONSTRAINT FK_sec_Roles_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        Code        NVARCHAR(128)  NOT NULL,
        Name        NVARCHAR(200)  NOT NULL,
        IsActive    BIT            NOT NULL CONSTRAINT DF_sec_Roles_IsActive DEFAULT (1),
        CreatedAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_sec_Roles_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_sec_Roles_CampusCode UNIQUE (CampusId, Code)
    );
END
GO
EXEC dbo.sp_AddDescription N'sec', N'Roles', NULL, N'角色定义；CampusId 为空表示跨院区模板。';
EXEC dbo.sp_AddDescription N'sec', N'Roles', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'sec', N'Roles', N'CampusId', N'所属院区，空为全局角色。';
EXEC dbo.sp_AddDescription N'sec', N'Roles', N'Code', N'角色编码。';
EXEC dbo.sp_AddDescription N'sec', N'Roles', N'Name', N'角色名称。';
EXEC dbo.sp_AddDescription N'sec', N'Roles', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'sec', N'Roles', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'sec.UserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE sec.UserRoles
    (
        Id         BIGINT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_sec_UserRoles PRIMARY KEY,
        UserId     BIGINT NOT NULL CONSTRAINT FK_sec_UserRoles_Users FOREIGN KEY REFERENCES sec.Users (Id),
        RoleId     BIGINT NOT NULL CONSTRAINT FK_sec_UserRoles_Roles FOREIGN KEY REFERENCES sec.Roles (Id),
        CampusId   BIGINT NOT NULL CONSTRAINT FK_sec_UserRoles_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        CONSTRAINT UQ_sec_UserRoles UNIQUE (UserId, RoleId, CampusId)
    );
    CREATE INDEX IX_sec_UserRoles_UserId ON sec.UserRoles (UserId);
END
GO
EXEC dbo.sp_AddDescription N'sec', N'UserRoles', NULL, N'用户在某院区拥有的角色。';
EXEC dbo.sp_AddDescription N'sec', N'UserRoles', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'sec', N'UserRoles', N'UserId', N'用户主键。';
EXEC dbo.sp_AddDescription N'sec', N'UserRoles', N'RoleId', N'角色主键。';
EXEC dbo.sp_AddDescription N'sec', N'UserRoles', N'CampusId', N'授权院区。';
GO

IF OBJECT_ID(N'sec.Permissions', N'U') IS NULL
BEGIN
    CREATE TABLE sec.Permissions
    (
        Id          BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_sec_Permissions PRIMARY KEY,
        Code        NVARCHAR(200)  NOT NULL,
        Module      NVARCHAR(64)   NULL,
        Description NVARCHAR(500) NULL,
        CONSTRAINT UQ_sec_Permissions_Code UNIQUE (Code)
    );
END
GO
EXEC dbo.sp_AddDescription N'sec', N'Permissions', NULL, N'权限点（与前端 RouteKey/权限码对齐）。';
EXEC dbo.sp_AddDescription N'sec', N'Permissions', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'sec', N'Permissions', N'Code', N'权限编码，唯一。';
EXEC dbo.sp_AddDescription N'sec', N'Permissions', N'Module', N'所属模块前缀（如 mdm、opd）。';
EXEC dbo.sp_AddDescription N'sec', N'Permissions', N'Description', N'权限说明。';
GO

IF OBJECT_ID(N'sec.RolePermissions', N'U') IS NULL
BEGIN
    CREATE TABLE sec.RolePermissions
    (
        RoleId       BIGINT NOT NULL CONSTRAINT FK_sec_RolePermissions_Roles FOREIGN KEY REFERENCES sec.Roles (Id),
        PermissionId BIGINT NOT NULL CONSTRAINT FK_sec_RolePermissions_Permissions FOREIGN KEY REFERENCES sec.Permissions (Id),
        CONSTRAINT PK_sec_RolePermissions PRIMARY KEY (RoleId, PermissionId)
    );
END
GO
EXEC dbo.sp_AddDescription N'sec', N'RolePermissions', NULL, N'角色与权限多对多。';
EXEC dbo.sp_AddDescription N'sec', N'RolePermissions', N'RoleId', N'角色主键。';
EXEC dbo.sp_AddDescription N'sec', N'RolePermissions', N'PermissionId', N'权限主键。';
GO

IF OBJECT_ID(N'sec.SystemParameters', N'U') IS NULL
BEGIN
    CREATE TABLE sec.SystemParameters
    (
        Id          BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_sec_SystemParameters PRIMARY KEY,
        CampusId    BIGINT         NULL CONSTRAINT FK_sec_SystemParameters_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        ParamKey    NVARCHAR(128)  NOT NULL,
        ParamValue  NVARCHAR(MAX)  NULL,
        ValueType   NVARCHAR(32)   NOT NULL CONSTRAINT DF_sec_SystemParameters_ValueType DEFAULT (N'String'),
        UpdatedAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_sec_SystemParameters_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_sec_SystemParameters_CampusKey UNIQUE (CampusId, ParamKey)
    );
END
GO
EXEC dbo.sp_AddDescription N'sec', N'SystemParameters', NULL, N'系统参数；CampusId 为空表示全院全局。';
EXEC dbo.sp_AddDescription N'sec', N'SystemParameters', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'sec', N'SystemParameters', N'CampusId', N'院区，空为全局。';
EXEC dbo.sp_AddDescription N'sec', N'SystemParameters', N'ParamKey', N'参数键。';
EXEC dbo.sp_AddDescription N'sec', N'SystemParameters', N'ParamValue', N'参数值（文本存储）。';
EXEC dbo.sp_AddDescription N'sec', N'SystemParameters', N'ValueType', N'值类型说明（String/Json/Number）。';
EXEC dbo.sp_AddDescription N'sec', N'SystemParameters', N'UpdatedAt', N'最后更新时间（UTC）。';
GO

IF OBJECT_ID(N'sec.IntegrationEndpoints', N'U') IS NULL
BEGIN
    CREATE TABLE sec.IntegrationEndpoints
    (
        Id          BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_sec_IntegrationEndpoints PRIMARY KEY,
        Name        NVARCHAR(128)  NOT NULL,
        BaseUrl     NVARCHAR(500)  NOT NULL,
        AuthType    NVARCHAR(64)   NULL,
        ConfigJson  NVARCHAR(MAX)  NULL,
        IsActive    BIT            NOT NULL CONSTRAINT DF_sec_IntegrationEndpoints_IsActive DEFAULT (1),
        CreatedAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_sec_IntegrationEndpoints_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'sec', N'IntegrationEndpoints', NULL, N'外部集成端点（医保、LIS、PACS 等）。';
EXEC dbo.sp_AddDescription N'sec', N'IntegrationEndpoints', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'sec', N'IntegrationEndpoints', N'Name', N'端点名称。';
EXEC dbo.sp_AddDescription N'sec', N'IntegrationEndpoints', N'BaseUrl', N'基础 URL。';
EXEC dbo.sp_AddDescription N'sec', N'IntegrationEndpoints', N'AuthType', N'认证类型（None/OAuth2/Certificate 等）。';
EXEC dbo.sp_AddDescription N'sec', N'IntegrationEndpoints', N'ConfigJson', N'扩展配置 JSON。';
EXEC dbo.sp_AddDescription N'sec', N'IntegrationEndpoints', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'sec', N'IntegrationEndpoints', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'sec.AuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE sec.AuditLogs
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_sec_AuditLogs PRIMARY KEY,
        UserId       BIGINT         NULL CONSTRAINT FK_sec_AuditLogs_Users FOREIGN KEY REFERENCES sec.Users (Id),
        Action       NVARCHAR(128)  NOT NULL,
        EntityType   NVARCHAR(128)  NOT NULL,
        EntityId     NVARCHAR(128)  NULL,
        OccurredAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_sec_AuditLogs_OccurredAt DEFAULT (SYSUTCDATETIME()),
        DetailJson   NVARCHAR(MAX)  NULL,
        IpAddress    NVARCHAR(64)   NULL
    );
    CREATE INDEX IX_sec_AuditLogs_OccurredAt ON sec.AuditLogs (OccurredAt);
    CREATE INDEX IX_sec_AuditLogs_UserId ON sec.AuditLogs (UserId);
END
GO
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', NULL, N'操作审计日志。';
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', N'UserId', N'操作用户，可空（系统任务）。';
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', N'Action', N'动作编码（如 Login/Update/Delete）。';
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', N'EntityType', N'业务实体类型名。';
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', N'EntityId', N'业务实体主键（字符串形式）。';
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', N'OccurredAt', N'发生时间（UTC）。';
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', N'DetailJson', N'变更明细 JSON。';
EXEC dbo.sp_AddDescription N'sec', N'AuditLogs', N'IpAddress', N'来源 IP。';
GO

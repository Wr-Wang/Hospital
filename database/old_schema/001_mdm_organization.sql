/*
  001_mdm_organization.sql
  主数据：机构、院区、科室、病区床位、人员。
  依赖：000_init_database.sql
*/
USE [Hospital];
GO

SET NOCOUNT ON;

/* ---------- mdm.Organizations ---------- */
IF OBJECT_ID(N'mdm.Organizations', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.Organizations
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_Organizations PRIMARY KEY,
        Code         NVARCHAR(64)   NOT NULL,
        Name         NVARCHAR(200)  NOT NULL,
        IsActive     BIT            NOT NULL CONSTRAINT DF_mdm_Organizations_IsActive DEFAULT (1),
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_Organizations_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId BIGINT        NULL,
        UpdatedAt    DATETIME2(3)   NULL,
        UpdatedByUserId BIGINT        NULL,
        IsDeleted    BIT            NOT NULL CONSTRAINT DF_mdm_Organizations_IsDeleted DEFAULT (0),
        DeletedAt    DATETIME2(3)   NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', NULL, N'集团或法人机构主数据。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'Code', N'机构编码，院内唯一。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'Name', N'机构名称。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'CreatedByUserId', N'创建人用户主键（sec.Users）。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'UpdatedAt', N'最后更新时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'UpdatedByUserId', N'最后更新人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'IsDeleted', N'软删除标记。';
EXEC dbo.sp_AddDescription N'mdm', N'Organizations', N'DeletedAt', N'软删除时间（UTC）。';
GO

/* ---------- mdm.Campuses ---------- */
IF OBJECT_ID(N'mdm.Campuses', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.Campuses
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_Campuses PRIMARY KEY,
        OrganizationId   BIGINT         NOT NULL CONSTRAINT FK_mdm_Campuses_Organizations FOREIGN KEY REFERENCES mdm.Organizations (Id),
        Code             NVARCHAR(64)   NOT NULL,
        Name             NVARCHAR(200)  NOT NULL,
        IsActive         BIT            NOT NULL CONSTRAINT DF_mdm_Campuses_IsActive DEFAULT (1),
        CreatedAt        DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_Campuses_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId  BIGINT         NULL,
        UpdatedAt        DATETIME2(3)   NULL,
        UpdatedByUserId  BIGINT         NULL,
        IsDeleted        BIT            NOT NULL CONSTRAINT DF_mdm_Campuses_IsDeleted DEFAULT (0),
        DeletedAt        DATETIME2(3)   NULL,
        CONSTRAINT UQ_mdm_Campuses_OrgCode UNIQUE (OrganizationId, Code)
    );
    CREATE INDEX IX_mdm_Campuses_OrganizationId ON mdm.Campuses (OrganizationId);
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', NULL, N'院区（多院区模型核心维度）。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'OrganizationId', N'所属机构。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'Code', N'院区编码，机构内唯一。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'Name', N'院区名称。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'CreatedByUserId', N'创建人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'UpdatedAt', N'最后更新时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'UpdatedByUserId', N'最后更新人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'IsDeleted', N'软删除标记。';
EXEC dbo.sp_AddDescription N'mdm', N'Campuses', N'DeletedAt', N'软删除时间（UTC）。';
GO

/* ---------- mdm.Departments ---------- */
IF OBJECT_ID(N'mdm.Departments', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.Departments
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_Departments PRIMARY KEY,
        CampusId        BIGINT         NOT NULL CONSTRAINT FK_mdm_Departments_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        ParentId        BIGINT         NULL CONSTRAINT FK_mdm_Departments_Parent FOREIGN KEY REFERENCES mdm.Departments (Id),
        Code            NVARCHAR(64)   NOT NULL,
        Name            NVARCHAR(200)  NOT NULL,
        DeptType        NVARCHAR(64)   NULL,
        IsClinical      BIT            NOT NULL CONSTRAINT DF_mdm_Departments_IsClinical DEFAULT (1),
        IsActive        BIT            NOT NULL CONSTRAINT DF_mdm_Departments_IsActive DEFAULT (1),
        CreatedAt       DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_Departments_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId BIGINT         NULL,
        UpdatedAt       DATETIME2(3)   NULL,
        UpdatedByUserId BIGINT         NULL,
        IsDeleted       BIT            NOT NULL CONSTRAINT DF_mdm_Departments_IsDeleted DEFAULT (0),
        DeletedAt       DATETIME2(3)   NULL,
        CONSTRAINT UQ_mdm_Departments_CampusCode UNIQUE (CampusId, Code)
    );
    CREATE INDEX IX_mdm_Departments_CampusId ON mdm.Departments (CampusId);
    CREATE INDEX IX_mdm_Departments_ParentId ON mdm.Departments (ParentId);
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'Departments', NULL, N'科室树（含门诊/急诊/住院/医技等类型）。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'CampusId', N'所属院区。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'ParentId', N'上级科室，根节点为空。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'Code', N'科室编码，院区内唯一。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'Name', N'科室名称。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'DeptType', N'科室类型（字典项编码或自由文本）。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'IsClinical', N'是否临床科室。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'CreatedByUserId', N'创建人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'UpdatedAt', N'最后更新时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'UpdatedByUserId', N'最后更新人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'IsDeleted', N'软删除标记。';
EXEC dbo.sp_AddDescription N'mdm', N'Departments', N'DeletedAt', N'软删除时间（UTC）。';
GO

/* ---------- mdm.Wards ---------- */
IF OBJECT_ID(N'mdm.Wards', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.Wards
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_Wards PRIMARY KEY,
        CampusId        BIGINT         NOT NULL CONSTRAINT FK_mdm_Wards_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        DepartmentId    BIGINT         NOT NULL CONSTRAINT FK_mdm_Wards_Departments FOREIGN KEY REFERENCES mdm.Departments (Id),
        Code            NVARCHAR(64)   NOT NULL,
        Name            NVARCHAR(200)  NOT NULL,
        IsActive        BIT            NOT NULL CONSTRAINT DF_mdm_Wards_IsActive DEFAULT (1),
        CreatedAt       DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_Wards_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId BIGINT         NULL,
        UpdatedAt       DATETIME2(3)   NULL,
        UpdatedByUserId BIGINT         NULL,
        IsDeleted       BIT            NOT NULL CONSTRAINT DF_mdm_Wards_IsDeleted DEFAULT (0),
        DeletedAt       DATETIME2(3)   NULL,
        CONSTRAINT UQ_mdm_Wards_CampusCode UNIQUE (CampusId, Code)
    );
    CREATE INDEX IX_mdm_Wards_DepartmentId ON mdm.Wards (DepartmentId);
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'Wards', NULL, N'病区。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'CampusId', N'所属院区。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'DepartmentId', N'所属科室（护理单元归属）。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'Code', N'病区编码。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'Name', N'病区名称。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'CreatedByUserId', N'创建人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'UpdatedAt', N'最后更新时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'UpdatedByUserId', N'最后更新人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'IsDeleted', N'软删除标记。';
EXEC dbo.sp_AddDescription N'mdm', N'Wards', N'DeletedAt', N'软删除时间（UTC）。';
GO

/* ---------- mdm.Beds ---------- */
IF OBJECT_ID(N'mdm.Beds', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.Beds
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_Beds PRIMARY KEY,
        WardId          BIGINT         NOT NULL CONSTRAINT FK_mdm_Beds_Wards FOREIGN KEY REFERENCES mdm.Wards (Id),
        BedNo           NVARCHAR(32)   NOT NULL,
        Status          NVARCHAR(32)   NOT NULL CONSTRAINT DF_mdm_Beds_Status DEFAULT (N'Empty'),
        OccupiedByAdmissionId BIGINT   NULL, -- 外键在 012 脚本末尾添加，避免循环依赖
        CreatedAt       DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_Beds_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId BIGINT         NULL,
        UpdatedAt       DATETIME2(3)   NULL,
        UpdatedByUserId BIGINT         NULL,
        IsDeleted       BIT            NOT NULL CONSTRAINT DF_mdm_Beds_IsDeleted DEFAULT (0),
        DeletedAt       DATETIME2(3)   NULL,
        CONSTRAINT UQ_mdm_Beds_WardBedNo UNIQUE (WardId, BedNo)
    );
    CREATE INDEX IX_mdm_Beds_WardId ON mdm.Beds (WardId);
    CREATE INDEX IX_mdm_Beds_OccupiedByAdmissionId ON mdm.Beds (OccupiedByAdmissionId);
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'Beds', NULL, N'床位。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'WardId', N'所属病区。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'BedNo', N'床号（病区唯一）。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'Status', N'床位状态：Empty/Occupied/Maintenance 等。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'OccupiedByAdmissionId', N'当前占用住院主键（ipd.Admissions），空床为 NULL；外键在住院脚本中添加。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'CreatedByUserId', N'创建人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'UpdatedAt', N'最后更新时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'UpdatedByUserId', N'最后更新人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'IsDeleted', N'软删除标记。';
EXEC dbo.sp_AddDescription N'mdm', N'Beds', N'DeletedAt', N'软删除时间（UTC）。';
GO

/* ---------- mdm.Staff ---------- */
IF OBJECT_ID(N'mdm.Staff', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.Staff
    (
        Id                  BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_Staff PRIMARY KEY,
        CampusId            BIGINT         NOT NULL CONSTRAINT FK_mdm_Staff_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        EmployeeNo          NVARCHAR(64)   NOT NULL,
        FullName            NVARCHAR(100)  NOT NULL,
        StaffCategory       NVARCHAR(64)   NULL,
        LicenseNo           NVARCHAR(128)  NULL,
        LicenseExpireDate   DATE           NULL,
        IsActive            BIT            NOT NULL CONSTRAINT DF_mdm_Staff_IsActive DEFAULT (1),
        CreatedAt           DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_Staff_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId     BIGINT         NULL,
        UpdatedAt           DATETIME2(3)   NULL,
        UpdatedByUserId     BIGINT         NULL,
        IsDeleted           BIT            NOT NULL CONSTRAINT DF_mdm_Staff_IsDeleted DEFAULT (0),
        DeletedAt           DATETIME2(3)   NULL,
        CONSTRAINT UQ_mdm_Staff_CampusEmployeeNo UNIQUE (CampusId, EmployeeNo)
    );
    CREATE INDEX IX_mdm_Staff_CampusId ON mdm.Staff (CampusId);
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'Staff', NULL, N'工作人员主档（医生/护士/医技/行政等）。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'CampusId', N'主属院区。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'EmployeeNo', N'工号，院区内唯一。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'FullName', N'姓名。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'StaffCategory', N'人员类别（如 Doctor/Nurse）。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'LicenseNo', N'执业证号（可空）。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'LicenseExpireDate', N'执业证到期日（可空）。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'IsActive', N'是否在职/启用。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'CreatedByUserId', N'创建人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'UpdatedAt', N'最后更新时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'UpdatedByUserId', N'最后更新人用户主键。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'IsDeleted', N'软删除标记。';
EXEC dbo.sp_AddDescription N'mdm', N'Staff', N'DeletedAt', N'软删除时间（UTC）。';
GO

/* ---------- mdm.StaffDepartments ---------- */
IF OBJECT_ID(N'mdm.StaffDepartments', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.StaffDepartments
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_StaffDepartments PRIMARY KEY,
        StaffId         BIGINT         NOT NULL CONSTRAINT FK_mdm_StaffDepartments_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        DepartmentId    BIGINT         NOT NULL CONSTRAINT FK_mdm_StaffDepartments_Departments FOREIGN KEY REFERENCES mdm.Departments (Id),
        IsPrimary       BIT            NOT NULL CONSTRAINT DF_mdm_StaffDepartments_IsPrimary DEFAULT (0),
        StartDate       DATE           NOT NULL CONSTRAINT DF_mdm_StaffDepartments_StartDate DEFAULT (CAST(SYSUTCDATETIME() AS DATE)),
        EndDate         DATE           NULL,
        CreatedAt       DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_StaffDepartments_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId BIGINT         NULL,
        CONSTRAINT UQ_mdm_StaffDepartments_StaffDeptStart UNIQUE (StaffId, DepartmentId, StartDate)
    );
    CREATE INDEX IX_mdm_StaffDepartments_StaffId ON mdm.StaffDepartments (StaffId);
    CREATE INDEX IX_mdm_StaffDepartments_DepartmentId ON mdm.StaffDepartments (DepartmentId);
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', NULL, N'人员与科室多对多及轮转关系。';
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', N'StaffId', N'人员主键。';
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', N'DepartmentId', N'科室主键。';
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', N'IsPrimary', N'是否主科室。';
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', N'StartDate', N'生效开始日期。';
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', N'EndDate', N'生效结束日期，空表示当前有效。';
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'StaffDepartments', N'CreatedByUserId', N'创建人用户主键。';
GO

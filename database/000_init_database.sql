/*
  000_init_database.sql
  创建数据库、EF Core 模型对应的所有架构和表。
  使用 EnsureCreated() 自动生成，与 Hospital.Infrastructure 实体模型保持一致。
  排序规则 Chinese_PRC_CI_AS。
*/
SET NOCOUNT ON;
GO

DECLARE @DbName sysname = N'Hospital';
DECLARE @sql nvarchar(max);

IF DB_ID(@DbName) IS NULL
BEGIN
    SET @sql = N'CREATE DATABASE ' + QUOTENAME(@DbName) + N' COLLATE Chinese_PRC_CI_AS;';
    EXEC (@sql);
END
GO

USE [Hospital];
GO

-- ===== 架构 =====
IF SCHEMA_ID(N'sec') IS NULL EXEC(N'CREATE SCHEMA [sec];');
GO
IF SCHEMA_ID(N'mdm') IS NULL EXEC(N'CREATE SCHEMA [mdm];');
GO
IF SCHEMA_ID(N'fin') IS NULL EXEC(N'CREATE SCHEMA [fin];');
GO
IF SCHEMA_ID(N'enc') IS NULL EXEC(N'CREATE SCHEMA [enc];');
GO
IF SCHEMA_ID(N'pha') IS NULL EXEC(N'CREATE SCHEMA [pha];');
GO
IF SCHEMA_ID(N'rad') IS NULL EXEC(N'CREATE SCHEMA [rad];');
GO
IF SCHEMA_ID(N'lab') IS NULL EXEC(N'CREATE SCHEMA [lab];');
GO
IF SCHEMA_ID(N'pat') IS NULL EXEC(N'CREATE SCHEMA [pat];');
GO
IF SCHEMA_ID(N'opd') IS NULL EXEC(N'CREATE SCHEMA [opd];');
GO

-- ===== 审计日志 =====
CREATE TABLE [sec].[AuditLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [UserName] nvarchar(50) NOT NULL,
    [Action] nvarchar(128) NOT NULL,
    [EntityType] nvarchar(128) NOT NULL,
    [EntityId] bigint NOT NULL,
    [DetailJson] nvarchar(max) NULL,
    [NewValue] nvarchar(max) NULL,
    [IpAddress] nvarchar(64) NULL,
    [OccurredAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);
GO

-- ===== 院区 =====
CREATE TABLE [mdm].[Campuses] (
    [Id] bigint NOT NULL IDENTITY,
    [Code] nvarchar(64) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Address] nvarchar(500) NULL,
    [Phone] nvarchar(32) NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Campuses] PRIMARY KEY ([Id])
);
GO

-- ===== 科室 =====
CREATE TABLE [mdm].[Departments] (
    [Id] bigint NOT NULL IDENTITY,
    [Code] nvarchar(64) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [ParentId] bigint NULL,
    [CampusId] bigint NOT NULL,
    [DeptType] nvarchar(64) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Departments_Departments_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [mdm].[Departments] ([Id])
);
GO
CREATE INDEX [IX_Departments_ParentId] ON [mdm].[Departments] ([ParentId]);
GO

-- ===== 诊断 =====
CREATE TABLE [enc].[Diagnoses] (
    [Id] bigint NOT NULL IDENTITY,
    [OutpatientEncounterId] bigint NOT NULL,
    [DiagnosisType] nvarchar(64) NOT NULL,
    [IcdCode] nvarchar(32) NOT NULL,
    [IcdName] nvarchar(256) NOT NULL,
    [IsPrimary] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Diagnoses] PRIMARY KEY ([Id])
);
GO

-- ===== 字典类型 =====
CREATE TABLE [mdm].[DictionaryTypes] (
    [Id] bigint NOT NULL IDENTITY,
    [Code] nvarchar(64) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_DictionaryTypes] PRIMARY KEY ([Id])
);
GO
CREATE UNIQUE INDEX [IX_DictionaryTypes_Code] ON [mdm].[DictionaryTypes] ([Code]);
GO

-- ===== 发药 =====
CREATE TABLE [pha].[Dispenses] (
    [Id] bigint NOT NULL IDENTITY,
    [PrescriptionId] bigint NOT NULL,
    [DispensedBy] bigint NOT NULL,
    [Status] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Remark] nvarchar(500) NULL,
    CONSTRAINT [PK_Dispenses] PRIMARY KEY ([Id])
);
GO

-- ===== 药品批次 =====
CREATE TABLE [pha].[DrugBatches] (
    [Id] bigint NOT NULL IDENTITY,
    [DrugCode] nvarchar(50) NOT NULL,
    [DrugName] nvarchar(200) NOT NULL,
    [Spec] nvarchar(100) NOT NULL,
    [BatchNo] nvarchar(100) NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [TotalQuantity] decimal(18,4) NOT NULL,
    [AvailableQuantity] decimal(18,4) NOT NULL,
    CONSTRAINT [PK_DrugBatches] PRIMARY KEY ([Id])
);
GO

-- ===== 电子病历 =====
CREATE TABLE [enc].[EmrDocuments] (
    [Id] bigint NOT NULL IDENTITY,
    [OutpatientEncounterId] bigint NOT NULL,
    [ContentJson] nvarchar(4000) NOT NULL,
    [DocType] nvarchar(64) NOT NULL,
    [Version] int NOT NULL DEFAULT 1,
    CONSTRAINT [PK_EmrDocuments] PRIMARY KEY ([Id])
);
GO

-- ===== 影像检查 =====
CREATE TABLE [rad].[ImagingOrders] (
    [Id] bigint NOT NULL IDENTITY,
    [OutpatientEncounterId] bigint NOT NULL,
    [ItemCode] nvarchar(64) NOT NULL,
    [ItemName] nvarchar(256) NOT NULL,
    [Status] nvarchar(64) NOT NULL,
    CONSTRAINT [PK_ImagingOrders] PRIMARY KEY ([Id])
);
GO

-- ===== 发票（收费） =====
CREATE TABLE [fin].[Invoices] (
    [Id] bigint NOT NULL IDENTITY,
    [PayerPatientId] bigint NOT NULL,
    [PatientName] nvarchar(100) NOT NULL,
    [TotalAmount] decimal(18,4) NOT NULL DEFAULT 0.0,
    [Status] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [SettledAt] datetime2 NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id])
);
GO

-- ===== 检验检查 =====
CREATE TABLE [lab].[LabOrders] (
    [Id] bigint NOT NULL IDENTITY,
    [OutpatientEncounterId] bigint NOT NULL,
    [ItemCode] nvarchar(64) NOT NULL,
    [ItemName] nvarchar(256) NOT NULL,
    [Status] nvarchar(64) NOT NULL,
    CONSTRAINT [PK_LabOrders] PRIMARY KEY ([Id])
);
GO

-- ===== 门诊就诊 =====
CREATE TABLE [enc].[OutpatientEncounters] (
    [Id] bigint NOT NULL IDENTITY,
    [PatientId] bigint NOT NULL,
    [StaffId] bigint NOT NULL,
    [DepartmentId] bigint NOT NULL,
    [CampusId] bigint NOT NULL,
    [RegistrationId] bigint NOT NULL,
    [Status] nvarchar(64) NOT NULL,
    [StartedAt] datetime2 NULL,
    [EndedAt] datetime2 NULL,
    CONSTRAINT [PK_OutpatientEncounters] PRIMARY KEY ([Id])
);
GO

-- ===== 患者 =====
CREATE TABLE [pat].[Patients] (
    [Id] bigint NOT NULL IDENTITY,
    [PatientNo] nvarchar(64) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Gender] nvarchar(16) NULL,
    [BirthDate] date NULL,
    [Phone] nvarchar(32) NULL,
    [AllergiesText] nvarchar(1000) NULL,
    [IdCardNo] nvarchar(32) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([Id])
);
GO
CREATE UNIQUE INDEX [IX_Patients_PatientNo] ON [pat].[Patients] ([PatientNo]);
GO

-- ===== 处方 =====
CREATE TABLE [pha].[Prescriptions] (
    [Id] bigint NOT NULL IDENTITY,
    [OutpatientEncounterId] bigint NOT NULL,
    [PrescribedByStaffId] bigint NOT NULL,
    [Status] nvarchar(64) NOT NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id])
);
GO

-- ===== 挂号 =====
CREATE TABLE [opd].[Registrations] (
    [Id] bigint NOT NULL IDENTITY,
    [PatientId] bigint NOT NULL,
    [SlotId] bigint NOT NULL,
    [DoctorId] bigint NOT NULL,
    [DeptId] bigint NOT NULL,
    [CampusId] bigint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [QueueNo] int NOT NULL DEFAULT 0,
    [SlotName] nvarchar(64) NOT NULL,
    [Status] nvarchar(64) NOT NULL,
    CONSTRAINT [PK_Registrations] PRIMARY KEY ([Id])
);
GO

-- ===== 角色 =====
CREATE TABLE [sec].[Roles] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO
CREATE INDEX [IX_Roles_Name] ON [sec].[Roles] ([Name]);
GO

-- ===== 排班模板 =====
CREATE TABLE [opd].[ScheduleTemplates] (
    [Id] bigint NOT NULL IDENTITY,
    [DoctorId] bigint NOT NULL,
    [DepartmentId] bigint NOT NULL,
    [CampusId] bigint NOT NULL,
    [ScheduleDate] date NOT NULL,
    [Status] nvarchar(64) NOT NULL,
    CONSTRAINT [PK_ScheduleTemplates] PRIMARY KEY ([Id])
);
GO

-- ===== 人员（员工） =====
CREATE TABLE [mdm].[Staff] (
    [Id] bigint NOT NULL IDENTITY,
    [EmployeeNo] nvarchar(64) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Gender] nvarchar(16) NOT NULL,
    [Phone] nvarchar(32) NULL,
    [CampusId] bigint NOT NULL,
    [DepartmentId] bigint NOT NULL,
    [StaffCategory] nvarchar(64) NOT NULL,
    [LicenseNo] nvarchar(128) NOT NULL,
    [LicenseExpireDate] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Staff] PRIMARY KEY ([Id])
);
GO

-- ===== 系统用户 =====
CREATE TABLE [sec].[Users] (
    [Id] bigint NOT NULL IDENTITY,
    [LoginName] nvarchar(128) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [DisplayName] nvarchar(100) NOT NULL,
    [CampusName] nvarchar(50) NOT NULL,
    [IsLocked] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

-- ===== 字典项 =====
CREATE TABLE [mdm].[DictionaryItems] (
    [Id] bigint NOT NULL IDENTITY,
    [TypeId] bigint NOT NULL,
    [Code] nvarchar(64) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [ParentId] bigint NULL,
    [SortOrder] int NOT NULL DEFAULT 0,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_DictionaryItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DictionaryItems_DictionaryTypes_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [mdm].[DictionaryTypes] ([Id])
);
GO
CREATE UNIQUE INDEX [IX_DictionaryItems_TypeId_Code] ON [mdm].[DictionaryItems] ([TypeId], [Code]);
GO

-- ===== 发药明细 =====
CREATE TABLE [pha].[DispenseLines] (
    [Id] bigint NOT NULL IDENTITY,
    [DispensingId] bigint NOT NULL,
    [InventoryLotId] bigint NOT NULL,
    [DrugName] nvarchar(256) NOT NULL,
    [Spec] nvarchar(128) NOT NULL,
    [Qty] decimal(18,4) NOT NULL,
    CONSTRAINT [PK_DispenseLines] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DispenseLines_Dispenses_DispensingId] FOREIGN KEY ([DispensingId]) REFERENCES [pha].[Dispenses] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_DispenseLines_DispensingId] ON [pha].[DispenseLines] ([DispensingId]);
GO

-- ===== 收费明细 =====
CREATE TABLE [fin].[ChargeLines] (
    [Id] bigint NOT NULL IDENTITY,
    [BillingId] bigint NOT NULL,
    [ItemType] nvarchar(64) NOT NULL,
    [ItemName] nvarchar(256) NOT NULL,
    [Amount] decimal(18,4) NOT NULL,
    CONSTRAINT [PK_ChargeLines] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChargeLines_Invoices_BillingId] FOREIGN KEY ([BillingId]) REFERENCES [fin].[Invoices] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_ChargeLines_BillingId] ON [fin].[ChargeLines] ([BillingId]);
GO

-- ===== 支付记录 =====
CREATE TABLE [fin].[Payments] (
    [Id] bigint NOT NULL IDENTITY,
    [PayMethod] nvarchar(64) NOT NULL,
    [Amount] decimal(18,4) NOT NULL,
    [TransactionRef] nvarchar(200) NULL,
    [PaidAt] datetime2 NOT NULL,
    [BillingId] bigint NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Invoices_BillingId] FOREIGN KEY ([BillingId]) REFERENCES [fin].[Invoices] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_Payments_BillingId] ON [fin].[Payments] ([BillingId]);
GO

-- ===== 患者知情同意 =====
CREATE TABLE [pat].[PatientConsents] (
    [Id] bigint NOT NULL IDENTITY,
    [ConsentType] nvarchar(128) NOT NULL,
    [GrantedAt] datetimeoffset NOT NULL,
    [ExpiresAt] datetimeoffset NULL,
    [DocumentRef] nvarchar(500) NULL,
    [PatientId] bigint NOT NULL,
    CONSTRAINT [PK_PatientConsents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientConsents_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [pat].[Patients] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_PatientConsents_PatientId] ON [pat].[PatientConsents] ([PatientId]);
GO

-- ===== 患者标识 =====
CREATE TABLE [pat].[PatientIdentifiers] (
    [Id] bigint NOT NULL IDENTITY,
    [IdType] nvarchar(64) NOT NULL,
    [IdValue] nvarchar(128) NOT NULL,
    [IsPrimary] bit NOT NULL DEFAULT CAST(0 AS bit),
    [PatientId] bigint NOT NULL,
    CONSTRAINT [PK_PatientIdentifiers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientIdentifiers_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [pat].[Patients] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_PatientIdentifiers_PatientId] ON [pat].[PatientIdentifiers] ([PatientId]);
GO

-- ===== 处方明细 =====
CREATE TABLE [pha].[PrescriptionLines] (
    [Id] bigint NOT NULL IDENTITY,
    [PrescriptionId] bigint NOT NULL,
    [DrugName] nvarchar(256) NOT NULL,
    [Spec] nvarchar(128) NOT NULL,
    [Form] nvarchar(64) NOT NULL,
    [Frequency] nvarchar(64) NOT NULL,
    [Dose] nvarchar(64) NOT NULL,
    [Days] int NOT NULL,
    [Qty] decimal(18,4) NOT NULL,
    [Note] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PrescriptionLines] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionLines_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [pha].[Prescriptions] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_PrescriptionLines_PrescriptionId] ON [pha].[PrescriptionLines] ([PrescriptionId]);
GO

-- ===== 排班时段 =====
CREATE TABLE [opd].[ScheduleSlots] (
    [Id] bigint NOT NULL IDENTITY,
    [SlotType] nvarchar(64) NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [TotalQuota] int NOT NULL DEFAULT 0,
    [BookedQuota] int NOT NULL DEFAULT 0,
    [TemplateId] bigint NOT NULL,
    CONSTRAINT [PK_ScheduleSlots] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ScheduleSlots_ScheduleTemplates_TemplateId] FOREIGN KEY ([TemplateId]) REFERENCES [opd].[ScheduleTemplates] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_ScheduleSlots_TemplateId] ON [opd].[ScheduleSlots] ([TemplateId]);
GO

-- ===== 字典项(旧表)兼容索引 =====
CREATE INDEX [IX_ChargeLines_BillingId] ON [fin].[ChargeLines] ([BillingId]);
GO

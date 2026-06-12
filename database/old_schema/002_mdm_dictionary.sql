/*
  002_mdm_dictionary.sql
  字典类型/项、收费物价项目。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'mdm.DictionaryTypes', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.DictionaryTypes
    (
        Code        NVARCHAR(64)   NOT NULL CONSTRAINT PK_mdm_DictionaryTypes PRIMARY KEY,
        Name        NVARCHAR(200)  NOT NULL,
        IsSystem    BIT            NOT NULL CONSTRAINT DF_mdm_DictionaryTypes_IsSystem DEFAULT (0),
        CreatedAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_DictionaryTypes_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryTypes', NULL, N'字典分类（如 ICD_FREQ、SPECIMEN）。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryTypes', N'Code', N'分类编码，主键。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryTypes', N'Name', N'分类名称。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryTypes', N'IsSystem', N'是否系统内置（防删）。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryTypes', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'mdm.DictionaryItems', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.DictionaryItems
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_DictionaryItems PRIMARY KEY,
        TypeCode     NVARCHAR(64)   NOT NULL CONSTRAINT FK_mdm_DictionaryItems_Types FOREIGN KEY REFERENCES mdm.DictionaryTypes (Code),
        Value        NVARCHAR(128)  NOT NULL,
        DisplayName  NVARCHAR(256)  NOT NULL,
        SortOrder    INT            NOT NULL CONSTRAINT DF_mdm_DictionaryItems_SortOrder DEFAULT (0),
        IsActive     BIT            NOT NULL CONSTRAINT DF_mdm_DictionaryItems_IsActive DEFAULT (1),
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_DictionaryItems_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2(3)   NULL,
        CONSTRAINT UQ_mdm_DictionaryItems_TypeValue UNIQUE (TypeCode, Value)
    );
    CREATE INDEX IX_mdm_DictionaryItems_TypeCode ON mdm.DictionaryItems (TypeCode);
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', NULL, N'字典项明细。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', N'TypeCode', N'所属字典分类编码。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', N'Value', N'存储值（程序/接口使用）。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', N'DisplayName', N'显示名称。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', N'SortOrder', N'排序号。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'DictionaryItems', N'UpdatedAt', N'最后更新时间（UTC）。';
GO

IF OBJECT_ID(N'mdm.ChargeItems', N'U') IS NULL
BEGIN
    CREATE TABLE mdm.ChargeItems
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mdm_ChargeItems PRIMARY KEY,
        CampusId     BIGINT         NOT NULL CONSTRAINT FK_mdm_ChargeItems_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        Code         NVARCHAR(64)   NOT NULL,
        Name         NVARCHAR(256)  NOT NULL,
        Unit         NVARCHAR(32)   NULL,
        Price        DECIMAL(18, 4) NOT NULL CONSTRAINT DF_mdm_ChargeItems_Price DEFAULT (0),
        Category     NVARCHAR(128)  NULL,
        IsActive     BIT            NOT NULL CONSTRAINT DF_mdm_ChargeItems_IsActive DEFAULT (1),
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_mdm_ChargeItems_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2(3)   NULL,
        CONSTRAINT UQ_mdm_ChargeItems_CampusCode UNIQUE (CampusId, Code)
    );
    CREATE INDEX IX_mdm_ChargeItems_CampusId ON mdm.ChargeItems (CampusId);
END
GO
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', NULL, N'收费物价项目（与计费引擎联动）。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'CampusId', N'所属院区（价格可院区差异）。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'Code', N'项目编码，院区内唯一。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'Name', N'项目名称。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'Unit', N'计价单位。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'Price', N'标准单价。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'Category', N'项目分类。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'mdm', N'ChargeItems', N'UpdatedAt', N'最后更新时间（UTC）。';
GO

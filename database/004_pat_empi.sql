/*
  004_pat_empi.sql
  患者主索引、证件、合并日志、隐私授权。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'pat.Patients', N'U') IS NULL
BEGIN
    CREATE TABLE pat.Patients
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pat_Patients PRIMARY KEY,
        PatientNo       NVARCHAR(64)   NOT NULL,
        IdCardNo        NVARCHAR(32)   NULL,
        Name            NVARCHAR(100)  NOT NULL,
        Gender          NVARCHAR(16)   NULL,
        BirthDate       DATE           NULL,
        Phone           NVARCHAR(32)   NULL,
        AllergiesText   NVARCHAR(1000) NULL,
        CreatedAt       DATETIME2(3)   NOT NULL CONSTRAINT DF_pat_Patients_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt       DATETIME2(3)   NULL,
        IsDeleted       BIT            NOT NULL CONSTRAINT DF_pat_Patients_IsDeleted DEFAULT (0),
        DeletedAt       DATETIME2(3)   NULL,
        CONSTRAINT UQ_pat_Patients_PatientNo UNIQUE (PatientNo)
    );
    CREATE UNIQUE INDEX IX_pat_Patients_IdCardNo ON pat.Patients (IdCardNo) WHERE IdCardNo IS NOT NULL;
END
GO
EXEC dbo.sp_AddDescription N'pat', N'Patients', NULL, N'患者主索引（EMPI）。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'PatientNo', N'院内患者号，全局唯一。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'IdCardNo', N'身份证号（可空，唯一过滤索引）。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'Name', N'姓名。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'Gender', N'性别。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'BirthDate', N'出生日期。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'Phone', N'联系电话。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'AllergiesText', N'过敏史摘要文本。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'CreatedAt', N'建档时间（UTC）。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'UpdatedAt', N'最后更新时间（UTC）。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'IsDeleted', N'软删除标记。';
EXEC dbo.sp_AddDescription N'pat', N'Patients', N'DeletedAt', N'软删除时间（UTC）。';
GO

IF OBJECT_ID(N'pat.PatientIdentifiers', N'U') IS NULL
BEGIN
    CREATE TABLE pat.PatientIdentifiers
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pat_PatientIdentifiers PRIMARY KEY,
        PatientId    BIGINT         NOT NULL CONSTRAINT FK_pat_PatientIdentifiers_Patients FOREIGN KEY REFERENCES pat.Patients (Id),
        IdType       NVARCHAR(64)   NOT NULL,
        IdValue      NVARCHAR(128)  NOT NULL,
        IsPrimary    BIT            NOT NULL CONSTRAINT DF_pat_PatientIdentifiers_IsPrimary DEFAULT (0),
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_pat_PatientIdentifiers_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE INDEX IX_pat_PatientIdentifiers_PatientId ON pat.PatientIdentifiers (PatientId);
END
GO
EXEC dbo.sp_AddDescription N'pat', N'PatientIdentifiers', NULL, N'患者多证件/卡号。';
EXEC dbo.sp_AddDescription N'pat', N'PatientIdentifiers', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pat', N'PatientIdentifiers', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'pat', N'PatientIdentifiers', N'IdType', N'证件类型（医保卡/护照等）。';
EXEC dbo.sp_AddDescription N'pat', N'PatientIdentifiers', N'IdValue', N'证件号码或卡号。';
EXEC dbo.sp_AddDescription N'pat', N'PatientIdentifiers', N'IsPrimary', N'是否主证件。';
EXEC dbo.sp_AddDescription N'pat', N'PatientIdentifiers', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'pat.PatientMergeLogs', N'U') IS NULL
BEGIN
    CREATE TABLE pat.PatientMergeLogs
    (
        Id                  BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pat_PatientMergeLogs PRIMARY KEY,
        SurvivorPatientId   BIGINT         NOT NULL CONSTRAINT FK_pat_PatientMergeLogs_Survivor FOREIGN KEY REFERENCES pat.Patients (Id),
        MergedPatientId     BIGINT         NOT NULL,
        MergedByUserId      BIGINT         NULL CONSTRAINT FK_pat_PatientMergeLogs_Users FOREIGN KEY REFERENCES sec.Users (Id),
        MergedAt            DATETIME2(3)   NOT NULL CONSTRAINT DF_pat_PatientMergeLogs_MergedAt DEFAULT (SYSUTCDATETIME()),
        PayloadJson         NVARCHAR(MAX)  NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'pat', N'PatientMergeLogs', NULL, N'患者合并审计。';
EXEC dbo.sp_AddDescription N'pat', N'PatientMergeLogs', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pat', N'PatientMergeLogs', N'SurvivorPatientId', N'合并后保留的患者主键。';
EXEC dbo.sp_AddDescription N'pat', N'PatientMergeLogs', N'MergedPatientId', N'被合并的患者主键（历史引用，可不建外键）。';
EXEC dbo.sp_AddDescription N'pat', N'PatientMergeLogs', N'MergedByUserId', N'操作人用户主键。';
EXEC dbo.sp_AddDescription N'pat', N'PatientMergeLogs', N'MergedAt', N'合并时间（UTC）。';
EXEC dbo.sp_AddDescription N'pat', N'PatientMergeLogs', N'PayloadJson', N'合并前后快照 JSON。';
GO

IF OBJECT_ID(N'pat.PatientConsents', N'U') IS NULL
BEGIN
    CREATE TABLE pat.PatientConsents
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pat_PatientConsents PRIMARY KEY,
        PatientId    BIGINT         NOT NULL CONSTRAINT FK_pat_PatientConsents_Patients FOREIGN KEY REFERENCES pat.Patients (Id),
        ConsentType  NVARCHAR(128)  NOT NULL,
        GrantedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_pat_PatientConsents_GrantedAt DEFAULT (SYSUTCDATETIME()),
        ExpiresAt    DATETIME2(3)   NULL,
        DocumentRef  NVARCHAR(500)  NULL
    );
    CREATE INDEX IX_pat_PatientConsents_PatientId ON pat.PatientConsents (PatientId);
END
GO
EXEC dbo.sp_AddDescription N'pat', N'PatientConsents', NULL, N'患者隐私与数据授权记录。';
EXEC dbo.sp_AddDescription N'pat', N'PatientConsents', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pat', N'PatientConsents', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'pat', N'PatientConsents', N'ConsentType', N'授权类型。';
EXEC dbo.sp_AddDescription N'pat', N'PatientConsents', N'GrantedAt', N'授权时间（UTC）。';
EXEC dbo.sp_AddDescription N'pat', N'PatientConsents', N'ExpiresAt', N'过期时间（UTC），空表示长期。';
EXEC dbo.sp_AddDescription N'pat', N'PatientConsents', N'DocumentRef', N'知情同意书存储引用。';
GO

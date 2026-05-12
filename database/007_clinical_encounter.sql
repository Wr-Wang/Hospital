/*
  007_clinical_encounter.sql
  门诊就诊、急诊就诊（分表）、病历与诊断（互斥外键）。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'enc.OutpatientEncounters', N'U') IS NULL
BEGIN
    CREATE TABLE enc.OutpatientEncounters
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_enc_OutpatientEncounters PRIMARY KEY,
        CampusId         BIGINT         NOT NULL CONSTRAINT FK_enc_OPEC_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        PatientId        BIGINT         NOT NULL CONSTRAINT FK_enc_OPEC_Patients FOREIGN KEY REFERENCES pat.Patients (Id),
        RegistrationId   BIGINT         NOT NULL CONSTRAINT FK_enc_OPEC_Registrations FOREIGN KEY REFERENCES opd.Registrations (Id),
        DepartmentId     BIGINT         NOT NULL CONSTRAINT FK_enc_OPEC_Departments FOREIGN KEY REFERENCES mdm.Departments (Id),
        StaffId          BIGINT         NULL CONSTRAINT FK_enc_OPEC_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        Status           NVARCHAR(64)   NOT NULL CONSTRAINT DF_enc_OPEC_Status DEFAULT (N'Open'),
        StartedAt        DATETIME2(3)   NOT NULL CONSTRAINT DF_enc_OPEC_StartedAt DEFAULT (SYSUTCDATETIME()),
        EndedAt          DATETIME2(3)   NULL
    );
    CREATE INDEX IX_enc_OPEC_PatientId ON enc.OutpatientEncounters (PatientId);
    CREATE INDEX IX_enc_OPEC_RegistrationId ON enc.OutpatientEncounters (RegistrationId);
END
GO
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', NULL, N'门诊就诊实例。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'RegistrationId', N'挂号订单主键。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'DepartmentId', N'就诊科室。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'StaffId', N'接诊医生，可空。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'Status', N'就诊状态。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'StartedAt', N'开始接诊时间（UTC）。';
EXEC dbo.sp_AddDescription N'enc', N'OutpatientEncounters', N'EndedAt', N'结束就诊时间（UTC）。';
GO

IF OBJECT_ID(N'enc.EmergencyEncounters', N'U') IS NULL
BEGIN
    CREATE TABLE enc.EmergencyEncounters
    (
        Id                    BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_enc_EmergencyEncounters PRIMARY KEY,
        CampusId              BIGINT         NOT NULL CONSTRAINT FK_enc_EME_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        PatientId             BIGINT         NOT NULL CONSTRAINT FK_enc_EME_Patients FOREIGN KEY REFERENCES pat.Patients (Id),
        RegistrationId        BIGINT         NULL CONSTRAINT FK_enc_EME_Registrations FOREIGN KEY REFERENCES opd.Registrations (Id),
        TriageQueueEntryId    BIGINT         NULL CONSTRAINT FK_enc_EME_Triage FOREIGN KEY REFERENCES opd.TriageQueueEntries (Id),
        DepartmentId          BIGINT         NOT NULL CONSTRAINT FK_enc_EME_Departments FOREIGN KEY REFERENCES mdm.Departments (Id),
        StaffId               BIGINT         NULL CONSTRAINT FK_enc_EME_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        Status                NVARCHAR(64)   NOT NULL CONSTRAINT DF_enc_EME_Status DEFAULT (N'Open'),
        StartedAt             DATETIME2(3)   NOT NULL CONSTRAINT DF_enc_EME_StartedAt DEFAULT (SYSUTCDATETIME()),
        EndedAt               DATETIME2(3)   NULL
    );
    CREATE INDEX IX_enc_EME_PatientId ON enc.EmergencyEncounters (PatientId);
END
GO
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', NULL, N'急诊就诊实例。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'RegistrationId', N'挂号订单，绿通可空。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'TriageQueueEntryId', N'分诊记录，可空。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'DepartmentId', N'急诊科室。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'StaffId', N'接诊医生，可空。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'Status', N'就诊状态。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'StartedAt', N'开始接诊时间（UTC）。';
EXEC dbo.sp_AddDescription N'enc', N'EmergencyEncounters', N'EndedAt', N'结束就诊时间（UTC）。';
GO

IF OBJECT_ID(N'enc.EmrDocuments', N'U') IS NULL
BEGIN
    CREATE TABLE enc.EmrDocuments
    (
        Id                       BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_enc_EmrDocuments PRIMARY KEY,
        OutpatientEncounterId    BIGINT         NULL CONSTRAINT FK_enc_Emr_OPEC FOREIGN KEY REFERENCES enc.OutpatientEncounters (Id),
        EmergencyEncounterId     BIGINT         NULL CONSTRAINT FK_enc_Emr_EME FOREIGN KEY REFERENCES enc.EmergencyEncounters (Id),
        DocType                  NVARCHAR(64)   NOT NULL,
        ContentJson              NVARCHAR(MAX)  NULL,
        ContentRef               NVARCHAR(500)  NULL,
        Version                  INT            NOT NULL CONSTRAINT DF_enc_Emr_Version DEFAULT (1),
        SignedAt                 DATETIME2(3)   NULL,
        CreatedAt                DATETIME2(3)   NOT NULL CONSTRAINT DF_enc_Emr_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT CK_enc_EmrDocuments_OneEncounter CHECK (
            (CASE WHEN OutpatientEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN EmergencyEncounterId IS NOT NULL THEN 1 ELSE 0 END) = 1
        )
    );
    CREATE INDEX IX_enc_Emr_OPEC ON enc.EmrDocuments (OutpatientEncounterId);
    CREATE INDEX IX_enc_Emr_EME ON enc.EmrDocuments (EmergencyEncounterId);
END
GO
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', NULL, N'病历/文书（门诊或急诊互斥关联）。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'OutpatientEncounterId', N'门诊就诊主键，与急诊二选一。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'EmergencyEncounterId', N'急诊就诊主键，与门诊二选一。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'DocType', N'文书类型。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'ContentJson', N'结构化内容 JSON。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'ContentRef', N'大文本/附件外部存储引用。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'Version', N'版本号。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'SignedAt', N'签名时间（UTC）。';
EXEC dbo.sp_AddDescription N'enc', N'EmrDocuments', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'enc.Diagnoses', N'U') IS NULL
BEGIN
    CREATE TABLE enc.Diagnoses
    (
        Id                       BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_enc_Diagnoses PRIMARY KEY,
        OutpatientEncounterId    BIGINT         NULL CONSTRAINT FK_enc_Dx_OPEC FOREIGN KEY REFERENCES enc.OutpatientEncounters (Id),
        EmergencyEncounterId     BIGINT         NULL CONSTRAINT FK_enc_Dx_EME FOREIGN KEY REFERENCES enc.EmergencyEncounters (Id),
        IcdCode                  NVARCHAR(32)   NOT NULL,
        IcdName                  NVARCHAR(256)  NOT NULL,
        DiagnosisType            NVARCHAR(64)   NULL,
        IsPrimary                BIT            NOT NULL CONSTRAINT DF_enc_Dx_IsPrimary DEFAULT (0),
        CreatedAt                DATETIME2(3)   NOT NULL CONSTRAINT DF_enc_Dx_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT CK_enc_Diagnoses_OneEncounter CHECK (
            (CASE WHEN OutpatientEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN EmergencyEncounterId IS NOT NULL THEN 1 ELSE 0 END) = 1
        )
    );
    CREATE INDEX IX_enc_Dx_OPEC ON enc.Diagnoses (OutpatientEncounterId);
    CREATE INDEX IX_enc_Dx_EME ON enc.Diagnoses (EmergencyEncounterId);
END
GO
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', NULL, N'诊断（门诊或急诊互斥关联）。';
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', N'OutpatientEncounterId', N'门诊就诊主键。';
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', N'EmergencyEncounterId', N'急诊就诊主键。';
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', N'IcdCode', N'ICD 诊断编码。';
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', N'IcdName', N'ICD 诊断名称。';
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', N'DiagnosisType', N'诊断类别（入院/出院/门诊等）。';
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', N'IsPrimary', N'是否主诊断。';
EXEC dbo.sp_AddDescription N'enc', N'Diagnoses', N'CreatedAt', N'创建时间（UTC）。';
GO

/*
  011_mon_monitoring.sql
  生命体征、危急值、ICU 波形元数据、远程设备绑定。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'mon.VitalSignSets', N'U') IS NULL
BEGIN
    CREATE TABLE mon.VitalSignSets
    (
        Id                        BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mon_VSS PRIMARY KEY,
        OutpatientEncounterId     BIGINT         NULL CONSTRAINT FK_mon_VSS_OPEC FOREIGN KEY REFERENCES enc.OutpatientEncounters (Id),
        EmergencyEncounterId      BIGINT         NULL CONSTRAINT FK_mon_VSS_EME FOREIGN KEY REFERENCES enc.EmergencyEncounters (Id),
        AdmissionId               BIGINT         NULL,
        RecordedAt                DATETIME2(3)   NOT NULL CONSTRAINT DF_mon_VSS_RecordedAt DEFAULT (SYSUTCDATETIME()),
        Source                    NVARCHAR(64)   NOT NULL CONSTRAINT DF_mon_VSS_Source DEFAULT (N'Manual'),
        CONSTRAINT CK_mon_VSS_Context CHECK (
            (CASE WHEN OutpatientEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN EmergencyEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN AdmissionId IS NOT NULL THEN 1 ELSE 0 END) = 1
        )
    );
    CREATE INDEX IX_mon_VSS_OPEC ON mon.VitalSignSets (OutpatientEncounterId);
    CREATE INDEX IX_mon_VSS_EME ON mon.VitalSignSets (EmergencyEncounterId);
    CREATE INDEX IX_mon_VSS_Adm ON mon.VitalSignSets (AdmissionId);
END
GO
EXEC dbo.sp_AddDescription N'mon', N'VitalSignSets', NULL, N'生命体征采集头（门/急/住三选一；住院 AdmissionId 外键在 012 添加）。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignSets', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignSets', N'OutpatientEncounterId', N'门诊就诊主键。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignSets', N'EmergencyEncounterId', N'急诊就诊主键。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignSets', N'AdmissionId', N'住院主键。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignSets', N'RecordedAt', N'记录时间（UTC）。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignSets', N'Source', N'来源（手工/设备）。';
GO

IF OBJECT_ID(N'mon.VitalSignItems', N'U') IS NULL
BEGIN
    CREATE TABLE mon.VitalSignItems
    (
        Id        BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mon_VSI PRIMARY KEY,
        SetId     BIGINT         NOT NULL CONSTRAINT FK_mon_VSI_Sets FOREIGN KEY REFERENCES mon.VitalSignSets (Id) ON DELETE CASCADE,
        Code      NVARCHAR(64)   NOT NULL,
        Value     NVARCHAR(128)  NOT NULL,
        Unit      NVARCHAR(32)   NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'mon', N'VitalSignItems', NULL, N'生命体征明细项。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignItems', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignItems', N'SetId', N'体征采集头主键。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignItems', N'Code', N'项目编码（T/PR/RR/BP 等）。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignItems', N'Value', N'测量值。';
EXEC dbo.sp_AddDescription N'mon', N'VitalSignItems', N'Unit', N'单位。';
GO

IF OBJECT_ID(N'mon.CriticalValues', N'U') IS NULL
BEGIN
    CREATE TABLE mon.CriticalValues
    (
        Id             BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mon_CV PRIMARY KEY,
        SourceSystem   NVARCHAR(64)   NOT NULL,
        RefId          NVARCHAR(128)  NOT NULL,
        PatientId      BIGINT         NULL CONSTRAINT FK_mon_CV_Patient FOREIGN KEY REFERENCES pat.Patients (Id),
        AcknowledgedAt DATETIME2(3)   NULL,
        ClosedAt       DATETIME2(3)   NULL,
        CreatedAt      DATETIME2(3)   NOT NULL CONSTRAINT DF_mon_CV_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'mon', N'CriticalValues', NULL, N'危急值消息与闭环状态。';
EXEC dbo.sp_AddDescription N'mon', N'CriticalValues', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mon', N'CriticalValues', N'SourceSystem', N'来源系统（LIS/PACS/监护）。';
EXEC dbo.sp_AddDescription N'mon', N'CriticalValues', N'RefId', N'来源侧业务主键（字符串）。';
EXEC dbo.sp_AddDescription N'mon', N'CriticalValues', N'PatientId', N'患者主键（冗余）。';
EXEC dbo.sp_AddDescription N'mon', N'CriticalValues', N'AcknowledgedAt', N'医护确认时间（UTC）。';
EXEC dbo.sp_AddDescription N'mon', N'CriticalValues', N'ClosedAt', N'闭环完成时间（UTC）。';
EXEC dbo.sp_AddDescription N'mon', N'CriticalValues', N'CreatedAt', N'产生时间（UTC）。';
GO

IF OBJECT_ID(N'mon.IcuWaveformSessions', N'U') IS NULL
BEGIN
    CREATE TABLE mon.IcuWaveformSessions
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mon_IcuWave PRIMARY KEY,
        AdmissionId  BIGINT         NULL,
        BedId        BIGINT         NULL CONSTRAINT FK_mon_IcuWave_Bed FOREIGN KEY REFERENCES mdm.Beds (Id),
        StartedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_mon_IcuWave_Start DEFAULT (SYSUTCDATETIME()),
        EndedAt      DATETIME2(3)   NULL,
        StorageKey   NVARCHAR(500)  NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'mon', N'IcuWaveformSessions', NULL, N'ICU 波形会话元数据（波形本体存对象存储）。';
EXEC dbo.sp_AddDescription N'mon', N'IcuWaveformSessions', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mon', N'IcuWaveformSessions', N'AdmissionId', N'住院主键（外键在 012 添加）。';
EXEC dbo.sp_AddDescription N'mon', N'IcuWaveformSessions', N'BedId', N'床位主键。';
EXEC dbo.sp_AddDescription N'mon', N'IcuWaveformSessions', N'StartedAt', N'会话开始（UTC）。';
EXEC dbo.sp_AddDescription N'mon', N'IcuWaveformSessions', N'EndedAt', N'会话结束（UTC）。';
EXEC dbo.sp_AddDescription N'mon', N'IcuWaveformSessions', N'StorageKey', N'对象存储键或 URL。';
GO

IF OBJECT_ID(N'mon.RemoteDevices', N'U') IS NULL
BEGIN
    CREATE TABLE mon.RemoteDevices
    (
        Id          BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_mon_RemoteDev PRIMARY KEY,
        PatientId   BIGINT         NOT NULL CONSTRAINT FK_mon_Remote_Patient FOREIGN KEY REFERENCES pat.Patients (Id),
        DeviceUid   NVARCHAR(128)  NOT NULL,
        BoundAt     DATETIME2(3)   NOT NULL CONSTRAINT DF_mon_Remote_BoundAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_mon_Remote_DeviceUid UNIQUE (DeviceUid)
    );
END
GO
EXEC dbo.sp_AddDescription N'mon', N'RemoteDevices', NULL, N'远程监护设备与患者绑定。';
EXEC dbo.sp_AddDescription N'mon', N'RemoteDevices', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'mon', N'RemoteDevices', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'mon', N'RemoteDevices', N'DeviceUid', N'设备唯一标识。';
EXEC dbo.sp_AddDescription N'mon', N'RemoteDevices', N'BoundAt', N'绑定时间（UTC）。';
GO

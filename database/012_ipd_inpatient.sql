/*
  012_ipd_inpatient.sql
  住院：入院、转床、预交金、医嘱与执行、护理与体温单。
  末尾：为 008/011 中 AdmissionId 及床位占用补充外键。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'ipd.Admissions', N'U') IS NULL
BEGIN
    CREATE TABLE ipd.Admissions
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ipd_Admissions PRIMARY KEY,
        CampusId        BIGINT         NOT NULL CONSTRAINT FK_ipd_Adm_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        PatientId       BIGINT         NOT NULL CONSTRAINT FK_ipd_Adm_Patients FOREIGN KEY REFERENCES pat.Patients (Id),
        AdmissionNo     NVARCHAR(64)   NOT NULL,
        DepartmentId    BIGINT         NOT NULL CONSTRAINT FK_ipd_Adm_Dept FOREIGN KEY REFERENCES mdm.Departments (Id),
        BedId           BIGINT         NULL CONSTRAINT FK_ipd_Adm_Beds FOREIGN KEY REFERENCES mdm.Beds (Id),
        Status          NVARCHAR(64)   NOT NULL CONSTRAINT DF_ipd_Adm_Status DEFAULT (N'InHospital'),
        AdmittedAt      DATETIME2(3)   NOT NULL CONSTRAINT DF_ipd_Adm_AdmittedAt DEFAULT (SYSUTCDATETIME()),
        DischargedAt    DATETIME2(3)   NULL,
        CONSTRAINT UQ_ipd_Admissions_CampusNo UNIQUE (CampusId, AdmissionNo)
    );
    CREATE INDEX IX_ipd_Adm_Patient ON ipd.Admissions (PatientId);
END
GO
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', NULL, N'住院记录（一次入院一条）。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'AdmissionNo', N'住院号，院区内唯一。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'DepartmentId', N'当前所在科室。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'BedId', N'当前床位，可空（待分床）。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'Status', N'在院状态。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'AdmittedAt', N'入院时间（UTC）。';
EXEC dbo.sp_AddDescription N'ipd', N'Admissions', N'DischargedAt', N'出院时间（UTC）。';
GO

IF OBJECT_ID(N'ipd.AdmissionTransfers', N'U') IS NULL
BEGIN
    CREATE TABLE ipd.AdmissionTransfers
    (
        Id            BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ipd_Transfers PRIMARY KEY,
        AdmissionId   BIGINT         NOT NULL CONSTRAINT FK_ipd_Tr_Adm FOREIGN KEY REFERENCES ipd.Admissions (Id),
        FromBedId     BIGINT         NULL,
        ToBedId       BIGINT         NULL,
        ToDepartmentId BIGINT        NULL CONSTRAINT FK_ipd_Tr_ToDept FOREIGN KEY REFERENCES mdm.Departments (Id),
        TransferredAt DATETIME2(3)   NOT NULL CONSTRAINT DF_ipd_Tr_At DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'ipd', N'AdmissionTransfers', NULL, N'转科转床记录。';
EXEC dbo.sp_AddDescription N'ipd', N'AdmissionTransfers', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'ipd', N'AdmissionTransfers', N'AdmissionId', N'住院主键。';
EXEC dbo.sp_AddDescription N'ipd', N'AdmissionTransfers', N'FromBedId', N'原床位主键（历史快照，可不建外键）。';
EXEC dbo.sp_AddDescription N'ipd', N'AdmissionTransfers', N'ToBedId', N'新床位主键。';
EXEC dbo.sp_AddDescription N'ipd', N'AdmissionTransfers', N'ToDepartmentId', N'新科室。';
EXEC dbo.sp_AddDescription N'ipd', N'AdmissionTransfers', N'TransferredAt', N'转科/转床时间（UTC）。';
GO

IF OBJECT_ID(N'ipd.DepositTransactions', N'U') IS NULL
BEGIN
    CREATE TABLE ipd.DepositTransactions
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ipd_Deposit PRIMARY KEY,
        AdmissionId  BIGINT         NOT NULL CONSTRAINT FK_ipd_Dep_Adm FOREIGN KEY REFERENCES ipd.Admissions (Id),
        Amount       DECIMAL(18, 4) NOT NULL,
        TxnType      NVARCHAR(32)   NOT NULL,
        OccurredAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_ipd_Dep_OccurredAt DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'ipd', N'DepositTransactions', NULL, N'住院预交金流水。';
EXEC dbo.sp_AddDescription N'ipd', N'DepositTransactions', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'ipd', N'DepositTransactions', N'AdmissionId', N'住院主键。';
EXEC dbo.sp_AddDescription N'ipd', N'DepositTransactions', N'Amount', N'金额（正缴负退）。';
EXEC dbo.sp_AddDescription N'ipd', N'DepositTransactions', N'TxnType', N'交易类型（Deposit/Refund）。';
EXEC dbo.sp_AddDescription N'ipd', N'DepositTransactions', N'OccurredAt', N'发生时间（UTC）。';
GO

IF OBJECT_ID(N'ipd.InpatientOrders', N'U') IS NULL
BEGIN
    CREATE TABLE ipd.InpatientOrders
    (
        Id                 BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ipd_InpatientOrders PRIMARY KEY,
        AdmissionId        BIGINT         NOT NULL CONSTRAINT FK_ipd_IO_Adm FOREIGN KEY REFERENCES ipd.Admissions (Id),
        OrderType          NVARCHAR(64)   NOT NULL,
        Status             NVARCHAR(64)   NOT NULL CONSTRAINT DF_ipd_IO_Status DEFAULT (N'Active'),
        StartTime          DATETIME2(3)   NULL,
        StopTime           DATETIME2(3)   NULL,
        OrderedByStaffId   BIGINT         NULL CONSTRAINT FK_ipd_IO_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        CreatedAt          DATETIME2(3)   NOT NULL CONSTRAINT DF_ipd_IO_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE INDEX IX_ipd_IO_Adm ON ipd.InpatientOrders (AdmissionId);
END
GO
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', NULL, N'住院长临嘱（CPOE），与处方表二选一承载药品时由院规约束。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', N'AdmissionId', N'住院主键。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', N'OrderType', N'医嘱类型（药疗/检验/护理等）。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', N'Status', N'医嘱状态。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', N'StartTime', N'开始执行时间（UTC）。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', N'StopTime', N'停止时间（UTC）。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', N'OrderedByStaffId', N'开立医生。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrders', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'ipd.InpatientOrderLines', N'U') IS NULL
BEGIN
    CREATE TABLE ipd.InpatientOrderLines
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ipd_IOL PRIMARY KEY,
        InpatientOrderId BIGINT         NOT NULL CONSTRAINT FK_ipd_IOL_IO FOREIGN KEY REFERENCES ipd.InpatientOrders (Id) ON DELETE CASCADE,
        [LineNo]         INT            NOT NULL CONSTRAINT DF_ipd_IOL_LineNo DEFAULT (1),
        ItemText         NVARCHAR(500)  NOT NULL,
        ChargeItemId     BIGINT         NULL CONSTRAINT FK_ipd_IOL_Charge FOREIGN KEY REFERENCES mdm.ChargeItems (Id)
    );
END

IF OBJECT_ID(N'ipd.InpatientOrderLines', N'U') IS NOT NULL
BEGIN
    DECLARE @constraintName sysname;
    DECLARE @indexName sysname;

    SELECT TOP 1 @constraintName = kc.name
    FROM sys.key_constraints kc
    JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE kc.parent_object_id = OBJECT_ID(N'ipd.InpatientOrderLines')
      AND c.name = N'LineNo'
      AND kc.type_desc IN (N'PRIMARY_KEY_CONSTRAINT', N'UNIQUE_CONSTRAINT');

    IF @constraintName IS NOT NULL
    BEGIN
        EXEC(N'ALTER TABLE ipd.InpatientOrderLines DROP CONSTRAINT [' + @constraintName + ']');
    END

    SELECT TOP 1 @indexName = i.name
    FROM sys.indexes i
    JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE i.object_id = OBJECT_ID(N'ipd.InpatientOrderLines')
      AND c.name = N'LineNo'
      AND i.is_unique = 1
      AND i.is_primary_key = 0;

    IF @indexName IS NOT NULL
    BEGIN
        EXEC(N'DROP INDEX [' + @indexName + '] ON ipd.InpatientOrderLines');
    END
END
GO
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrderLines', NULL, N'住院医嘱明细（简化文本+可选收费项）。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrderLines', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrderLines', N'InpatientOrderId', N'住院医嘱头主键。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrderLines', N'LineNo', N'行号（非关键字段）。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrderLines', N'ItemText', N'医嘱内容描述。';
EXEC dbo.sp_AddDescription N'ipd', N'InpatientOrderLines', N'ChargeItemId', N'关联收费项目，可空。';
GO

IF OBJECT_ID(N'ipd.OrderExecutions', N'U') IS NULL
BEGIN
    CREATE TABLE ipd.OrderExecutions
    (
        Id                BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ipd_OrderExec PRIMARY KEY,
        InpatientOrderId  BIGINT         NOT NULL CONSTRAINT FK_ipd_OE_IO FOREIGN KEY REFERENCES ipd.InpatientOrders (Id),
        ExecutedAt        DATETIME2(3)   NOT NULL CONSTRAINT DF_ipd_OE_ExecAt DEFAULT (SYSUTCDATETIME()),
        ExecutorUserId    BIGINT         NULL CONSTRAINT FK_ipd_OE_User FOREIGN KEY REFERENCES sec.Users (Id),
        Barcode           NVARCHAR(128)  NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'ipd', N'OrderExecutions', NULL, N'住院医嘱护士执行记录。';
EXEC dbo.sp_AddDescription N'ipd', N'OrderExecutions', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'ipd', N'OrderExecutions', N'InpatientOrderId', N'住院医嘱主键。';
EXEC dbo.sp_AddDescription N'ipd', N'OrderExecutions', N'ExecutedAt', N'执行时间（UTC）。';
EXEC dbo.sp_AddDescription N'ipd', N'OrderExecutions', N'ExecutorUserId', N'执行人用户主键。';
EXEC dbo.sp_AddDescription N'ipd', N'OrderExecutions', N'Barcode', N'扫码条码记录。';
GO

IF OBJECT_ID(N'ipd.NursingRecords', N'U') IS NULL
BEGIN
    CREATE TABLE ipd.NursingRecords
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ipd_Nursing PRIMARY KEY,
        AdmissionId  BIGINT         NOT NULL CONSTRAINT FK_ipd_Nurse_Adm FOREIGN KEY REFERENCES ipd.Admissions (Id),
        RecordType   NVARCHAR(64)   NOT NULL,
        ContentJson  NVARCHAR(MAX)  NULL,
        RecordedAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_ipd_Nurse_At DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'ipd', N'NursingRecords', NULL, N'护理记录。';
EXEC dbo.sp_AddDescription N'ipd', N'NursingRecords', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'ipd', N'NursingRecords', N'AdmissionId', N'住院主键。';
EXEC dbo.sp_AddDescription N'ipd', N'NursingRecords', N'RecordType', N'记录类型。';
EXEC dbo.sp_AddDescription N'ipd', N'NursingRecords', N'ContentJson', N'结构化内容 JSON。';
EXEC dbo.sp_AddDescription N'ipd', N'NursingRecords', N'RecordedAt', N'记录时间（UTC）。';
GO

IF OBJECT_ID(N'ipd.TemperatureSheetEntries', N'U') IS NULL
BEGIN
    CREATE TABLE ipd.TemperatureSheetEntries
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ipd_Temp PRIMARY KEY,
        AdmissionId  BIGINT         NOT NULL CONSTRAINT FK_ipd_Temp_Adm FOREIGN KEY REFERENCES ipd.Admissions (Id),
        ChartDate    DATE           NOT NULL,
        PointsJson   NVARCHAR(MAX)  NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'ipd', N'TemperatureSheetEntries', NULL, N'体温单（可按天一条 JSON 点集）。';
EXEC dbo.sp_AddDescription N'ipd', N'TemperatureSheetEntries', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'ipd', N'TemperatureSheetEntries', N'AdmissionId', N'住院主键。';
EXEC dbo.sp_AddDescription N'ipd', N'TemperatureSheetEntries', N'ChartDate', N'图表日期。';
EXEC dbo.sp_AddDescription N'ipd', N'TemperatureSheetEntries', N'PointsJson', N'体温脉搏等采样点 JSON。';
GO

/* ---------- 外键：住院主键回填 ---------- */
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_lab_LabOrders_Admissions')
    ALTER TABLE lab.LabOrders WITH CHECK ADD CONSTRAINT FK_lab_LabOrders_Admissions FOREIGN KEY (AdmissionId) REFERENCES ipd.Admissions (Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_rad_ImagingOrders_Admissions')
    ALTER TABLE rad.ImagingOrders WITH CHECK ADD CONSTRAINT FK_rad_ImagingOrders_Admissions FOREIGN KEY (AdmissionId) REFERENCES ipd.Admissions (Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_pha_Prescriptions_Admissions')
    ALTER TABLE pha.Prescriptions WITH CHECK ADD CONSTRAINT FK_pha_Prescriptions_Admissions FOREIGN KEY (AdmissionId) REFERENCES ipd.Admissions (Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_mon_VSS_Admissions')
    ALTER TABLE mon.VitalSignSets WITH CHECK ADD CONSTRAINT FK_mon_VSS_Admissions FOREIGN KEY (AdmissionId) REFERENCES ipd.Admissions (Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_mon_IcuWave_Admissions')
    ALTER TABLE mon.IcuWaveformSessions WITH CHECK ADD CONSTRAINT FK_mon_IcuWave_Admissions FOREIGN KEY (AdmissionId) REFERENCES ipd.Admissions (Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_mdm_Beds_Admissions')
    ALTER TABLE mdm.Beds WITH CHECK ADD CONSTRAINT FK_mdm_Beds_Admissions FOREIGN KEY (OccupiedByAdmissionId) REFERENCES ipd.Admissions (Id);
GO

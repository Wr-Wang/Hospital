/*
  010_eqp_asset.sql
  设备台账、流转、巡检、工单、计量。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'eqp.Assets', N'U') IS NULL
BEGIN
    CREATE TABLE eqp.Assets
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_eqp_Assets PRIMARY KEY,
        CampusId     BIGINT         NOT NULL CONSTRAINT FK_eqp_Assets_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        AssetCode    NVARCHAR(64)   NOT NULL,
        Name         NVARCHAR(256)  NOT NULL,
        Category     NVARCHAR(128)  NULL,
        Manufacturer NVARCHAR(200)  NULL,
        Model        NVARCHAR(128)  NULL,
        Status       NVARCHAR(64)   NOT NULL CONSTRAINT DF_eqp_Assets_Status DEFAULT (N'InStock'),
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_eqp_Assets_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_eqp_Assets_CampusCode UNIQUE (CampusId, AssetCode)
    );
END
GO
EXEC dbo.sp_AddDescription N'eqp', N'Assets', NULL, N'设备资产台账。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'AssetCode', N'资产编码。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'Name', N'设备名称。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'Category', N'分类。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'Manufacturer', N'生产厂商。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'Model', N'型号。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'Status', N'资产状态。';
EXEC dbo.sp_AddDescription N'eqp', N'Assets', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'eqp.AssetMovements', N'U') IS NULL
BEGIN
    CREATE TABLE eqp.AssetMovements
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_eqp_AssetMovements PRIMARY KEY,
        AssetId      BIGINT         NOT NULL CONSTRAINT FK_eqp_Move_Assets FOREIGN KEY REFERENCES eqp.Assets (Id),
        MovementType NVARCHAR(64)   NOT NULL,
        FromDeptId   BIGINT         NULL CONSTRAINT FK_eqp_Move_FromDept FOREIGN KEY REFERENCES mdm.Departments (Id),
        ToDeptId     BIGINT         NULL CONSTRAINT FK_eqp_Move_ToDept FOREIGN KEY REFERENCES mdm.Departments (Id),
        OccurredAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_eqp_Move_OccurredAt DEFAULT (SYSUTCDATETIME()),
        Remark       NVARCHAR(500)  NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'eqp', N'AssetMovements', NULL, N'设备领用、借还、调拨记录。';
EXEC dbo.sp_AddDescription N'eqp', N'AssetMovements', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'eqp', N'AssetMovements', N'AssetId', N'设备主键。';
EXEC dbo.sp_AddDescription N'eqp', N'AssetMovements', N'MovementType', N'流转类型。';
EXEC dbo.sp_AddDescription N'eqp', N'AssetMovements', N'FromDeptId', N'来源科室。';
EXEC dbo.sp_AddDescription N'eqp', N'AssetMovements', N'ToDeptId', N'目标科室。';
EXEC dbo.sp_AddDescription N'eqp', N'AssetMovements', N'OccurredAt', N'发生时间（UTC）。';
EXEC dbo.sp_AddDescription N'eqp', N'AssetMovements', N'Remark', N'备注。';
GO

IF OBJECT_ID(N'eqp.InspectionTasks', N'U') IS NULL
BEGIN
    CREATE TABLE eqp.InspectionTasks
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_eqp_InspTasks PRIMARY KEY,
        AssetId      BIGINT         NOT NULL CONSTRAINT FK_eqp_Insp_Assets FOREIGN KEY REFERENCES eqp.Assets (Id),
        PlanDate     DATE           NOT NULL,
        Status       NVARCHAR(64)   NOT NULL CONSTRAINT DF_eqp_Insp_Status DEFAULT (N'Pending')
    );
END
GO
EXEC dbo.sp_AddDescription N'eqp', N'InspectionTasks', NULL, N'设备巡检计划任务。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionTasks', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionTasks', N'AssetId', N'设备主键。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionTasks', N'PlanDate', N'计划日期。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionTasks', N'Status', N'任务状态。';
GO

IF OBJECT_ID(N'eqp.InspectionResults', N'U') IS NULL
BEGIN
    CREATE TABLE eqp.InspectionResults
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_eqp_InspResults PRIMARY KEY,
        TaskId       BIGINT         NOT NULL CONSTRAINT FK_eqp_InspRes_Task FOREIGN KEY REFERENCES eqp.InspectionTasks (Id) ON DELETE CASCADE,
        Result       NVARCHAR(64)   NOT NULL,
        Notes        NVARCHAR(500)  NULL,
        InspectedAt  DATETIME2(3)   NOT NULL CONSTRAINT DF_eqp_InspRes_At DEFAULT (SYSUTCDATETIME()),
        InspectorId  BIGINT         NULL CONSTRAINT FK_eqp_InspRes_Staff FOREIGN KEY REFERENCES mdm.Staff (Id)
    );
END
GO
EXEC dbo.sp_AddDescription N'eqp', N'InspectionResults', NULL, N'设备巡检执行结果。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionResults', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionResults', N'TaskId', N'巡检任务主键。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionResults', N'Result', N'结果（正常/异常）。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionResults', N'Notes', N'说明。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionResults', N'InspectedAt', N'巡检时间（UTC）。';
EXEC dbo.sp_AddDescription N'eqp', N'InspectionResults', N'InspectorId', N'巡检人。';
GO

IF OBJECT_ID(N'eqp.WorkOrders', N'U') IS NULL
BEGIN
    CREATE TABLE eqp.WorkOrders
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_eqp_WorkOrders PRIMARY KEY,
        AssetId      BIGINT         NOT NULL CONSTRAINT FK_eqp_WO_Assets FOREIGN KEY REFERENCES eqp.Assets (Id),
        Title        NVARCHAR(200)  NOT NULL,
        Status       NVARCHAR(64)   NOT NULL CONSTRAINT DF_eqp_WO_Status DEFAULT (N'Open'),
        ReportedAt   DATETIME2(3)   NOT NULL CONSTRAINT DF_eqp_WO_ReportedAt DEFAULT (SYSUTCDATETIME()),
        CompletedAt  DATETIME2(3)   NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'eqp', N'WorkOrders', NULL, N'设备维修工单。';
EXEC dbo.sp_AddDescription N'eqp', N'WorkOrders', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'eqp', N'WorkOrders', N'AssetId', N'设备主键。';
EXEC dbo.sp_AddDescription N'eqp', N'WorkOrders', N'Title', N'工单标题/故障简述。';
EXEC dbo.sp_AddDescription N'eqp', N'WorkOrders', N'Status', N'工单状态。';
EXEC dbo.sp_AddDescription N'eqp', N'WorkOrders', N'ReportedAt', N'报修时间（UTC）。';
EXEC dbo.sp_AddDescription N'eqp', N'WorkOrders', N'CompletedAt', N'完工时间（UTC）。';
GO

IF OBJECT_ID(N'eqp.CalibrationRecords', N'U') IS NULL
BEGIN
    CREATE TABLE eqp.CalibrationRecords
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_eqp_Calib PRIMARY KEY,
        AssetId      BIGINT         NOT NULL CONSTRAINT FK_eqp_Calib_Assets FOREIGN KEY REFERENCES eqp.Assets (Id),
        CalibDate    DATE           NOT NULL,
        NextDueDate  DATE           NULL,
        CertificateRef NVARCHAR(500) NULL,
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_eqp_Calib_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'eqp', N'CalibrationRecords', NULL, N'计量/校准/强检记录。';
EXEC dbo.sp_AddDescription N'eqp', N'CalibrationRecords', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'eqp', N'CalibrationRecords', N'AssetId', N'设备主键。';
EXEC dbo.sp_AddDescription N'eqp', N'CalibrationRecords', N'CalibDate', N'检定/校准日期。';
EXEC dbo.sp_AddDescription N'eqp', N'CalibrationRecords', N'NextDueDate', N'下次到期日。';
EXEC dbo.sp_AddDescription N'eqp', N'CalibrationRecords', N'CertificateRef', N'证书附件引用。';
EXEC dbo.sp_AddDescription N'eqp', N'CalibrationRecords', N'CreatedAt', N'创建时间（UTC）。';
GO

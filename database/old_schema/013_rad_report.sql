/*
  013_rad_report.sql
  医技预约、到检、报告索引（影像类，关联 rad.ImagingOrders）。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'rad.Appointments', N'U') IS NULL
BEGIN
    CREATE TABLE rad.Appointments
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_rad_Appt PRIMARY KEY,
        ImagingOrderId   BIGINT         NOT NULL CONSTRAINT FK_rad_Appt_Img FOREIGN KEY REFERENCES rad.ImagingOrders (Id),
        Modality         NVARCHAR(64)   NULL,
        ScheduledAt      DATETIME2(3)   NOT NULL,
        Status           NVARCHAR(64)   NOT NULL CONSTRAINT DF_rad_Appt_Status DEFAULT (N'Scheduled')
    );
    CREATE INDEX IX_rad_Appt_Order ON rad.Appointments (ImagingOrderId);
END
GO
EXEC dbo.sp_AddDescription N'rad', N'Appointments', NULL, N'医技（影像）预约。';
EXEC dbo.sp_AddDescription N'rad', N'Appointments', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'rad', N'Appointments', N'ImagingOrderId', N'检查申请主键。';
EXEC dbo.sp_AddDescription N'rad', N'Appointments', N'Modality', N'模态（CT/MR/US 等）。';
EXEC dbo.sp_AddDescription N'rad', N'Appointments', N'ScheduledAt', N'预约检查时间（UTC）。';
EXEC dbo.sp_AddDescription N'rad', N'Appointments', N'Status', N'预约状态。';
GO

IF OBJECT_ID(N'rad.Registrations', N'U') IS NULL
BEGIN
    CREATE TABLE rad.Registrations
    (
        Id             BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_rad_Reg PRIMARY KEY,
        AppointmentId  BIGINT         NOT NULL CONSTRAINT FK_rad_Reg_Appt FOREIGN KEY REFERENCES rad.Appointments (Id),
        ArrivedAt      DATETIME2(3)   NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'rad', N'Registrations', NULL, N'到检登记。';
EXEC dbo.sp_AddDescription N'rad', N'Registrations', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'rad', N'Registrations', N'AppointmentId', N'预约主键。';
EXEC dbo.sp_AddDescription N'rad', N'Registrations', N'ArrivedAt', N'到检时间（UTC）。';
GO

IF OBJECT_ID(N'rad.Reports', N'U') IS NULL
BEGIN
    CREATE TABLE rad.Reports
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_rad_Reports PRIMARY KEY,
        ImagingOrderId   BIGINT         NOT NULL CONSTRAINT FK_rad_Rep_Img FOREIGN KEY REFERENCES rad.ImagingOrders (Id),
        ReportNo         NVARCHAR(64)   NULL,
        PdfUrl           NVARCHAR(1000) NULL,
        StorageKey       NVARCHAR(500)  NULL,
        ReleasedAt       DATETIME2(3)   NULL
    );
    CREATE INDEX IX_rad_Rep_Order ON rad.Reports (ImagingOrderId);
END
GO
EXEC dbo.sp_AddDescription N'rad', N'Reports', NULL, N'影像报告索引（文件本体可存对象存储）。';
EXEC dbo.sp_AddDescription N'rad', N'Reports', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'rad', N'Reports', N'ImagingOrderId', N'检查申请主键。';
EXEC dbo.sp_AddDescription N'rad', N'Reports', N'ReportNo', N'报告号。';
EXEC dbo.sp_AddDescription N'rad', N'Reports', N'PdfUrl', N'报告 PDF 直链（若使用）。';
EXEC dbo.sp_AddDescription N'rad', N'Reports', N'StorageKey', N'对象存储键。';
EXEC dbo.sp_AddDescription N'rad', N'Reports', N'ReleasedAt', N'报告发布时间（UTC）。';
GO

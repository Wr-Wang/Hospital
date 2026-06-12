/*
  015_rpt_meta.sql
  报表定义与导出任务（元数据层）。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'rpt.ReportDefinitions', N'U') IS NULL
BEGIN
    CREATE TABLE rpt.ReportDefinitions
    (
        Id                 BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_rpt_ReportDef PRIMARY KEY,
        Code               NVARCHAR(128)  NOT NULL,
        Name               NVARCHAR(256)  NOT NULL,
        ReportServerPath   NVARCHAR(500)  NULL,
        SqlText            NVARCHAR(MAX)  NULL,
        IsActive           BIT            NOT NULL CONSTRAINT DF_rpt_ReportDef_Active DEFAULT (1),
        CONSTRAINT UQ_rpt_ReportDef_Code UNIQUE (Code)
    );
END
GO
EXEC dbo.sp_AddDescription N'rpt', N'ReportDefinitions', NULL, N'报表定义（建议仅存路径，慎存可执行 SQL）。';
EXEC dbo.sp_AddDescription N'rpt', N'ReportDefinitions', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'rpt', N'ReportDefinitions', N'Code', N'报表编码。';
EXEC dbo.sp_AddDescription N'rpt', N'ReportDefinitions', N'Name', N'报表名称。';
EXEC dbo.sp_AddDescription N'rpt', N'ReportDefinitions', N'ReportServerPath', N'报表服务路径或文件路径。';
EXEC dbo.sp_AddDescription N'rpt', N'ReportDefinitions', N'SqlText', N'可选 SQL 文本（高风险，生产慎用）。';
EXEC dbo.sp_AddDescription N'rpt', N'ReportDefinitions', N'IsActive', N'是否启用。';
GO

IF OBJECT_ID(N'rpt.ExportJobs', N'U') IS NULL
BEGIN
    CREATE TABLE rpt.ExportJobs
    (
        Id                  BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_rpt_Export PRIMARY KEY,
        RequestedByUserId   BIGINT         NOT NULL CONSTRAINT FK_rpt_Export_User FOREIGN KEY REFERENCES sec.Users (Id),
        Status              NVARCHAR(64)   NOT NULL CONSTRAINT DF_rpt_Export_Status DEFAULT (N'Queued'),
        FileStorageKey      NVARCHAR(500)  NULL,
        CreatedAt           DATETIME2(3)   NOT NULL CONSTRAINT DF_rpt_Export_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CompletedAt         DATETIME2(3)   NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'rpt', N'ExportJobs', NULL, N'报表导出异步任务。';
EXEC dbo.sp_AddDescription N'rpt', N'ExportJobs', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'rpt', N'ExportJobs', N'RequestedByUserId', N'申请人用户主键。';
EXEC dbo.sp_AddDescription N'rpt', N'ExportJobs', N'Status', N'任务状态。';
EXEC dbo.sp_AddDescription N'rpt', N'ExportJobs', N'FileStorageKey', N'生成文件存储键。';
EXEC dbo.sp_AddDescription N'rpt', N'ExportJobs', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'rpt', N'ExportJobs', N'CompletedAt', N'完成时间（UTC）。';
GO

/*
  005_opd_schedule.sql
  排班模板与号源时段。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'opd.ScheduleTemplates', N'U') IS NULL
BEGIN
    CREATE TABLE opd.ScheduleTemplates
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_opd_ScheduleTemplates PRIMARY KEY,
        CampusId        BIGINT         NOT NULL CONSTRAINT FK_opd_ScheduleTemplates_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        DepartmentId    BIGINT         NOT NULL CONSTRAINT FK_opd_ScheduleTemplates_Departments FOREIGN KEY REFERENCES mdm.Departments (Id),
        EffectiveFrom   DATE           NOT NULL,
        EffectiveTo     DATE           NULL,
        Notes           NVARCHAR(500)  NULL,
        CreatedAt       DATETIME2(3)   NOT NULL CONSTRAINT DF_opd_ScheduleTemplates_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId BIGINT         NULL
    );
    CREATE INDEX IX_opd_ScheduleTemplates_CampusDept ON opd.ScheduleTemplates (CampusId, DepartmentId);
END
GO
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', NULL, N'排班/号源模板头。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', N'DepartmentId', N'科室。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', N'EffectiveFrom', N'生效开始日期。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', N'EffectiveTo', N'生效结束日期，空表示长期。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', N'Notes', N'备注。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', N'CreatedAt', N'创建时间（UTC）。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleTemplates', N'CreatedByUserId', N'创建人用户主键。';
GO

IF OBJECT_ID(N'opd.ScheduleSlots', N'U') IS NULL
BEGIN
    CREATE TABLE opd.ScheduleSlots
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_opd_ScheduleSlots PRIMARY KEY,
        TemplateId      BIGINT         NOT NULL CONSTRAINT FK_opd_ScheduleSlots_Templates FOREIGN KEY REFERENCES opd.ScheduleTemplates (Id),
        StaffId         BIGINT         NOT NULL CONSTRAINT FK_opd_ScheduleSlots_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        SlotDate        DATE           NOT NULL,
        StartTime       TIME(0)        NOT NULL,
        EndTime         TIME(0)        NOT NULL,
        TotalQuota      INT            NOT NULL CONSTRAINT DF_opd_ScheduleSlots_TotalQuota DEFAULT (0),
        BookedQuota     INT            NOT NULL CONSTRAINT DF_opd_ScheduleSlots_BookedQuota DEFAULT (0),
        SlotType        NVARCHAR(64)   NULL,
        IsStopped       BIT            NOT NULL CONSTRAINT DF_opd_ScheduleSlots_IsStopped DEFAULT (0),
        CreatedAt       DATETIME2(3)   NOT NULL CONSTRAINT DF_opd_ScheduleSlots_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE INDEX IX_opd_ScheduleSlots_StaffDate ON opd.ScheduleSlots (StaffId, SlotDate);
    CREATE INDEX IX_opd_ScheduleSlots_TemplateId ON opd.ScheduleSlots (TemplateId);
END
GO
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', NULL, N'号源时段（医生排班颗粒）。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'TemplateId', N'所属模板。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'StaffId', N'出诊医生/号源归属人员。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'SlotDate', N'出诊日期。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'StartTime', N'开始时间。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'EndTime', N'结束时间。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'TotalQuota', N'总号源数。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'BookedQuota', N'已预约/已挂号占用数。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'SlotType', N'号别（普通/专家等）。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'IsStopped', N'是否停诊。';
EXEC dbo.sp_AddDescription N'opd', N'ScheduleSlots', N'CreatedAt', N'创建时间（UTC）。';
GO

/*
  006_opd_registration.sql
  预约、挂号、分诊队列。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'opd.Appointments', N'U') IS NULL
BEGIN
    CREATE TABLE opd.Appointments
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_opd_Appointments PRIMARY KEY,
        CampusId     BIGINT         NOT NULL CONSTRAINT FK_opd_Appointments_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        PatientId    BIGINT         NOT NULL CONSTRAINT FK_opd_Appointments_Patients FOREIGN KEY REFERENCES pat.Patients (Id),
        SlotId       BIGINT         NOT NULL CONSTRAINT FK_opd_Appointments_Slots FOREIGN KEY REFERENCES opd.ScheduleSlots (Id),
        Channel      NVARCHAR(64)   NOT NULL,
        Status       NVARCHAR(64)   NOT NULL CONSTRAINT DF_opd_Appointments_Status DEFAULT (N'Booked'),
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_opd_Appointments_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE INDEX IX_opd_Appointments_PatientId ON opd.Appointments (PatientId);
END
GO
EXEC dbo.sp_AddDescription N'opd', N'Appointments', NULL, N'预约记录。';
EXEC dbo.sp_AddDescription N'opd', N'Appointments', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'opd', N'Appointments', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'opd', N'Appointments', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'opd', N'Appointments', N'SlotId', N'号源时段主键。';
EXEC dbo.sp_AddDescription N'opd', N'Appointments', N'Channel', N'预约渠道（院内/互联网/电话）。';
EXEC dbo.sp_AddDescription N'opd', N'Appointments', N'Status', N'预约状态。';
EXEC dbo.sp_AddDescription N'opd', N'Appointments', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'opd.Registrations', N'U') IS NULL
BEGIN
    CREATE TABLE opd.Registrations
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_opd_Registrations PRIMARY KEY,
        CampusId         BIGINT         NOT NULL CONSTRAINT FK_opd_Registrations_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        PatientId        BIGINT         NOT NULL CONSTRAINT FK_opd_Registrations_Patients FOREIGN KEY REFERENCES pat.Patients (Id),
        SlotId           BIGINT         NOT NULL CONSTRAINT FK_opd_Registrations_Slots FOREIGN KEY REFERENCES opd.ScheduleSlots (Id),
        AppointmentId    BIGINT         NULL CONSTRAINT FK_opd_Registrations_Appointments FOREIGN KEY REFERENCES opd.Appointments (Id),
        RegistrationNo   NVARCHAR(64)   NOT NULL,
        Status           NVARCHAR(64)   NOT NULL CONSTRAINT DF_opd_Registrations_Status DEFAULT (N'Registered'),
        FeeAmount        DECIMAL(18, 4) NOT NULL CONSTRAINT DF_opd_Registrations_FeeAmount DEFAULT (0),
        PaidAt           DATETIME2(3)   NULL,
        CreatedAt        DATETIME2(3)   NOT NULL CONSTRAINT DF_opd_Registrations_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_opd_Registrations_CampusRegNo UNIQUE (CampusId, RegistrationNo)
    );
    CREATE INDEX IX_opd_Registrations_PatientId ON opd.Registrations (PatientId);
END
GO
EXEC dbo.sp_AddDescription N'opd', N'Registrations', NULL, N'挂号订单。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'SlotId', N'号源时段主键。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'AppointmentId', N'来源预约，可空（窗口直接挂号）。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'RegistrationNo', N'挂号流水号，院区内唯一。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'Status', N'挂号状态（已挂/退号等）。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'FeeAmount', N'挂号费金额。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'PaidAt', N'缴费时间（UTC）。';
EXEC dbo.sp_AddDescription N'opd', N'Registrations', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'opd.TriageQueueEntries', N'U') IS NULL
BEGIN
    CREATE TABLE opd.TriageQueueEntries
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_opd_TriageQueueEntries PRIMARY KEY,
        CampusId         BIGINT         NOT NULL CONSTRAINT FK_opd_Triage_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        RegistrationId   BIGINT         NOT NULL CONSTRAINT FK_opd_Triage_Registrations FOREIGN KEY REFERENCES opd.Registrations (Id),
        Priority         INT            NOT NULL CONSTRAINT DF_opd_Triage_Priority DEFAULT (0),
        QueueNo          NVARCHAR(32)   NULL,
        CalledAt         DATETIME2(3)   NULL,
        RoomId           NVARCHAR(64)   NULL,
        CreatedAt        DATETIME2(3)   NOT NULL CONSTRAINT DF_opd_Triage_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE INDEX IX_opd_Triage_RegistrationId ON opd.TriageQueueEntries (RegistrationId);
END
GO
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', NULL, N'分诊队列入队记录。';
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', N'RegistrationId', N'挂号订单主键。';
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', N'Priority', N'优先级，数值越大越优先（急诊）。';
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', N'QueueNo', N'排队号。';
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', N'CalledAt', N'叫号时间（UTC）。';
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', N'RoomId', N'诊室/诊区标识。';
EXEC dbo.sp_AddDescription N'opd', N'TriageQueueEntries', N'CreatedAt', N'入队时间（UTC）。';
GO

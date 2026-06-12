DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-05-26', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-05-26', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-26', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-05-26', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-26', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-05-27', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-05-27', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-27', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-05-27', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-27', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-05-28', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-05-28', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-28', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-05-28', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-28', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-05-29', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-05-29', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-29', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-05-29', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-29', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-05-30', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-05-30', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-30', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-05-30', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-30', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-05-31', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-05-31', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-31', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-05-31', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-31', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-06-01', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-06-01', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-06-01', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-06-01', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-06-01', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO
SELECT COUNT(*) AS TotalTemplates FROM opd.ScheduleTemplates WHERE ScheduleDate > '2026-05-25';
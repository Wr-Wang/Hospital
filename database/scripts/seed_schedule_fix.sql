-- 补充缺失的排班数据

-- ===== 5月30日（周六） =====
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-05-30', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-05-30', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-30', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-05-30', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-30', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

-- ===== 5月31日（周日） =====
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (2, 2, 1, '2026-05-31', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (6, 2, 1, '2026-05-31', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-31', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (8, 3, 1, '2026-05-31', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-31', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

-- ===== 补充周磊(10) 内科(2) 5/26-5/29 =====
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-26', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-27', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-28', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (10, 2, 1, '2026-05-29', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

-- ===== 补充黄勇(9) 外科(3) 5/26-5/29 =====
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-26', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-27', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-28', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (9, 3, 1, '2026-05-29', N'已发布');
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

-- 验证
SELECT 'ScheduleTemplates' AS T, COUNT(*) AS C FROM opd.ScheduleTemplates
UNION ALL
SELECT 'ScheduleSlots', COUNT(*) FROM opd.ScheduleSlots;

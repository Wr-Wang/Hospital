-- 为未来7天所有医生创建排班数据
-- 医生: 张医生(2)内科, 刘洋(6)内科, 周磊(10)内科, 王强(8)外科, 黄勇(9)外科

DECLARE @i INT = 1
DECLARE @date DATE
DECLARE @today DATE = CAST(GETDATE() AS DATE)

WHILE @i <= 7
BEGIN
    SET @date = DATEADD(DAY, @i, @today)

    -- 内科 - 张医生
    INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
    VALUES (2, 2, 1, @date, N'已发布');
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

    -- 内科 - 刘洋
    INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
    VALUES (6, 2, 1, @date, N'已发布');
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

    -- 内科 - 周磊
    INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
    VALUES (10, 2, 1, @date, N'已发布');
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

    -- 外科 - 王强
    INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
    VALUES (8, 3, 1, @date, N'已发布');
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

    -- 外科 - 黄勇
    INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status)
    VALUES (9, 3, 1, @date, N'已发布');
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'上午', '08:00', '12:00', 30, 0, SCOPE_IDENTITY());
    INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId)
    VALUES (N'下午', '14:00', '17:00', 20, 0, SCOPE_IDENTITY());

    SET @i = @i + 1
END

SELECT COUNT(*) AS ScheduleCount FROM opd.ScheduleTemplates WHERE ScheduleDate > @today;

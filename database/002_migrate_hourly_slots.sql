/*
  数据库迁移/初始化脚本 v2
  清空历史挂号/排班数据 → 重建按小时时段 → 创建测试数据
  目标数据库：Hospital（SQL Server）

  执行：在 SSMS 中打开执行，或：
    sqlcmd -S . -d Hospital -U sa -P 123456 -i database/002_migrate_hourly_slots.sql
*/

PRINT N'========================================';
PRINT N' 开始迁移：按小时时段 + 初始化测试数据';
PRINT N'========================================';
GO

-- ===================================================================
-- 第一步：按外键顺序清空所有历史数据
-- ===================================================================
PRINT N'[1/5] 清空历史数据...';

DELETE FROM [enc].[EmrDocuments];
DELETE FROM [enc].[Diagnoses];
DELETE FROM [enc].[OutpatientEncounters];
DELETE FROM [opd].[Registrations];
DELETE FROM [opd].[ScheduleSlots];

PRINT N'  - 历史挂号、就诊、排班时段已清空';
GO

-- ===================================================================
-- 第二步：为每个排班模板重建按小时时段（保留已有模板）
-- ===================================================================
PRINT N'[2/5] 重建按小时时段...';

DECLARE @TmplId BIGINT;
DECLARE @SlotType NVARCHAR(64);
DECLARE @StartTime TIME(0);
DECLARE @EndTime TIME(0);
DECLARE @Quota INT = 5;

-- 7 个按小时时段的定义（跳过 12:00-14:00 午休）
DECLARE slot_cursor CURSOR LOCAL FOR
    SELECT Id FROM [opd].[ScheduleTemplates];

OPEN slot_cursor;
FETCH NEXT FROM slot_cursor INTO @TmplId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO [opd].[ScheduleSlots] ([TemplateId], [SlotType], [StartTime], [EndTime], [TotalQuota], [BookedQuota])
    VALUES
        (@TmplId, N'08:00-09:00', '08:00', '09:00', @Quota, 0),
        (@TmplId, N'09:00-10:00', '09:00', '10:00', @Quota, 0),
        (@TmplId, N'10:00-11:00', '10:00', '11:00', @Quota, 0),
        (@TmplId, N'11:00-12:00', '11:00', '12:00', @Quota, 0),
        (@TmplId, N'14:00-15:00', '14:00', '15:00', @Quota, 0),
        (@TmplId, N'15:00-16:00', '15:00', '16:00', @Quota, 0),
        (@TmplId, N'16:00-17:00', '16:00', '17:00', @Quota, 0);

    FETCH NEXT FROM slot_cursor INTO @TmplId;
END

CLOSE slot_cursor;
DEALLOCATE slot_cursor;

PRINT N'  - 所有排班时段已更新为按小时（共 7 个时段，每时段配额 5）';
GO

-- ===================================================================
-- 第三步：创建测试挂号数据（过去的已就诊 + 今天的待就诊）
-- ===================================================================
PRINT N'[3/5] 创建测试挂号数据...';

DECLARE @CurDate DATE = CAST(SYSDATETIME() AS DATE);
DECLARE @RegDate DATE;
DECLARE @Seq INT = 1;
DECLARE @SlotAM BIGINT, @SlotPM BIGINT;
DECLARE @PatId1 BIGINT, @PatId2 BIGINT, @PatId3 BIGINT, @PatId4 BIGINT;

-- 取前 4 个患者作为测试对象
SELECT TOP 1 @PatId1 = Id FROM [pat].[Patients] ORDER BY Id;
SELECT TOP 1 @PatId2 = Id FROM [pat].[Patients] ORDER BY Id OFFSET 1 ROWS;
SELECT TOP 1 @PatId3 = Id FROM [pat].[Patients] ORDER BY Id OFFSET 2 ROWS;
SELECT TOP 1 @PatId4 = Id FROM [pat].[Patients] ORDER BY Id OFFSET 3 ROWS;

IF @PatId1 IS NOT NULL
BEGIN
    DECLARE reg_cursor CURSOR LOCAL FOR
        SELECT DATEADD(DAY, -n, @CurDate) FROM (VALUES(0),(1),(2),(3),(4),(5),(6)) nums(n);

    OPEN reg_cursor;
    FETCH NEXT FROM reg_cursor INTO @RegDate;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF DATEPART(WEEKDAY, @RegDate) BETWEEN 2 AND 6  -- 工作日
        BEGIN
            -- 找当天第一个医生的 08:00-09:00 时段
            SELECT TOP 1 @SlotAM = ss.Id
            FROM [opd].[ScheduleSlots] ss
            INNER JOIN [opd].[ScheduleTemplates] st ON ss.TemplateId = st.Id
            WHERE st.ScheduleDate = @RegDate AND ss.SlotType = N'08:00-09:00';

            -- 找当天第二个医生的 14:00-15:00 时段
            SELECT TOP 1 @SlotPM = ss.Id
            FROM [opd].[ScheduleSlots] ss
            INNER JOIN [opd].[ScheduleTemplates] st ON ss.TemplateId = st.Id
            WHERE st.ScheduleDate = @RegDate AND ss.SlotType = N'14:00-15:00'
              AND st.DoctorId <> (SELECT TOP 1 DoctorId FROM [opd].[ScheduleTemplates] WHERE Id = (SELECT TOP 1 TemplateId FROM [opd].[ScheduleSlots] WHERE Id = @SlotAM));

            IF @RegDate < @CurDate
            BEGIN
                -- 过去日期 → 已就诊
                IF @SlotAM IS NOT NULL AND @PatId1 IS NOT NULL
                    INSERT INTO [opd].[Registrations] ([PatientId], [SlotId], [DoctorId], [DeptId], [CampusId], [CreatedAt], [QueueNo], [SlotName], [Status])
                    SELECT TOP 1 @PatId1, @SlotAM, st.DoctorId, st.DepartmentId, st.CampusId, DATEADD(HOUR, 8, CAST(@RegDate AS DATETIME2)), @Seq, N'08:00-09:00', N'已就诊'
                    FROM [opd].[ScheduleTemplates] st WHERE st.Id = (SELECT TemplateId FROM [opd].[ScheduleSlots] WHERE Id = @SlotAM);
                SET @Seq = @Seq + 1;

                IF @SlotPM IS NOT NULL AND @PatId2 IS NOT NULL
                    INSERT INTO [opd].[Registrations] ([PatientId], [SlotId], [DoctorId], [DeptId], [CampusId], [CreatedAt], [QueueNo], [SlotName], [Status])
                    SELECT TOP 1 @PatId2, @SlotPM, st.DoctorId, st.DepartmentId, st.CampusId, DATEADD(HOUR, 14, CAST(@RegDate AS DATETIME2)), @Seq, N'14:00-15:00', N'已就诊'
                    FROM [opd].[ScheduleTemplates] st WHERE st.Id = (SELECT TemplateId FROM [opd].[ScheduleSlots] WHERE Id = @SlotPM);
                SET @Seq = @Seq + 1;
            END
            ELSE
            BEGIN
                -- 今天 → 已挂号（待就诊）
                IF @SlotAM IS NOT NULL AND @PatId3 IS NOT NULL
                    INSERT INTO [opd].[Registrations] ([PatientId], [SlotId], [DoctorId], [DeptId], [CampusId], [CreatedAt], [QueueNo], [SlotName], [Status])
                    SELECT TOP 1 @PatId3, @SlotAM, st.DoctorId, st.DepartmentId, st.CampusId, SYSDATETIME(), @Seq, N'08:00-09:00', N'已挂号'
                    FROM [opd].[ScheduleTemplates] st WHERE st.Id = (SELECT TemplateId FROM [opd].[ScheduleSlots] WHERE Id = @SlotAM);
                SET @Seq = @Seq + 1;

                IF @SlotPM IS NOT NULL AND @PatId4 IS NOT NULL
                    INSERT INTO [opd].[Registrations] ([PatientId], [SlotId], [DoctorId], [DeptId], [CampusId], [CreatedAt], [QueueNo], [SlotName], [Status])
                    SELECT TOP 1 @PatId4, @SlotPM, st.DoctorId, st.DepartmentId, st.CampusId, SYSDATETIME(), @Seq, N'14:00-15:00', N'已挂号'
                    FROM [opd].[ScheduleTemplates] st WHERE st.Id = (SELECT TemplateId FROM [opd].[ScheduleSlots] WHERE Id = @SlotPM);
                SET @Seq = @Seq + 1;
            END
        END
        FETCH NEXT FROM reg_cursor INTO @RegDate;
    END

    CLOSE reg_cursor;
    DEALLOCATE reg_cursor;

    PRINT N'  - 测试挂号数据创建完成';
END
ELSE
    PRINT N'  - 跳过：未找到患者数据';
GO

-- ===================================================================
-- 第四步：创建就诊记录（关联已就诊的挂号）
-- ===================================================================
PRINT N'[4/5] 创建就诊记录...';

INSERT INTO [enc].[OutpatientEncounters] ([PatientId], [StaffId], [DepartmentId], [CampusId], [RegistrationId], [Status], [StartedAt], [EndedAt])
SELECT r.[PatientId], r.[DoctorId], r.[DeptId], r.[CampusId], r.[Id], N'已完成',
    DATEADD(MINUTE, 30, r.[CreatedAt]), DATEADD(MINUTE, 90, r.[CreatedAt])
FROM [opd].[Registrations] r
WHERE r.[Status] = N'已就诊'
  AND NOT EXISTS (SELECT 1 FROM [enc].[OutpatientEncounters] e WHERE e.[RegistrationId] = r.[Id]);

INSERT INTO [enc].[OutpatientEncounters] ([PatientId], [StaffId], [DepartmentId], [CampusId], [RegistrationId], [Status], [StartedAt])
SELECT r.[PatientId], r.[DoctorId], r.[DeptId], r.[CampusId], r.[Id], N'就诊中', SYSDATETIME()
FROM [opd].[Registrations] r
WHERE r.[Status] = N'已挂号'
  AND NOT EXISTS (SELECT 1 FROM [enc].[OutpatientEncounters] e WHERE e.[RegistrationId] = r.[Id]);

PRINT N'  - 就诊记录创建完成';
GO

-- ===================================================================
-- 第五步：更新排班时段的已预约数（BookedQuota）
-- ===================================================================
PRINT N'[5/5] 同步时段已预约数...';

UPDATE ss
SET ss.[BookedQuota] = r.cnt
FROM [opd].[ScheduleSlots] ss
INNER JOIN (
    SELECT [SlotId], COUNT(*) AS cnt
    FROM [opd].[Registrations]
    WHERE [Status] IN (N'已挂号', N'已就诊')
    GROUP BY [SlotId]
) r ON ss.[Id] = r.[SlotId];

PRINT N'  - 时段已预约数已同步';
GO

-- ===================================================================
-- 完成
-- ===================================================================
PRINT N'';
PRINT N'========================================';
PRINT N' 迁移完成！';
PRINT N'';
PRINT N' 时段划分：';
PRINT N'   上午 08:00-09:00 | 09:00-10:00 | 10:00-11:00 | 11:00-12:00';
PRINT N'   （午休 12:00-14:00）';
PRINT N'   下午 14:00-15:00 | 15:00-16:00 | 16:00-17:00';
PRINT N'';
PRINT N' 每时段配额：5 号';
PRINT N'========================================';
GO

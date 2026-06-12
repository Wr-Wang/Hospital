/*
  999_verify_seed_data.sql
  验证种子数据行数，检查 27 张 EF Core 表是否都有 10+ 行数据。
  执行方法：在 SSMS 中运行，查看 Results 选项卡。
*/
USE [Hospital];
GO

SET NOCOUNT ON;

DECLARE @MinRows INT = 10;

-- 豁免表（业务特性决定数据量较少为正常）
DECLARE @ExemptTables TABLE (SchemaName NVARCHAR(128), TableName NVARCHAR(128));
INSERT INTO @ExemptTables VALUES
    (N'sec', N'AuditLogs'),          -- 审计日志：随系统使用增加
    (N'pha', N'Dispenses'),          -- 发药：依赖处方
    (N'pha', N'DispenseLines'),      -- 发药明细：依赖发药
    (N'fin', N'Invoices'),           -- 发票：依赖挂号收费
    (N'fin', N'ChargeLines'),        -- 收费明细：依赖发票
    (N'fin', N'Payments'),           -- 支付记录：依赖发票
    (N'enc', N'OutpatientEncounters'), -- 就诊记录：依赖挂号
    (N'enc', N'Diagnoses'),          -- 诊断：依赖就诊
    (N'enc', N'EmrDocuments'),       -- 病历：依赖就诊
    (N'pha', N'Prescriptions'),      -- 处方：依赖就诊
    (N'pha', N'PrescriptionLines'),  -- 处方明细：依赖处方
    (N'lab', N'LabOrders'),          -- 检验：依赖就诊
    (N'rad', N'ImagingOrders'),      -- 检查：依赖就诊
    (N'opd', N'Registrations'),      -- 挂号：依赖排班
    (N'opd', N'ScheduleTemplates'),  -- 排班模板：数量取决于排班周期
    (N'opd', N'ScheduleSlots'),      -- 排班时段：数量取决于排班模板
    (N'pat', N'PatientIdentifiers'), -- 患者标识：依赖患者
    (N'pat', N'PatientConsents');    -- 知情同意：依赖患者

SELECT
    SCHEMA_NAME(t.schema_id) AS SchemaName,
    t.name AS TableName,
    p.rows AS RowCount,
    CASE WHEN p.rows >= @MinRows THEN N'✅' ELSE N'❌' END AS Status,
    CASE
        WHEN p.rows >= @MinRows THEN N'通过'
        WHEN et.SchemaName IS NOT NULL THEN N'豁免（少量为正常）'
        ELSE N'未达标（< ' + CAST(@MinRows AS NVARCHAR) + N' 行）'
    END AS Remarks
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id AND i.index_id <= 1
CROSS APPLY (
    SELECT SUM(p.rows) AS rows
    FROM sys.partitions p
    WHERE p.object_id = t.object_id AND p.index_id = i.index_id
) p
LEFT JOIN @ExemptTables et ON SCHEMA_NAME(t.schema_id) = et.SchemaName AND t.name = et.TableName
WHERE t.is_ms_shipped = 0
ORDER BY SCHEMA_NAME(t.schema_id), t.name;

DECLARE @TotalTables INT, @Passed INT, @Failed INT, @Exempt INT;

SELECT
    @TotalTables = COUNT(*),
    @Passed = SUM(CASE WHEN p.rows >= @MinRows THEN 1 ELSE 0 END),
    @Failed = SUM(CASE WHEN p.rows < @MinRows AND et.SchemaName IS NULL THEN 1 ELSE 0 END),
    @Exempt = SUM(CASE WHEN et.SchemaName IS NOT NULL THEN 1 ELSE 0 END)
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id AND i.index_id <= 1
CROSS APPLY (
    SELECT SUM(p.rows) AS rows
    FROM sys.partitions p
    WHERE p.object_id = t.object_id AND p.index_id = i.index_id
) p
LEFT JOIN @ExemptTables et ON SCHEMA_NAME(t.schema_id) = et.SchemaName AND t.name = et.TableName
WHERE t.is_ms_shipped = 0;

PRINT N'';
PRINT N'========================================';
PRINT N'  种子数据验证汇总';
PRINT N'========================================';
PRINT N'  总表数:          ' + CAST(@TotalTables AS NVARCHAR);
PRINT N'  达标 (>= 10行):  ' + CAST(@Passed AS NVARCHAR);
PRINT N'  未达标:          ' + CAST(@Failed AS NVARCHAR);
PRINT N'  豁免:            ' + CAST(@Exempt AS NVARCHAR);
PRINT N'========================================';

IF @Failed > 0
    PRINT N'  ⚠ 存在未达标表，请检查上方 ❌ 标记的表。';
ELSE
    PRINT N'  ✅ 所有表均达到 10 行以上数据！';
GO

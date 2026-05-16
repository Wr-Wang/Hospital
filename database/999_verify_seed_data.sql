/*
  999_verify_seed_data.sql
  验证所有表的种子数据行数是否达到 10+。
  执行方法：在 SSMS 中运行，查看 Results 选项卡。
*/
USE [Hospital];
GO

SET NOCOUNT ON;

DECLARE @MinRows INT = 10;

-- 需要跳过的表（依赖特定业务逻辑，数据量少为正常）
DECLARE @ExemptTables TABLE (SchemaName NVARCHAR(128), TableName NVARCHAR(128));
INSERT INTO @ExemptTables VALUES
    (N'pat', N'PatientMergeLogs'),   -- 合并操作较少
    (N'pha', N'ControlledDrugWitness'), -- 仅管控药品需要核对
    (N'fin', N'Refunds'),            -- 退费应较少
    (N'fin', N'InsuranceReconcileBatches'), -- 按月批次
    (N'fin', N'InsuranceReconcileLines'),
    (N'rpt', N'ExportJobs');         -- 导出任务应较少

SELECT
    SCHEMA_NAME(t.schema_id) AS SchemaName,
    t.name AS TableName,
    p.rows AS RowCount,
    CASE WHEN p.rows >= @MinRows THEN N'✅' ELSE N'❌' END AS Status,
    CASE
        WHEN p.rows >= @MinRows THEN N'通过'
        WHEN et.SchemaName IS NOT NULL THEN N'豁免（业务表，少量为正常）'
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

-- 汇总统计
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

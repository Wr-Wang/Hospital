/*
  000_init_database.sql
  创建数据库、架构及扩展属性辅助存储过程（字段/表说明）。
  目标：SQL Server 2019+，排序规则 Chinese_PRC_CI_AS。
  执行前请按需修改数据库名称 @DbName。
*/
SET NOCOUNT ON;
GO

DECLARE @DbName sysname = N'Hospital';
DECLARE @sql nvarchar(max);

IF DB_ID(@DbName) IS NULL
BEGIN
    SET @sql = N'CREATE DATABASE ' + QUOTENAME(@DbName) + N' COLLATE Chinese_PRC_CI_AS;';
    EXEC (@sql);
END
GO

USE [Hospital];
GO

/* 业务架构 */
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'mdm') EXEC (N'CREATE SCHEMA mdm');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'sec') EXEC (N'CREATE SCHEMA sec'); -- 安全与审计（避免与系统架构 sys 冲突）
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'pat') EXEC (N'CREATE SCHEMA pat');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'opd') EXEC (N'CREATE SCHEMA opd');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'enc') EXEC (N'CREATE SCHEMA enc');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'lab') EXEC (N'CREATE SCHEMA lab');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'rad') EXEC (N'CREATE SCHEMA rad');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'pha') EXEC (N'CREATE SCHEMA pha');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'ipd') EXEC (N'CREATE SCHEMA ipd');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'mon') EXEC (N'CREATE SCHEMA mon');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'eqp') EXEC (N'CREATE SCHEMA eqp');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'fin') EXEC (N'CREATE SCHEMA fin');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'rpt') EXEC (N'CREATE SCHEMA rpt');
GO

/* 若已存在同名 dbo 存储过程则替换 */
CREATE OR ALTER PROCEDURE dbo.sp_AddDescription
    @schema   sysname,
    @object   sysname,
    @column   sysname = NULL, -- NULL 表示对象级（表）说明
    @description nvarchar(4000)
AS
BEGIN
    SET NOCOUNT ON;
    IF @column IS NULL
    BEGIN
        IF EXISTS (
            SELECT 1 FROM sys.extended_properties ep
            INNER JOIN sys.tables t ON ep.major_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE ep.minor_id = 0 AND ep.name = N'MS_Description'
              AND s.name = @schema AND t.name = @object
        )
            EXEC sys.sp_updateextendedproperty
                @name = N'MS_Description', @value = @description,
                @level0type = N'SCHEMA', @level0name = @schema,
                @level1type = N'TABLE',  @level1name = @object;
        ELSE
            EXEC sys.sp_addextendedproperty
                @name = N'MS_Description', @value = @description,
                @level0type = N'SCHEMA', @level0name = @schema,
                @level1type = N'TABLE',  @level1name = @object;
    END
    ELSE
    BEGIN
        IF EXISTS (
            SELECT 1 FROM sys.extended_properties ep
            INNER JOIN sys.columns c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
            INNER JOIN sys.tables t ON c.object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE ep.name = N'MS_Description'
              AND s.name = @schema AND t.name = @object AND c.name = @column
        )
            EXEC sys.sp_updateextendedproperty
                @name = N'MS_Description', @value = @description,
                @level0type = N'SCHEMA', @level0name = @schema,
                @level1type = N'TABLE',  @level1name = @object,
                @level2type = N'COLUMN', @level2name = @column;
        ELSE
            EXEC sys.sp_addextendedproperty
                @name = N'MS_Description', @value = @description,
                @level0type = N'SCHEMA', @level0name = @schema,
                @level1type = N'TABLE',  @level1name = @object,
                @level2type = N'COLUMN', @level2name = @column;
    END
END
GO

/* 说明：应用安全表使用架构 sec，勿使用 sys（系统保留）。 */

/*
  010_wechat_login.sql
  微信登录功能：新增患者微信账号关联表和 refresh_token 表。
*/
SET NOCOUNT ON;
GO
USE [Hospital];
GO

-- ===== 微信账号关联表 =====
IF OBJECT_ID(N'sec.WeChatAccounts', N'U') IS NULL
BEGIN
    CREATE TABLE [sec].[WeChatAccounts] (
        [Id]           bigint NOT NULL IDENTITY,
        [OpenId]       nvarchar(128) NOT NULL,
        [UnionId]      nvarchar(128) NULL,
        [PatientId]    bigint NOT NULL,
        [NickName]     nvarchar(100) NULL,
        [AvatarUrl]    nvarchar(500) NULL,
        [Phone]        nvarchar(32) NULL,
        [CreatedAt]    datetime2 NOT NULL DEFAULT GETDATE(),
        [LastLoginAt]  datetime2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_WeChatAccounts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WeChatAccounts_Patients_PatientId]
            FOREIGN KEY ([PatientId]) REFERENCES [pat].[Patients]([Id])
    );
    CREATE UNIQUE INDEX [IX_WeChatAccounts_OpenId] ON [sec].[WeChatAccounts] ([OpenId]);
END
GO

-- ===== 患者 refresh_token 表 =====
IF OBJECT_ID(N'sec.PatientRefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE [sec].[PatientRefreshTokens] (
        [Id]           bigint NOT NULL IDENTITY,
        [PatientId]    bigint NOT NULL,
        [Token]        nvarchar(500) NOT NULL,
        [ExpiresAt]    datetime2 NOT NULL,
        [CreatedAt]    datetime2 NOT NULL DEFAULT GETDATE(),
        [RevokedAt]    datetime2 NULL,
        CONSTRAINT [PK_PatientRefreshTokens] PRIMARY KEY ([Id])
    );
    CREATE INDEX [IX_PatientRefreshTokens_PatientId] ON [sec].[PatientRefreshTokens] ([PatientId]);
END
GO

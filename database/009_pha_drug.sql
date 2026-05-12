/*
  009_pha_drug.sql
  药品主数据、仓储、库存流水、发药与管控药双人核对。
  依赖：008（处方明细引用 DrugId）。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'pha.Drugs', N'U') IS NULL
BEGIN
    CREATE TABLE pha.Drugs
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_Drugs PRIMARY KEY,
        CampusId     BIGINT         NOT NULL CONSTRAINT FK_pha_Drugs_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        Code         NVARCHAR(64)   NOT NULL,
        Name         NVARCHAR(256)  NOT NULL,
        Spec         NVARCHAR(128)  NULL,
        Unit         NVARCHAR(32)   NULL,
        IsControlled BIT            NOT NULL CONSTRAINT DF_pha_Drugs_IsControlled DEFAULT (0),
        IsActive     BIT            NOT NULL CONSTRAINT DF_pha_Drugs_IsActive DEFAULT (1),
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_pha_Drugs_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_pha_Drugs_CampusCode UNIQUE (CampusId, Code)
    );
END
GO
EXEC dbo.sp_AddDescription N'pha', N'Drugs', NULL, N'药品主数据。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'CampusId', N'院区（目录可院区化）。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'Code', N'药品编码。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'Name', N'药品名称。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'Spec', N'规格。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'Unit', N'单位。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'IsControlled', N'是否管控药品。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'IsActive', N'是否启用。';
EXEC dbo.sp_AddDescription N'pha', N'Drugs', N'CreatedAt', N'创建时间（UTC）。';
GO

/* 处方明细 -> 药品 外键 */
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_pha_PrescriptionLines_Drugs')
BEGIN
    ALTER TABLE pha.PrescriptionLines WITH CHECK
    ADD CONSTRAINT FK_pha_PrescriptionLines_Drugs FOREIGN KEY (DrugId) REFERENCES pha.Drugs (Id);
END
GO

IF OBJECT_ID(N'pha.StorageLocations', N'U') IS NULL
BEGIN
    CREATE TABLE pha.StorageLocations
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_StorageLocations PRIMARY KEY,
        CampusId     BIGINT         NOT NULL CONSTRAINT FK_pha_Storage_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        Name         NVARCHAR(200)  NOT NULL,
        LocationType NVARCHAR(64)   NOT NULL,
        IsActive     BIT            NOT NULL CONSTRAINT DF_pha_Storage_IsActive DEFAULT (1)
    );
END
GO
EXEC dbo.sp_AddDescription N'pha', N'StorageLocations', NULL, N'药库/药房/科室库位。';
EXEC dbo.sp_AddDescription N'pha', N'StorageLocations', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'StorageLocations', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'pha', N'StorageLocations', N'Name', N'库位名称。';
EXEC dbo.sp_AddDescription N'pha', N'StorageLocations', N'LocationType', N'库位类型（中心药库/门诊药房/病区基数药等）。';
EXEC dbo.sp_AddDescription N'pha', N'StorageLocations', N'IsActive', N'是否启用。';
GO

IF OBJECT_ID(N'pha.DrugBatches', N'U') IS NULL
BEGIN
    CREATE TABLE pha.DrugBatches
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_DrugBatches PRIMARY KEY,
        DrugId       BIGINT         NOT NULL CONSTRAINT FK_pha_Batches_Drugs FOREIGN KEY REFERENCES pha.Drugs (Id),
        CampusId     BIGINT         NOT NULL CONSTRAINT FK_pha_Batches_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        BatchNo      NVARCHAR(64)   NOT NULL,
        ExpiryDate   DATE           NULL,
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_pha_Batches_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_pha_DrugBatches UNIQUE (DrugId, CampusId, BatchNo)
    );
END
GO
EXEC dbo.sp_AddDescription N'pha', N'DrugBatches', NULL, N'药品批号与效期。';
EXEC dbo.sp_AddDescription N'pha', N'DrugBatches', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'DrugBatches', N'DrugId', N'药品主键。';
EXEC dbo.sp_AddDescription N'pha', N'DrugBatches', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'pha', N'DrugBatches', N'BatchNo', N'生产批号。';
EXEC dbo.sp_AddDescription N'pha', N'DrugBatches', N'ExpiryDate', N'效期至。';
EXEC dbo.sp_AddDescription N'pha', N'DrugBatches', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'pha.InventoryLots', N'U') IS NULL
BEGIN
    CREATE TABLE pha.InventoryLots
    (
        Id                 BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_InventoryLots PRIMARY KEY,
        DrugBatchId        BIGINT         NOT NULL CONSTRAINT FK_pha_Lots_Batches FOREIGN KEY REFERENCES pha.DrugBatches (Id),
        StorageLocationId  BIGINT         NOT NULL CONSTRAINT FK_pha_Lots_Storage FOREIGN KEY REFERENCES pha.StorageLocations (Id),
        QtyOnHand          DECIMAL(18, 4) NOT NULL CONSTRAINT DF_pha_Lots_Qty DEFAULT (0),
        UpdatedAt          DATETIME2(3)   NOT NULL CONSTRAINT DF_pha_Lots_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_pha_InventoryLots UNIQUE (DrugBatchId, StorageLocationId)
    );
END
GO
EXEC dbo.sp_AddDescription N'pha', N'InventoryLots', NULL, N'库存 lot（批号 + 库位）。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryLots', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryLots', N'DrugBatchId', N'药品批号主键。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryLots', N'StorageLocationId', N'库位主键。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryLots', N'QtyOnHand', N'当前在手数量。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryLots', N'UpdatedAt', N'最后更新时间（UTC）。';
GO

IF OBJECT_ID(N'pha.InventoryTransactions', N'U') IS NULL
BEGIN
    CREATE TABLE pha.InventoryTransactions
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_InvTxn PRIMARY KEY,
        InventoryLotId BIGINT         NOT NULL CONSTRAINT FK_pha_InvTxn_Lots FOREIGN KEY REFERENCES pha.InventoryLots (Id),
        TxnType         NVARCHAR(32)   NOT NULL,
        Qty             DECIMAL(18, 4) NOT NULL,
        RefDocNo        NVARCHAR(128)  NULL,
        OccurredAt      DATETIME2(3)   NOT NULL CONSTRAINT DF_pha_InvTxn_OccurredAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId BIGINT         NULL CONSTRAINT FK_pha_InvTxn_User FOREIGN KEY REFERENCES sec.Users (Id)
    );
    CREATE INDEX IX_pha_InvTxn_Lot ON pha.InventoryTransactions (InventoryLotId);
END
GO
EXEC dbo.sp_AddDescription N'pha', N'InventoryTransactions', NULL, N'入出库流水。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryTransactions', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryTransactions', N'InventoryLotId', N'库存 lot 主键。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryTransactions', N'TxnType', N'事务类型（In/Out/Adjust 等）。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryTransactions', N'Qty', N'数量（正入负出）。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryTransactions', N'RefDocNo', N'关联业务单号。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryTransactions', N'OccurredAt', N'发生时间（UTC）。';
EXEC dbo.sp_AddDescription N'pha', N'InventoryTransactions', N'CreatedByUserId', N'操作人用户主键。';
GO

IF OBJECT_ID(N'pha.Dispenses', N'U') IS NULL
BEGIN
    CREATE TABLE pha.Dispenses
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_Dispenses PRIMARY KEY,
        CampusId         BIGINT         NOT NULL CONSTRAINT FK_pha_Disp_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        PrescriptionId   BIGINT         NOT NULL CONSTRAINT FK_pha_Disp_Rx FOREIGN KEY REFERENCES pha.Prescriptions (Id),
        Status           NVARCHAR(64)   NOT NULL CONSTRAINT DF_pha_Disp_Status DEFAULT (N'Pending'),
        CreatedAt        DATETIME2(3)   NOT NULL CONSTRAINT DF_pha_Disp_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE INDEX IX_pha_Disp_Rx ON pha.Dispenses (PrescriptionId);
END
GO
EXEC dbo.sp_AddDescription N'pha', N'Dispenses', NULL, N'发药单。';
EXEC dbo.sp_AddDescription N'pha', N'Dispenses', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'Dispenses', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'pha', N'Dispenses', N'PrescriptionId', N'处方主键。';
EXEC dbo.sp_AddDescription N'pha', N'Dispenses', N'Status', N'发药状态。';
EXEC dbo.sp_AddDescription N'pha', N'Dispenses', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'pha.DispenseLines', N'U') IS NULL
BEGIN
    CREATE TABLE pha.DispenseLines
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_DispenseLines PRIMARY KEY,
        DispenseId       BIGINT         NOT NULL CONSTRAINT FK_pha_DispLines_Disp FOREIGN KEY REFERENCES pha.Dispenses (Id) ON DELETE CASCADE,
        PrescriptionLineId BIGINT       NULL,
        InventoryLotId   BIGINT         NOT NULL CONSTRAINT FK_pha_DispLines_Lots FOREIGN KEY REFERENCES pha.InventoryLots (Id),
        Qty              DECIMAL(18, 4) NOT NULL
    );
    CREATE INDEX IX_pha_DispLines_Disp ON pha.DispenseLines (DispenseId);
END
GO
EXEC dbo.sp_AddDescription N'pha', N'DispenseLines', NULL, N'发药明细。';
EXEC dbo.sp_AddDescription N'pha', N'DispenseLines', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'DispenseLines', N'DispenseId', N'发药单主键。';
EXEC dbo.sp_AddDescription N'pha', N'DispenseLines', N'PrescriptionLineId', N'处方明细主键（外键可在院规确定后添加）。';
EXEC dbo.sp_AddDescription N'pha', N'DispenseLines', N'InventoryLotId', N'扣减的库存 lot。';
EXEC dbo.sp_AddDescription N'pha', N'DispenseLines', N'Qty', N'实发数量。';
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_pha_DispenseLines_PrescriptionLines')
BEGIN
    ALTER TABLE pha.DispenseLines WITH CHECK
    ADD CONSTRAINT FK_pha_DispenseLines_PrescriptionLines FOREIGN KEY (PrescriptionLineId) REFERENCES pha.PrescriptionLines (Id);
END
GO

IF OBJECT_ID(N'pha.ControlledDrugWitness', N'U') IS NULL
BEGIN
    CREATE TABLE pha.ControlledDrugWitness
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_CtrlWitness PRIMARY KEY,
        DispenseLineId   BIGINT         NOT NULL CONSTRAINT FK_pha_CtrlWitness_Line FOREIGN KEY REFERENCES pha.DispenseLines (Id) ON DELETE CASCADE,
        WitnessUserId    BIGINT         NOT NULL CONSTRAINT FK_pha_CtrlWitness_User FOREIGN KEY REFERENCES sec.Users (Id),
        WitnessedAt      DATETIME2(3)   NOT NULL CONSTRAINT DF_pha_CtrlWitness_At DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'pha', N'ControlledDrugWitness', NULL, N'管控药品双人核对记录。';
EXEC dbo.sp_AddDescription N'pha', N'ControlledDrugWitness', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'ControlledDrugWitness', N'DispenseLineId', N'发药明细主键。';
EXEC dbo.sp_AddDescription N'pha', N'ControlledDrugWitness', N'WitnessUserId', N'第二核对人用户主键。';
EXEC dbo.sp_AddDescription N'pha', N'ControlledDrugWitness', N'WitnessedAt', N'核对时间（UTC）。';
GO

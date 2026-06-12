/*
  014_fin_billing.sql
  费用、结算、支付、退费、发票桥、医保与对账。
*/
USE [Hospital];
GO

IF OBJECT_ID(N'fin.Invoices', N'U') IS NULL
BEGIN
    CREATE TABLE fin.Invoices
    (
        Id              BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_Invoices PRIMARY KEY,
        CampusId        BIGINT         NOT NULL CONSTRAINT FK_fin_Inv_Campus FOREIGN KEY REFERENCES mdm.Campuses (Id),
        PayerPatientId  BIGINT         NOT NULL CONSTRAINT FK_fin_Inv_Patient FOREIGN KEY REFERENCES pat.Patients (Id),
        TotalAmount     DECIMAL(18, 4) NOT NULL CONSTRAINT DF_fin_Inv_Total DEFAULT (0),
        SettledAt       DATETIME2(3)   NULL,
        Status          NVARCHAR(64)   NOT NULL CONSTRAINT DF_fin_Inv_Status DEFAULT (N'Open')
    );
END
GO
EXEC dbo.sp_AddDescription N'fin', N'Invoices', NULL, N'结算单头。';
EXEC dbo.sp_AddDescription N'fin', N'Invoices', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'Invoices', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'fin', N'Invoices', N'PayerPatientId', N'付款患者。';
EXEC dbo.sp_AddDescription N'fin', N'Invoices', N'TotalAmount', N'总金额。';
EXEC dbo.sp_AddDescription N'fin', N'Invoices', N'SettledAt', N'结算完成时间（UTC）。';
EXEC dbo.sp_AddDescription N'fin', N'Invoices', N'Status', N'结算状态。';
GO

IF OBJECT_ID(N'fin.ChargeLines', N'U') IS NULL
BEGIN
    CREATE TABLE fin.ChargeLines
    (
        Id                        BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_ChargeLines PRIMARY KEY,
        CampusId                  BIGINT         NOT NULL CONSTRAINT FK_fin_CL_Campus FOREIGN KEY REFERENCES mdm.Campuses (Id),
        InvoiceId                 BIGINT         NULL CONSTRAINT FK_fin_CL_Invoice FOREIGN KEY REFERENCES fin.Invoices (Id),
        OutpatientEncounterId     BIGINT         NULL CONSTRAINT FK_fin_CL_OPEC FOREIGN KEY REFERENCES enc.OutpatientEncounters (Id),
        EmergencyEncounterId      BIGINT         NULL CONSTRAINT FK_fin_CL_EME FOREIGN KEY REFERENCES enc.EmergencyEncounters (Id),
        AdmissionId               BIGINT         NULL CONSTRAINT FK_fin_CL_Adm FOREIGN KEY REFERENCES ipd.Admissions (Id),
        LabOrderId                BIGINT         NULL CONSTRAINT FK_fin_CL_Lab FOREIGN KEY REFERENCES lab.LabOrders (Id),
        ImagingOrderId            BIGINT         NULL CONSTRAINT FK_fin_CL_Img FOREIGN KEY REFERENCES rad.ImagingOrders (Id),
        PrescriptionLineId        BIGINT         NULL CONSTRAINT FK_fin_CL_RxLine FOREIGN KEY REFERENCES pha.PrescriptionLines (Id),
        InpatientOrderId          BIGINT         NULL CONSTRAINT FK_fin_CL_IO FOREIGN KEY REFERENCES ipd.InpatientOrders (Id),
        ChargeItemId              BIGINT         NULL CONSTRAINT FK_fin_CL_Charge FOREIGN KEY REFERENCES mdm.ChargeItems (Id),
        Qty                       DECIMAL(18, 4) NOT NULL CONSTRAINT DF_fin_CL_Qty DEFAULT (1),
        Amount                    DECIMAL(18, 4) NOT NULL,
        Discount                  DECIMAL(18, 4) NOT NULL CONSTRAINT DF_fin_CL_Disc DEFAULT (0),
        Status                    NVARCHAR(64)   NOT NULL CONSTRAINT DF_fin_CL_Status DEFAULT (N'Posted')
    );
    CREATE INDEX IX_fin_CL_Invoice ON fin.ChargeLines (InvoiceId);
END
GO
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', NULL, N'费用明细；门/急/住上下文至多一项非空，来源子表外键可并存多项但需计费引擎防重。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'InvoiceId', N'所属结算单，可空（未结账）。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'OutpatientEncounterId', N'门诊就诊上下文。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'EmergencyEncounterId', N'急诊就诊上下文。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'AdmissionId', N'住院上下文。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'LabOrderId', N'来源检验申请。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'ImagingOrderId', N'来源检查申请。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'PrescriptionLineId', N'来源处方明细。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'InpatientOrderId', N'来源住院医嘱。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'ChargeItemId', N'收费项目。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'Qty', N'数量。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'Amount', N'金额。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'Discount', N'折扣金额。';
EXEC dbo.sp_AddDescription N'fin', N'ChargeLines', N'Status', N'行状态（暂存/已过账等）。';
GO

IF OBJECT_ID(N'fin.Payments', N'U') IS NULL
BEGIN
    CREATE TABLE fin.Payments
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_Payments PRIMARY KEY,
        InvoiceId        BIGINT         NOT NULL CONSTRAINT FK_fin_Pay_Inv FOREIGN KEY REFERENCES fin.Invoices (Id),
        PayMethod          NVARCHAR(64)   NOT NULL,
        Amount           DECIMAL(18, 4) NOT NULL,
        TransactionRef   NVARCHAR(200)  NULL,
        PaidAt           DATETIME2(3)   NOT NULL CONSTRAINT DF_fin_Pay_PaidAt DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'fin', N'Payments', NULL, N'支付流水。';
EXEC dbo.sp_AddDescription N'fin', N'Payments', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'Payments', N'InvoiceId', N'结算单主键。';
EXEC dbo.sp_AddDescription N'fin', N'Payments', N'PayMethod', N'支付方式（现金/微信/医保等）。';
EXEC dbo.sp_AddDescription N'fin', N'Payments', N'Amount', N'支付金额。';
EXEC dbo.sp_AddDescription N'fin', N'Payments', N'TransactionRef', N'第三方交易号。';
EXEC dbo.sp_AddDescription N'fin', N'Payments', N'PaidAt', N'支付时间（UTC）。';
GO

IF OBJECT_ID(N'fin.Refunds', N'U') IS NULL
BEGIN
    CREATE TABLE fin.Refunds
    (
        Id                  BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_Refunds PRIMARY KEY,
        OriginalPaymentId   BIGINT         NOT NULL CONSTRAINT FK_fin_Ref_Pay FOREIGN KEY REFERENCES fin.Payments (Id),
        Amount              DECIMAL(18, 4) NOT NULL,
        Reason              NVARCHAR(500)  NULL,
        ApprovedByUserId    BIGINT         NULL CONSTRAINT FK_fin_Ref_User FOREIGN KEY REFERENCES sec.Users (Id),
        RefundedAt          DATETIME2(3)   NOT NULL CONSTRAINT DF_fin_Ref_At DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'fin', N'Refunds', NULL, N'退费记录。';
EXEC dbo.sp_AddDescription N'fin', N'Refunds', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'Refunds', N'OriginalPaymentId', N'原支付流水主键。';
EXEC dbo.sp_AddDescription N'fin', N'Refunds', N'Amount', N'退费金额。';
EXEC dbo.sp_AddDescription N'fin', N'Refunds', N'Reason', N'退费原因。';
EXEC dbo.sp_AddDescription N'fin', N'Refunds', N'ApprovedByUserId', N'审批人用户主键。';
EXEC dbo.sp_AddDescription N'fin', N'Refunds', N'RefundedAt', N'退费时间（UTC）。';
GO

IF OBJECT_ID(N'fin.InvoiceBridgeLogs', N'U') IS NULL
BEGIN
    CREATE TABLE fin.InvoiceBridgeLogs
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_Bridge PRIMARY KEY,
        InvoiceId    BIGINT         NOT NULL CONSTRAINT FK_fin_Bridge_Inv FOREIGN KEY REFERENCES fin.Invoices (Id),
        RequestJson  NVARCHAR(MAX)  NULL,
        ResponseJson NVARCHAR(MAX)  NULL,
        Status       NVARCHAR(64)   NOT NULL,
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_fin_Bridge_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'fin', N'InvoiceBridgeLogs', NULL, N'电子发票/票据接口调用日志。';
EXEC dbo.sp_AddDescription N'fin', N'InvoiceBridgeLogs', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'InvoiceBridgeLogs', N'InvoiceId', N'结算单主键。';
EXEC dbo.sp_AddDescription N'fin', N'InvoiceBridgeLogs', N'RequestJson', N'请求报文。';
EXEC dbo.sp_AddDescription N'fin', N'InvoiceBridgeLogs', N'ResponseJson', N'响应报文。';
EXEC dbo.sp_AddDescription N'fin', N'InvoiceBridgeLogs', N'Status', N'调用状态。';
EXEC dbo.sp_AddDescription N'fin', N'InvoiceBridgeLogs', N'CreatedAt', N'记录时间（UTC）。';
GO

IF OBJECT_ID(N'fin.InsuranceReads', N'U') IS NULL
BEGIN
    CREATE TABLE fin.InsuranceReads
    (
        Id             BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_InsRead PRIMARY KEY,
        PatientId      BIGINT         NOT NULL CONSTRAINT FK_fin_InsRead_Pat FOREIGN KEY REFERENCES pat.Patients (Id),
        InsuredArea    NVARCHAR(128)  NULL,
        RawPayloadJson NVARCHAR(MAX)  NULL,
        ReadAt         DATETIME2(3)   NOT NULL CONSTRAINT DF_fin_InsRead_At DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReads', NULL, N'医保读卡/电子凭证读卡记录。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReads', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReads', N'PatientId', N'患者主键。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReads', N'InsuredArea', N'参保地/统筹区标识。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReads', N'RawPayloadJson', N'读卡原始报文 JSON。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReads', N'ReadAt', N'读卡时间（UTC）。';
GO

IF OBJECT_ID(N'fin.InsuranceSettlements', N'U') IS NULL
BEGIN
    CREATE TABLE fin.InsuranceSettlements
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_InsSet PRIMARY KEY,
        InvoiceId    BIGINT         NOT NULL CONSTRAINT FK_fin_InsSet_Inv FOREIGN KEY REFERENCES fin.Invoices (Id),
        InsTxnId     NVARCHAR(128)  NULL,
        FundPay      DECIMAL(18, 4) NOT NULL CONSTRAINT DF_fin_InsSet_Fund DEFAULT (0),
        SelfPay      DECIMAL(18, 4) NOT NULL CONSTRAINT DF_fin_InsSet_Self DEFAULT (0),
        SettledAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_fin_InsSet_At DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'fin', N'InsuranceSettlements', NULL, N'医保结算结果。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceSettlements', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceSettlements', N'InvoiceId', N'结算单主键。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceSettlements', N'InsTxnId', N'医保交易流水号。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceSettlements', N'FundPay', N'基金支付金额。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceSettlements', N'SelfPay', N'个人自费金额。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceSettlements', N'SettledAt', N'结算时间（UTC）。';
GO

IF OBJECT_ID(N'fin.InsuranceReconcileBatches', N'U') IS NULL
BEGIN
    CREATE TABLE fin.InsuranceReconcileBatches
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_InsRecBatch PRIMARY KEY,
        CampusId     BIGINT         NOT NULL CONSTRAINT FK_fin_RecBat_Campus FOREIGN KEY REFERENCES mdm.Campuses (Id),
        PeriodStart  DATE           NOT NULL,
        PeriodEnd    DATE           NOT NULL,
        Status       NVARCHAR(64)   NOT NULL CONSTRAINT DF_fin_RecBat_Status DEFAULT (N'Open'),
        CreatedAt    DATETIME2(3)   NOT NULL CONSTRAINT DF_fin_RecBat_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileBatches', NULL, N'医保对账批次。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileBatches', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileBatches', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileBatches', N'PeriodStart', N'对账周期起。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileBatches', N'PeriodEnd', N'对账周期止。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileBatches', N'Status', N'批次状态。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileBatches', N'CreatedAt', N'创建时间（UTC）。';
GO

IF OBJECT_ID(N'fin.InsuranceReconcileLines', N'U') IS NULL
BEGIN
    CREATE TABLE fin.InsuranceReconcileLines
    (
        Id             BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_fin_InsRecLine PRIMARY KEY,
        BatchId        BIGINT         NOT NULL CONSTRAINT FK_fin_RecLine_Bat FOREIGN KEY REFERENCES fin.InsuranceReconcileBatches (Id) ON DELETE CASCADE,
        SettlementId   BIGINT         NULL CONSTRAINT FK_fin_RecLine_Set FOREIGN KEY REFERENCES fin.InsuranceSettlements (Id),
        DiffAmount     DECIMAL(18, 4) NOT NULL CONSTRAINT DF_fin_RecLine_Diff DEFAULT (0),
        Resolution     NVARCHAR(500)  NULL
    );
END
GO
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileLines', NULL, N'医保对账明细。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileLines', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileLines', N'BatchId', N'对账批次主键。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileLines', N'SettlementId', N'关联医保结算记录。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileLines', N'DiffAmount', N'差异金额。';
EXEC dbo.sp_AddDescription N'fin', N'InsuranceReconcileLines', N'Resolution', N'处理说明。';
GO

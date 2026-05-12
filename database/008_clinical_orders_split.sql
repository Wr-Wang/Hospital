/*
  008_clinical_orders_split.sql
  分表：检验申请、检查申请、处方（门/急/住上下文三选一）。
  说明：AdmissionId 外键在 012 脚本末尾添加。
*/
USE [Hospital];
GO

/* ---------- lab.LabOrders ---------- */
IF OBJECT_ID(N'lab.LabOrders', N'U') IS NULL
BEGIN
    CREATE TABLE lab.LabOrders
    (
        Id                        BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_lab_LabOrders PRIMARY KEY,
        CampusId                  BIGINT         NOT NULL CONSTRAINT FK_lab_LabOrders_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        OutpatientEncounterId     BIGINT         NULL CONSTRAINT FK_lab_LabOrders_OPEC FOREIGN KEY REFERENCES enc.OutpatientEncounters (Id),
        EmergencyEncounterId      BIGINT         NULL CONSTRAINT FK_lab_LabOrders_EME FOREIGN KEY REFERENCES enc.EmergencyEncounters (Id),
        AdmissionId               BIGINT         NULL,
        OrderedByStaffId          BIGINT         NULL CONSTRAINT FK_lab_LabOrders_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        Status                    NVARCHAR(64)   NOT NULL CONSTRAINT DF_lab_LabOrders_Status DEFAULT (N'Ordered'),
        OrderedAt                 DATETIME2(3)   NOT NULL CONSTRAINT DF_lab_LabOrders_OrderedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT CK_lab_LabOrders_Context CHECK (
            (CASE WHEN OutpatientEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN EmergencyEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN AdmissionId IS NOT NULL THEN 1 ELSE 0 END) = 1
        )
    );
    CREATE INDEX IX_lab_LabOrders_OPEC ON lab.LabOrders (OutpatientEncounterId);
    CREATE INDEX IX_lab_LabOrders_EME ON lab.LabOrders (EmergencyEncounterId);
    CREATE INDEX IX_lab_LabOrders_Adm ON lab.LabOrders (AdmissionId);
END
GO
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', NULL, N'检验申请单头（门诊/急诊/住院三选一）。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', N'OutpatientEncounterId', N'门诊就诊主键。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', N'EmergencyEncounterId', N'急诊就诊主键。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', N'AdmissionId', N'住院主键（外键在 012 添加）。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', N'OrderedByStaffId', N'开立医生。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', N'Status', N'申请状态。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrders', N'OrderedAt', N'开立时间（UTC）。';
GO

IF OBJECT_ID(N'lab.LabOrderLines', N'U') IS NULL
BEGIN
    CREATE TABLE lab.LabOrderLines
    (
        Id             BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_lab_LabOrderLines PRIMARY KEY,
        LabOrderId     BIGINT         NOT NULL CONSTRAINT FK_lab_LabOrderLines_Orders FOREIGN KEY REFERENCES lab.LabOrders (Id) ON DELETE CASCADE,
        ChargeItemId   BIGINT         NULL CONSTRAINT FK_lab_LabOrderLines_ChargeItems FOREIGN KEY REFERENCES mdm.ChargeItems (Id),
        ItemName       NVARCHAR(256)  NULL,
        SpecimenType   NVARCHAR(128)  NULL,
        Qty            INT            NOT NULL CONSTRAINT DF_lab_LabOrderLines_Qty DEFAULT (1)
    );
    CREATE INDEX IX_lab_LabOrderLines_OrderId ON lab.LabOrderLines (LabOrderId);
END
GO
EXEC dbo.sp_AddDescription N'lab', N'LabOrderLines', NULL, N'检验申请明细。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrderLines', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrderLines', N'LabOrderId', N'检验申请头主键。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrderLines', N'ChargeItemId', N'收费项目主键（检验组合）。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrderLines', N'ItemName', N'项目名称（冗余展示）。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrderLines', N'SpecimenType', N'标本类型。';
EXEC dbo.sp_AddDescription N'lab', N'LabOrderLines', N'Qty', N'数量。';
GO

IF OBJECT_ID(N'lab.SpecimenCollections', N'U') IS NULL
BEGIN
    CREATE TABLE lab.SpecimenCollections
    (
        Id           BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_lab_SpecimenCollections PRIMARY KEY,
        LabOrderId   BIGINT         NOT NULL CONSTRAINT FK_lab_Specimen_LabOrders FOREIGN KEY REFERENCES lab.LabOrders (Id) ON DELETE CASCADE,
        CollectedAt  DATETIME2(3)   NULL,
        CollectorId  BIGINT         NULL CONSTRAINT FK_lab_Specimen_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        Status       NVARCHAR(64)   NOT NULL CONSTRAINT DF_lab_Specimen_Status DEFAULT (N'Pending')
    );
END
GO
EXEC dbo.sp_AddDescription N'lab', N'SpecimenCollections', NULL, N'标本采集与上机状态（简化）。';
EXEC dbo.sp_AddDescription N'lab', N'SpecimenCollections', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'lab', N'SpecimenCollections', N'LabOrderId', N'检验申请主键。';
EXEC dbo.sp_AddDescription N'lab', N'SpecimenCollections', N'CollectedAt', N'采集时间（UTC）。';
EXEC dbo.sp_AddDescription N'lab', N'SpecimenCollections', N'CollectorId', N'采集人（护士/医技）。';
EXEC dbo.sp_AddDescription N'lab', N'SpecimenCollections', N'Status', N'状态。';
GO

/* ---------- rad.ImagingOrders ---------- */
IF OBJECT_ID(N'rad.ImagingOrders', N'U') IS NULL
BEGIN
    CREATE TABLE rad.ImagingOrders
    (
        Id                        BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_rad_ImagingOrders PRIMARY KEY,
        CampusId                  BIGINT         NOT NULL CONSTRAINT FK_rad_Img_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        OutpatientEncounterId     BIGINT         NULL CONSTRAINT FK_rad_Img_OPEC FOREIGN KEY REFERENCES enc.OutpatientEncounters (Id),
        EmergencyEncounterId      BIGINT         NULL CONSTRAINT FK_rad_Img_EME FOREIGN KEY REFERENCES enc.EmergencyEncounters (Id),
        AdmissionId               BIGINT         NULL,
        OrderedByStaffId          BIGINT         NULL CONSTRAINT FK_rad_Img_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        Status                    NVARCHAR(64)   NOT NULL CONSTRAINT DF_rad_Img_Status DEFAULT (N'Ordered'),
        OrderedAt                 DATETIME2(3)   NOT NULL CONSTRAINT DF_rad_Img_OrderedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT CK_rad_ImagingOrders_Context CHECK (
            (CASE WHEN OutpatientEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN EmergencyEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN AdmissionId IS NOT NULL THEN 1 ELSE 0 END) = 1
        )
    );
    CREATE INDEX IX_rad_Img_OPEC ON rad.ImagingOrders (OutpatientEncounterId);
    CREATE INDEX IX_rad_Img_EME ON rad.ImagingOrders (EmergencyEncounterId);
    CREATE INDEX IX_rad_Img_Adm ON rad.ImagingOrders (AdmissionId);
END
GO
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', NULL, N'影像/检查申请单头（门诊/急诊/住院三选一）。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', N'OutpatientEncounterId', N'门诊就诊主键。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', N'EmergencyEncounterId', N'急诊就诊主键。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', N'AdmissionId', N'住院主键（外键在 012 添加）。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', N'OrderedByStaffId', N'开立医生。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', N'Status', N'申请状态。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrders', N'OrderedAt', N'开立时间（UTC）。';
GO

IF OBJECT_ID(N'rad.ImagingOrderLines', N'U') IS NULL
BEGIN
    CREATE TABLE rad.ImagingOrderLines
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_rad_ImagingOrderLines PRIMARY KEY,
        ImagingOrderId   BIGINT         NOT NULL CONSTRAINT FK_rad_ImgLines_Orders FOREIGN KEY REFERENCES rad.ImagingOrders (Id) ON DELETE CASCADE,
        ChargeItemId     BIGINT         NULL CONSTRAINT FK_rad_ImgLines_Charge FOREIGN KEY REFERENCES mdm.ChargeItems (Id),
        BodyPart         NVARCHAR(128)  NULL,
        Laterality       NVARCHAR(32)   NULL,
        Qty              INT            NOT NULL CONSTRAINT DF_rad_ImgLines_Qty DEFAULT (1)
    );
    CREATE INDEX IX_rad_ImgLines_OrderId ON rad.ImagingOrderLines (ImagingOrderId);
END
GO
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrderLines', NULL, N'检查申请明细。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrderLines', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrderLines', N'ImagingOrderId', N'检查申请头主键。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrderLines', N'ChargeItemId', N'收费项目主键。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrderLines', N'BodyPart', N'检查部位。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrderLines', N'Laterality', N'侧别（左/右/双侧）。';
EXEC dbo.sp_AddDescription N'rad', N'ImagingOrderLines', N'Qty', N'数量。';
GO

/* ---------- pha.Prescriptions ---------- */
IF OBJECT_ID(N'pha.Prescriptions', N'U') IS NULL
BEGIN
    CREATE TABLE pha.Prescriptions
    (
        Id                        BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_Prescriptions PRIMARY KEY,
        CampusId                  BIGINT         NOT NULL CONSTRAINT FK_pha_Rx_Campuses FOREIGN KEY REFERENCES mdm.Campuses (Id),
        OutpatientEncounterId     BIGINT         NULL CONSTRAINT FK_pha_Rx_OPEC FOREIGN KEY REFERENCES enc.OutpatientEncounters (Id),
        EmergencyEncounterId      BIGINT         NULL CONSTRAINT FK_pha_Rx_EME FOREIGN KEY REFERENCES enc.EmergencyEncounters (Id),
        AdmissionId               BIGINT         NULL,
        PrescriptionNo            NVARCHAR(64)   NOT NULL,
        Status                    NVARCHAR(64)   NOT NULL CONSTRAINT DF_pha_Rx_Status DEFAULT (N'Active'),
        PrescribedByStaffId       BIGINT         NULL CONSTRAINT FK_pha_Rx_Staff FOREIGN KEY REFERENCES mdm.Staff (Id),
        PrescribedAt               DATETIME2(3)   NOT NULL CONSTRAINT DF_pha_Rx_PrescribedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT CK_pha_Prescriptions_Context CHECK (
            (CASE WHEN OutpatientEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN EmergencyEncounterId IS NOT NULL THEN 1 ELSE 0 END
             + CASE WHEN AdmissionId IS NOT NULL THEN 1 ELSE 0 END) = 1
        ),
        CONSTRAINT UQ_pha_Prescriptions_CampusNo UNIQUE (CampusId, PrescriptionNo)
    );
    CREATE INDEX IX_pha_Rx_OPEC ON pha.Prescriptions (OutpatientEncounterId);
    CREATE INDEX IX_pha_Rx_EME ON pha.Prescriptions (EmergencyEncounterId);
    CREATE INDEX IX_pha_Rx_Adm ON pha.Prescriptions (AdmissionId);
END
GO
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', NULL, N'处方头（门诊/急诊/住院三选一）。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'CampusId', N'院区。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'OutpatientEncounterId', N'门诊就诊主键。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'EmergencyEncounterId', N'急诊就诊主键。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'AdmissionId', N'住院主键（外键在 012 添加）。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'PrescriptionNo', N'处方号，院区内唯一。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'Status', N'处方状态。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'PrescribedByStaffId', N'开立医生。';
EXEC dbo.sp_AddDescription N'pha', N'Prescriptions', N'PrescribedAt', N'开立时间（UTC）。';
GO

IF OBJECT_ID(N'pha.PrescriptionLines', N'U') IS NULL
BEGIN
    CREATE TABLE pha.PrescriptionLines
    (
        Id               BIGINT         NOT NULL IDENTITY(1, 1) CONSTRAINT PK_pha_PrescriptionLines PRIMARY KEY,
        PrescriptionId   BIGINT         NOT NULL CONSTRAINT FK_pha_RxLines_Rx FOREIGN KEY REFERENCES pha.Prescriptions (Id) ON DELETE CASCADE,
        DrugId           BIGINT         NOT NULL,
        Dose             NVARCHAR(64)   NULL,
        Frequency        NVARCHAR(64)   NULL,
        Days             INT            NULL,
        Route            NVARCHAR(64)   NULL,
        Qty              DECIMAL(18, 4) NOT NULL CONSTRAINT DF_pha_RxLines_Qty DEFAULT (1)
    );
    CREATE INDEX IX_pha_RxLines_RxId ON pha.PrescriptionLines (PrescriptionId);
END
GO
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', NULL, N'处方明细（DrugId 外键在 009 药品表创建后由 ALTER 添加）。';
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', N'Id', N'主键。';
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', N'PrescriptionId', N'处方头主键。';
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', N'DrugId', N'药品主键（pha.Drugs）。';
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', N'Dose', N'单次剂量。';
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', N'Frequency', N'用药频次。';
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', N'Days', N'用药天数。';
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', N'Route', N'给药途径。';
EXEC dbo.sp_AddDescription N'pha', N'PrescriptionLines', N'Qty', N'发药数量（计算结果或开立总量）。';
GO

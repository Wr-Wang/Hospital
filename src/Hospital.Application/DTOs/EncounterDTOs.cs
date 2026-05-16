using System.Collections.Generic;

namespace Hospital.Application.DTOs;

// ===== 就诊与患者队列 =====

public sealed record EncounterDto(
    long Id,
    long PatientId,
    long DoctorId,
    long DeptId,
    long CampusId,
    long RegistrationId,
    string Status,
    string? StartTime,
    string? EndTime);

public sealed record EncounterQueueItemDto(
    long Id,
    long PatientId,
    string PatientName,
    string PatientGender,
    string PatientIdCard,
    int QueueNumber,
    string SlotName,
    string Status,
    string RegisterTime);

// ===== 病历 =====

public sealed record MedicalRecordDto(
    long Id,
    long EncounterId,
    string ChiefComplaint,
    string PresentIllness,
    string PastHistory,
    string PhysicalExam,
    string Status,
    int Version);

public sealed record SaveMedicalRecordDto(
    string ChiefComplaint,
    string PresentIllness,
    string PastHistory,
    string PhysicalExam,
    bool IsSubmit);

// ===== 诊断 =====

public sealed record DiagnosisDto(
    long Id,
    long EncounterId,
    string DiagnosisType,
    string IcdCode,
    string Description,
    bool IsPrimary);

public sealed record CreateDiagnosisDto(
    string DiagnosisType,
    string IcdCode,
    string Description,
    bool IsPrimary);

// ===== 处方 =====

public sealed record PrescriptionDto(
    long Id,
    long EncounterId,
    long DoctorId,
    string Status,
    List<PrescriptionItemDto> Items);

public sealed record PrescriptionItemDto(
    long Id,
    string DrugName,
    string Spec,
    string Form,
    string Freq,
    string Dosage,
    int Duration,
    int Qty,
    string Note);

public sealed record CreatePrescriptionDto(
    List<CreatePrescriptionItemDto> Items);

public sealed record CreatePrescriptionItemDto(
    string DrugName,
    string Spec,
    string Form,
    string Freq,
    string Dosage,
    int Duration,
    int Qty,
    string? Note);

// ===== 检验检查 =====

public sealed record LabOrderDto(
    long Id,
    long EncounterId,
    string ItemCode,
    string ItemName,
    string Status);

public sealed record CreateLabOrderDto(
    string ItemCode,
    string ItemName);

public sealed record RadOrderDto(
    long Id,
    long EncounterId,
    string ItemCode,
    string ItemName,
    string Status);

public sealed record CreateRadOrderDto(
    string ItemCode,
    string ItemName);

using System.Collections.Generic;

namespace Hospital.Application.DTOs;

// ===== 收费工作台 =====

/// <summary>待收费项目（支持处方 + 检验 + 检查统一展示）</summary>
public sealed record ChargeItemDto(
    long Id,
    string ItemType,   // Prescription / LabOrder / RadOrder
    string ItemName,
    string PatientName,
    string DoctorName,
    string DeptName,
    decimal Amount,
    string Status,
    string CreateTime);

/// <summary>缴费请求</summary>
public sealed record PayRequestDto(
    List<PayItemDto> Items);

public sealed record PayItemDto(
    long Id,
    string ItemType);  // Prescription / LabOrder / RadOrder

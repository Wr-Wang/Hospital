namespace Hospital.Application.DTOs;

public sealed record PatientDto(
    long Id,
    string PatientNo,
    string Name,
    string? Gender,
    string? BirthDate,
    string? Phone,
    string? AllergiesText,
    string? IdCard);

public sealed record CreatePatientDto(
    string PatientNo,
    string Name,
    string? Gender,
    string? BirthDate,
    string? Phone,
    string? AllergiesText,
    string? IdCard);

public sealed record PatientSearchResultDto(
    List<PatientDto> Items,
    int TotalCount,
    int Page,
    int Size);

public sealed record PatientProfileDto(
    long Id,
    string PatientNo,
    string Name,
    string? Gender,
    string? BirthDate,
    string? Phone,
    string? AllergiesText,
    string? IdCard,
    List<VisitSummaryDto> RecentVisits);

public sealed record VisitSummaryDto(
    long Id,
    string Date,
    string DeptName,
    string? DoctorName,
    string? Diagnosis);

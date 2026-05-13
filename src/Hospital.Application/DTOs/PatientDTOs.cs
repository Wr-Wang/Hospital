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

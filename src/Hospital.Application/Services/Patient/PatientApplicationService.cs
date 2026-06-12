using System.Globalization;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.ValueObjects;

namespace Hospital.Application.Services;

public sealed class PatientApplicationService : IPatientApplicationService
{
    private readonly IPatientRepository _patientRepository;

    public PatientApplicationService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto?> GetByIdAsync(long id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        return MapToDto(patient);
    }

    public async Task<PatientDto?> GetByPatientNoAsync(string patientNo)
    {
        var patient = await _patientRepository.GetByPatientNoAsync(patientNo);
        return MapToDto(patient);
    }

    public async Task<PatientDto?> GetByIdCardAsync(string idCard)
    {
        var patient = await _patientRepository.GetByIdCardAsync(idCard);
        return MapToDto(patient);
    }

    public async Task<List<PatientDto>> GetSuspectDuplicatesAsync(string name, string? phone)
    {
        var patients = await _patientRepository.GetSuspectDuplicatesAsync(name, phone);
        return patients.Select(p => MapToDto(p)!).ToList();
    }

    public async Task<PatientSearchResultDto> SearchAsync(string? keyword, int page, int size)
    {
        var (items, totalCount) = await _patientRepository.SearchAsync(keyword, page, size);
        return new PatientSearchResultDto(
            items.Select(p => MapToDto(p)!).ToList(),
            totalCount,
            page,
            size);
    }

    public async Task<PatientProfileDto?> GetProfileAsync(long id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient is null) return null;

        // 目前返回基本信息，就诊历史待后续模块对接后补充
        return new PatientProfileDto(
            patient.Id,
            patient.PatientNo,
            patient.Name,
            patient.Gender?.ToString(),
            patient.BirthDate?.ToString("yyyy-MM-dd"),
            patient.Phone?.Value,
            patient.AllergiesText,
            patient.IdCard?.Number,
            new List<VisitSummaryDto>());
    }

    public async Task<long> CreateAsync(CreatePatientDto request)
    {
        Gender? gender = request.Gender != null ? Enum.Parse<Gender>(request.Gender) : null;
        DateOnly? birthDate = request.BirthDate != null ? DateOnly.ParseExact(request.BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
        PhoneNumber? phone = request.Phone != null ? new PhoneNumber(request.Phone) : null;
        IdCard? idCard = request.IdCard != null ? new IdCard(request.IdCard) : null;

        // 自动生成 PatientNo（前端未传入时）
        var patientNo = string.IsNullOrWhiteSpace(request.PatientNo)
            ? GeneratePatientNo()
            : request.PatientNo;

        var patient = new Patient(patientNo, request.Name, gender, birthDate, phone, request.AllergiesText, idCard);
        await _patientRepository.AddAsync(patient);
        return patient.Id;
    }

    private static string GeneratePatientNo()
    {
        var now = DateTime.UtcNow;
        var random = Random.Shared.Next(100, 999);
        return $"P{now:yyyyMMddHHmmss}{random}";
    }

    private static PatientDto? MapToDto(Patient? patient)
    {
        if (patient is null)
            return null;

        return new PatientDto(
            patient.Id,
            patient.PatientNo,
            patient.Name,
            patient.Gender?.ToString(),
            patient.BirthDate?.ToString("yyyy-MM-dd"),
            patient.Phone?.Value,
            patient.AllergiesText,
            patient.IdCard?.Number);
    }
}

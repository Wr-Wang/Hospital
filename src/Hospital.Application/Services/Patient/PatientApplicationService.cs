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

    public async Task<long> CreateAsync(CreatePatientDto request)
    {
        Gender? gender = request.Gender != null ? Enum.Parse<Gender>(request.Gender) : null;
        DateOnly? birthDate = request.BirthDate != null ? DateOnly.Parse(request.BirthDate) : null;
        PhoneNumber? phone = request.Phone != null ? new PhoneNumber(request.Phone) : null;
        IdCard? idCard = request.IdCard != null ? new IdCard(request.IdCard) : null;

        var patient = new Patient(request.PatientNo, request.Name, gender, birthDate, phone, request.AllergiesText, idCard);
        await _patientRepository.AddAsync(patient);
        return patient.Id;
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

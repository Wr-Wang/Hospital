using System.Globalization;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.Entities;
using Hospital.Domain.ValueObjects;

namespace Hospital.Application.Services;

public sealed class PatientApplicationService : IPatientApplicationService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IEncounterRepository _encounterRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly ILabOrderRepository _labOrderRepository;
    private readonly IRadOrderRepository _radOrderRepository;

    public PatientApplicationService(
        IPatientRepository patientRepository,
        IEncounterRepository encounterRepository,
        IRegistrationRepository registrationRepository,
        IDepartmentRepository departmentRepository,
        IStaffRepository staffRepository,
        IDiagnosisRepository diagnosisRepository,
        IPrescriptionRepository prescriptionRepository,
        ILabOrderRepository labOrderRepository,
        IRadOrderRepository radOrderRepository)
    {
        _patientRepository = patientRepository;
        _encounterRepository = encounterRepository;
        _registrationRepository = registrationRepository;
        _departmentRepository = departmentRepository;
        _staffRepository = staffRepository;
        _diagnosisRepository = diagnosisRepository;
        _prescriptionRepository = prescriptionRepository;
        _labOrderRepository = labOrderRepository;
        _radOrderRepository = radOrderRepository;
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

        // 就诊历史：按就诊倒序，逐条补齐挂号日期/科室/医生/主诊断
        var encounters = await _encounterRepository.GetByPatientAsync(id);
        var visits = new List<VisitSummaryDto>();
        foreach (var e in encounters.OrderByDescending(e => e.Id))
        {
            var registration = await _registrationRepository.GetByIdAsync(e.RegistrationId);
            var dept = await _departmentRepository.GetByIdAsync(e.DeptId);
            var doctor = await _staffRepository.GetByIdAsync(e.DoctorId);
            var diagnoses = await _diagnosisRepository.GetByEncounterIdAsync(e.Id);
            var primary = diagnoses.FirstOrDefault(d => d.IsPrimary) ?? diagnoses.FirstOrDefault();

            visits.Add(new VisitSummaryDto(
                e.Id,
                registration?.RegisterTime.ToString("yyyy-MM-dd HH:mm") ?? string.Empty,
                dept?.Name ?? string.Empty,
                doctor?.Name,
                primary?.Description));
        }

        // 处方/检验/放射按患者聚合（临床实体只关联 EncounterId，经 Encounter 反查 PatientId）
        var prescriptions = (await _prescriptionRepository.GetByPatientAsync(id))
            .Select(MapToPrescriptionDto).ToList();
        var labOrders = (await _labOrderRepository.GetByPatientAsync(id))
            .Select(o => new LabOrderDto(o.Id, o.EncounterId, o.ItemCode, o.ItemName, o.Status.ToString())).ToList();
        var radOrders = (await _radOrderRepository.GetByPatientAsync(id))
            .Select(o => new RadOrderDto(o.Id, o.EncounterId, o.ItemCode, o.ItemName, o.Status.ToString())).ToList();

        return new PatientProfileDto(
            patient.Id,
            patient.PatientNo,
            patient.Name,
            patient.Gender?.ToString(),
            patient.BirthDate?.ToString("yyyy-MM-dd"),
            patient.Phone?.Value,
            patient.AllergiesText,
            patient.IdCard?.Number,
            visits,
            prescriptions,
            labOrders,
            radOrders);
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

    public async Task UpdateAsync(long id, UpdatePatientDto request)
    {
        var patient = await _patientRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("患者不存在");

        Gender? gender = request.Gender != null ? Enum.Parse<Gender>(request.Gender) : null;
        DateOnly? birthDate = request.BirthDate != null ? DateOnly.ParseExact(request.BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
        PhoneNumber? phone = request.Phone != null ? new PhoneNumber(request.Phone) : null;

        patient.UpdateBasicInfo(request.Name, gender, birthDate, phone, request.AllergiesText);

        // 身份证补全：仅当传入了非空身份证时更新，避免误清空已登记的证件号
        if (!string.IsNullOrWhiteSpace(request.IdCard))
            patient.UpdateIdCard(new IdCard(request.IdCard));

        await _patientRepository.UpdateAsync(patient);
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

    private static PrescriptionDto MapToPrescriptionDto(Prescription p)
    {
        return new PrescriptionDto(
            p.Id, p.EncounterId, p.DoctorId, p.Status.ToString(),
            p.Items.Select(i => new PrescriptionItemDto(
                i.Id, i.DrugName, i.Spec, i.Form, i.Freq,
                i.Dosage, i.Duration, i.Qty, i.Note)).ToList());
    }
}

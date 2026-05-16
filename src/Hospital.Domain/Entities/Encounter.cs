using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>就诊记录实体，支持待诊→就诊中→已完成状态流转</summary>
public class Encounter : Entity
{
    // EF Core
    private Encounter() { }

    public Encounter(long patientId, long doctorId, long deptId, long campusId, long registrationId)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        DeptId = deptId;
        CampusId = campusId;
        RegistrationId = registrationId;
        Status = EncounterStatus.待诊;
    }

    public long PatientId { get; private set; }
    public long DoctorId { get; private set; }
    public long DeptId { get; private set; }
    public long CampusId { get; private set; }
    public long RegistrationId { get; private set; }
    public EncounterStatus Status { get; private set; }
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }

    /// <summary>开始接诊</summary>
    public void StartConsultation()
    {
        if (Status != EncounterStatus.待诊)
            throw new InvalidOperationException("仅待诊状态的记录可以开始接诊");

        Status = EncounterStatus.就诊中;
        StartTime = DateTime.Now;
    }

    /// <summary>完成接诊</summary>
    public void CompleteConsultation()
    {
        if (Status != EncounterStatus.就诊中)
            throw new InvalidOperationException("仅就诊中状态的记录可以完成接诊");

        Status = EncounterStatus.已完成;
        EndTime = DateTime.Now;
    }
}

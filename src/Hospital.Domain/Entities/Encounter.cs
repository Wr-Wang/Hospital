namespace Hospital.Domain.Entities;

/// <summary>就诊记录实体（基础版），挂号时创建，Phase 5/6 扩展为完整模块</summary>
public class Encounter : Entity
{
    // EF Core
    private Encounter() { Status = default!; }

    public Encounter(long patientId, long doctorId, long deptId, long campusId, long registrationId)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        DeptId = deptId;
        CampusId = campusId;
        RegistrationId = registrationId;
        Status = "待就诊";
    }

    public long PatientId { get; private set; }
    public long DoctorId { get; private set; }
    public long DeptId { get; private set; }
    public long CampusId { get; private set; }
    public long RegistrationId { get; private set; }
    public string Status { get; set; }
}

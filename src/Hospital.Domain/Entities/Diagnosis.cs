using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>诊断记录实体</summary>
public class Diagnosis : Entity
{
    // EF Core
    private Diagnosis() { }

    public Diagnosis(long encounterId, DiagnosisType diagnosisType, string icdCode, string description, bool isPrimary)
    {
        EncounterId = encounterId;
        DiagnosisType = diagnosisType;
        IcdCode = icdCode;
        Description = description;
        IsPrimary = isPrimary;
    }

    public long EncounterId { get; private set; }
    public DiagnosisType DiagnosisType { get; private set; }
    public string IcdCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
}

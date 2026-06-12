namespace Hospital.Domain.Aggregates.Patient;

public class PatientIdentifier : Entity
{
    public string IdType { get; private set; }
    public string IdValue { get; private set; }
    public bool IsPrimary { get; private set; }

    private PatientIdentifier()
    {
        IdType = default!;
        IdValue = default!;
    } // For EF Core

    public PatientIdentifier(string idType, string idValue, bool isPrimary)
    {
        IdType = idType ?? throw new ArgumentNullException(nameof(idType));
        IdValue = idValue ?? throw new ArgumentNullException(nameof(idValue));
        IsPrimary = isPrimary;
    }
}
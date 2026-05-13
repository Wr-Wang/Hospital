using Hospital.Domain.Events;

namespace Hospital.Domain.Aggregates.Patient.Events;

public class PatientCreatedEvent : DomainEvent
{
    public long PatientId { get; }
    public string PatientNo { get; }

    public PatientCreatedEvent(long patientId, string patientNo)
    {
        PatientId = patientId;
        PatientNo = patientNo;
    }
}

public class PatientUpdatedEvent : DomainEvent
{
    public long PatientId { get; }

    public PatientUpdatedEvent(long patientId)
    {
        PatientId = patientId;
    }
}

public class PatientIdentifierAddedEvent : DomainEvent
{
    public long PatientId { get; }
    public long IdentifierId { get; }

    public PatientIdentifierAddedEvent(long patientId, long identifierId)
    {
        PatientId = patientId;
        IdentifierId = identifierId;
    }
}

public class PatientConsentGrantedEvent : DomainEvent
{
    public long PatientId { get; }
    public long ConsentId { get; }

    public PatientConsentGrantedEvent(long patientId, long consentId)
    {
        PatientId = patientId;
        ConsentId = consentId;
    }
}

public class PatientMergedEvent : DomainEvent
{
    public long SurvivorPatientId { get; }
    public long MergedPatientId { get; }
    public string MergedBy { get; }

    public PatientMergedEvent(long survivorPatientId, long mergedPatientId, string mergedBy)
    {
        SurvivorPatientId = survivorPatientId;
        MergedPatientId = mergedPatientId;
        MergedBy = mergedBy;
    }
}
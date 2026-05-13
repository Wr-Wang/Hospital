using Hospital.Domain.ValueObjects;
using Hospital.Domain.Events;
using Hospital.Domain.Aggregates.Patient.Events;

namespace Hospital.Domain.Aggregates.Patient;

public class Patient : AggregateRoot
{
    public string PatientNo { get; private set; }
    public string Name { get; private set; }
    public Gender? Gender { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public string? AllergiesText { get; private set; }
    public IdCard? IdCard { get; private set; }

    private readonly List<PatientIdentifier> _identifiers = new();
    public IReadOnlyCollection<PatientIdentifier> Identifiers => _identifiers.AsReadOnly();

    private readonly List<PatientConsent> _consents = new();
    public IReadOnlyCollection<PatientConsent> Consents => _consents.AsReadOnly();

    private Patient()
    {
        PatientNo = default!;
        Name = default!;
    } // For EF Core

    public Patient(string patientNo, string name, Gender? gender, DateOnly? birthDate, PhoneNumber? phone, string? allergiesText, IdCard? idCard)
    {
        PatientNo = patientNo ?? throw new ArgumentNullException(nameof(patientNo));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Gender = gender;
        BirthDate = birthDate;
        Phone = phone;
        AllergiesText = allergiesText;
        IdCard = idCard;

        AddDomainEvent(new PatientCreatedEvent(Id, PatientNo));
    }

    public void UpdateBasicInfo(string name, Gender? gender, DateOnly? birthDate, PhoneNumber? phone, string? allergiesText)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Gender = gender;
        BirthDate = birthDate;
        Phone = phone;
        AllergiesText = allergiesText;

        AddDomainEvent(new PatientUpdatedEvent(Id));
    }

    public void AddIdentifier(string idType, string idValue, bool isPrimary)
    {
        if (_identifiers.Any(i => i.IdType == idType && i.IdValue == idValue))
            throw new InvalidOperationException("Identifier already exists");

        if (isPrimary && _identifiers.Any(i => i.IsPrimary))
            throw new InvalidOperationException("Only one primary identifier allowed");

        var identifier = new PatientIdentifier(idType, idValue, isPrimary);
        _identifiers.Add(identifier);

        AddDomainEvent(new PatientIdentifierAddedEvent(Id, identifier.Id));
    }

    public void AddConsent(string consentType, DateTimeOffset grantedAt, DateTimeOffset? expiresAt, string? documentRef)
    {
        var consent = new PatientConsent(consentType, grantedAt, expiresAt, documentRef);
        _consents.Add(consent);

        AddDomainEvent(new PatientConsentGrantedEvent(Id, consent.Id));
    }

    public void MergeWith(Patient otherPatient, string mergedBy)
    {
        // Business logic for merging
        // Assume otherPatient is marked as deleted

        AddDomainEvent(new PatientMergedEvent(Id, otherPatient.Id, mergedBy));
    }
}
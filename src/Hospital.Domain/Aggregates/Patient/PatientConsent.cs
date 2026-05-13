namespace Hospital.Domain.Aggregates.Patient;

public class PatientConsent : Entity
{
    public string ConsentType { get; private set; }
    public DateTimeOffset GrantedAt { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public string? DocumentRef { get; private set; }

    private PatientConsent()
    {
        ConsentType = default!;
    } // For EF Core

    public PatientConsent(string consentType, DateTimeOffset grantedAt, DateTimeOffset? expiresAt, string? documentRef)
    {
        ConsentType = consentType ?? throw new ArgumentNullException(nameof(consentType));
        GrantedAt = grantedAt;
        ExpiresAt = expiresAt;
        DocumentRef = documentRef;
    }
}
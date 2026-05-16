using Hospital.Domain.ValueObjects;

namespace Hospital.Domain.Entities;

public class Campus : Entity
{
    public CampusCode Code { get; private set; }
    public string Name { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Campus()
    {
        Code = default!;
        Name = default!;
    } // For EF Core

    public Campus(CampusCode code, string name, string? address, string? phone)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address;
        Phone = phone;
    }

    public void UpdateInfo(string name, string? address, string? phone)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address;
        Phone = phone;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}

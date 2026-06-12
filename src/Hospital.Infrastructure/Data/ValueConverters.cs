using Hospital.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hospital.Infrastructure.Data;

public static class ValueConverters
{
    public static readonly ValueConverter<CampusCode, string> CampusCodeConverter
        = new(v => v.Value, v => new CampusCode(v));

    public static readonly ValueConverter<DepartmentCode, string> DepartmentCodeConverter
        = new(v => v.Value, v => new DepartmentCode(v));

    public static readonly ValueConverter<LicenseNumber, string> LicenseNumberConverter
        = new(v => v.Value, v => new LicenseNumber(v));

    public static readonly ValueConverter<PhoneNumber, string> PhoneNumberConverter
        = new(v => v.Value, v => new PhoneNumber(v));

    public static readonly ValueConverter<IdCard, string> IdCardConverter
        = new(v => v.Number, v => new IdCard(v));
}

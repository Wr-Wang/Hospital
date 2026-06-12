using Hospital.Application.Repositories;
using Hospital.Domain;
using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.ValueObjects;

namespace Hospital.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients = new();

    public PatientRepository()
    {
        var seeds = new Patient[]
        {
            new("P20250001", "王小明", Gender.Male, new DateOnly(1990, 5, 15), new PhoneNumber("13800138010"), null, new IdCard("110101199005151234")),
            new("P20250002", "李小红", Gender.Female, new DateOnly(1985, 8, 22), new PhoneNumber("13800138011"), "青霉素过敏", new IdCard("110101198508221234")),
            new("P20250003", "张伟", Gender.Male, new DateOnly(1978, 12, 3), new PhoneNumber("13800138012"), null, new IdCard("110101197812031234")),
            new("P20250004", "赵丽华", Gender.Female, new DateOnly(1995, 3, 18), new PhoneNumber("13800138013"), "磺胺类药物过敏", new IdCard("110101199503181234")),
            new("P20250005", "刘强", Gender.Male, new DateOnly(2000, 7, 9), new PhoneNumber("13800138014"), null, new IdCard("110101200007091234")),
            new("P20250006", "陈芳", Gender.Female, new DateOnly(1965, 11, 25), new PhoneNumber("13800138015"), null, new IdCard("110101196511251234")),
            new("P20250007", "杨磊", Gender.Male, new DateOnly(1988, 1, 30), new PhoneNumber("13800138016"), "头孢菌素过敏", new IdCard("110101198801301234")),
            new("P20250008", "周敏", Gender.Female, new DateOnly(1992, 6, 14), new PhoneNumber("13800138017"), null, null),
            new("P20250009", "吴刚", Gender.Male, new DateOnly(1975, 9, 8), null, null, new IdCard("110101197509081234")),
            new("P20250010", "林小燕", Gender.Female, new DateOnly(2002, 4, 20), new PhoneNumber("13800138018"), "花粉过敏", null),
        };
        for (int i = 0; i < seeds.Length; i++)
        {
            typeof(Entity).GetProperty("Id")?.SetValue(seeds[i], i + 1);
            _patients.Add(seeds[i]);
        }
    }

    public Task<Patient?> GetByIdAsync(long id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(patient);
    }

    public Task<Patient?> GetByPatientNoAsync(string patientNo)
    {
        var patient = _patients.FirstOrDefault(p => p.PatientNo == patientNo);
        return Task.FromResult(patient);
    }

    public Task<Patient?> GetByIdCardAsync(string idCard)
    {
        var patient = _patients.FirstOrDefault(p => p.IdCard?.Number == idCard);
        return Task.FromResult(patient);
    }

    public Task<List<Patient>> GetSuspectDuplicatesAsync(string name, string? phone)
    {
        var matches = _patients.Where(p =>
            p.Name.Contains(name, StringComparison.OrdinalIgnoreCase) &&
            (phone == null || p.Phone?.Value == phone))
            .Take(10)
            .ToList();
        return Task.FromResult(matches);
    }

    public Task<(List<Patient> Items, int TotalCount)> SearchAsync(string? keyword, int page, int size)
    {
        var query = _patients.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p =>
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                (p.IdCard?.Number.Contains(keyword) ?? false) ||
                (p.Phone?.Value.Contains(keyword) ?? false) ||
                p.PatientNo.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return Task.FromResult((items, totalCount));
    }

    public Task AddAsync(Patient patient)
    {
        patient.GetType().GetProperty("Id")?.SetValue(patient, _patients.Count + 1);
        _patients.Add(patient);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Patient patient)
    {
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient is not null)
            _patients.Remove(patient);
        return Task.CompletedTask;
    }
}

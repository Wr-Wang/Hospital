using Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>患者编号生成器（格式: P + YYYYMMDD + 4位序列）</summary>
public sealed class PatientNoService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PatientNoService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<string> NextNoAsync()
    {
        var today = DateTime.Now.ToString("yyyyMMdd");
        var prefix = $"P{today}";

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

        var maxNo = await db.Patients
            .Where(p => p.PatientNo.StartsWith(prefix))
            .MaxAsync(p => (string?)p.PatientNo) ?? string.Empty;

        var seq = 1;
        if (maxNo.Length >= prefix.Length + 4 && int.TryParse(maxNo[^4..], out var lastSeq))
            seq = lastSeq + 1;

        return $"{prefix}{seq:D4}";
    }
}

using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Infrastructure.Data;

/// <summary>
/// 启动时初始化数据库（EnsureCreated 建表 + 种子患者数据）。
/// 初始化失败不阻断应用启动，由后台循环重试，直到 SQL Server 可用。
/// 避免 SQL 短暂不可用时整个 API 以 500.30 挂掉、且 SQL 恢复后需要手动回收应用池才能恢复的问题。
/// </summary>
public sealed class DatabaseInitializationService : BackgroundService
{
    private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseInitializationService> _logger;
    private int _initialized;
    private int _consecutiveFailures;

    public DatabaseInitializationService(
        IServiceScopeFactory scopeFactory,
        ILogger<DatabaseInitializationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>启动时尽力完成一次初始化；失败只记录日志，由后台任务继续重试。</summary>
    public Task EnsureDatabaseReadyAsync() => TryInitializeAsync();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await TryInitializeAsync())
                return;

            try
            {
                await Task.Delay(RetryInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private async Task<bool> TryInitializeAsync()
    {
        if (Volatile.Read(ref _initialized) == 1)
            return true;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

            db.Database.EnsureCreated();
            SeedPatientsIfEmpty(db);

            Volatile.Write(ref _initialized, 1);
            _consecutiveFailures = 0;
            _logger.LogInformation("数据库初始化完成（EnsureCreated + 种子患者数据）");
            return true;
        }
        catch (Exception ex)
        {
            _consecutiveFailures++;
            if (_consecutiveFailures == 1)
                _logger.LogWarning(ex, "数据库初始化失败：{Message}。每 {Interval}s 自动重试，直到 SQL Server 可用",
                    ex.Message, RetryInterval.TotalSeconds);
            else
                _logger.LogDebug(ex, "数据库初始化重试中（第 {Attempt} 次失败）：{Message}", _consecutiveFailures, ex.Message);
            return false;
        }
    }

    private static void SeedPatientsIfEmpty(HospitalDbContext db)
    {
        if (db.Patients.Any())
            return;

        db.Patients.AddRange(
            new Patient("P20250001", "张明", Gender.Male, new DateOnly(1990, 3, 15), new PhoneNumber("13800138001"), "青霉素过敏", new IdCard("110101199003151234")),
            new Patient("P20250002", "李芳", Gender.Female, new DateOnly(1985, 7, 20), new PhoneNumber("13800138002"), null, new IdCard("110101198507202345")),
            new Patient("P20250003", "王建国", Gender.Male, new DateOnly(1978, 11, 11), new PhoneNumber("13800138003"), "磺胺类过敏", new IdCard("110101197811113456")),
            new Patient("P20250004", "赵秀英", Gender.Female, new DateOnly(1992, 8, 8), new PhoneNumber("13800138004"), null, new IdCard("110101199208084567")),
            new Patient("P20250005", "刘浩然", Gender.Male, new DateOnly(2001, 5, 5), new PhoneNumber("13800138005"), null, new IdCard("110101200105055678")),
            new Patient("P20250006", "陈德明", Gender.Male, new DateOnly(1965, 12, 25), new PhoneNumber("13800138006"), "阿司匹林过敏", new IdCard("110101196512256789")),
            new Patient("P20250007", "杨雪", Gender.Female, new DateOnly(1995, 9, 15), new PhoneNumber("13800138007"), null, new IdCard("110101199509152345")),
            new Patient("P20250008", "黄海波", Gender.Male, new DateOnly(1982, 3, 30), new PhoneNumber("13800138008"), null, new IdCard("110101198203308901")),
            new Patient("P20250009", "周玉兰", Gender.Female, new DateOnly(1976, 9, 9), new PhoneNumber("13800138009"), "头孢类过敏", new IdCard("110101197609092345")),
            new Patient("P20250010", "吴磊", Gender.Male, new DateOnly(1998, 8, 18), new PhoneNumber("13800138010"), null, new IdCard("110101199808186789"))
        );
        db.SaveChanges();
    }
}

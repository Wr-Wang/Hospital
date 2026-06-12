using System.Reflection;
using System.Text;
using Hospital.Api.Filters;
using Hospital.Api.Middleware;
using Hospital.Application.Services;
using Hospital.Application.Repositories;
using Hospital.Infrastructure.Repositories;
using Hospital.Infrastructure.Repositories.Ef;
using Hospital.Application.Services.WeChat;
using Hospital.Infrastructure.ExternalServices;
using Hospital.Infrastructure.Data;
using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.ValueObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Register EF Core DbContext
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("HospitalDb"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(3)));

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiLogFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// Register EF Core repositories (Scoped — aligned with DbContext lifetime)
builder.Services.AddScoped<IPatientRepository, EfPatientRepository>();
builder.Services.AddScoped<ICampusRepository, EfCampusRepository>();
builder.Services.AddScoped<IDepartmentRepository, EfDepartmentRepository>();
builder.Services.AddScoped<IStaffRepository, EfStaffRepository>();
builder.Services.AddScoped<IDictionaryRepository, EfDictionaryRepository>();
builder.Services.AddScoped<IScheduleRepository, EfScheduleRepository>();
builder.Services.AddScoped<IRegistrationRepository, EfRegistrationRepository>();
builder.Services.AddScoped<IEncounterRepository, EfEncounterRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, EfMedicalRecordRepository>();
builder.Services.AddScoped<IDiagnosisRepository, EfDiagnosisRepository>();
builder.Services.AddScoped<IPrescriptionRepository, EfPrescriptionRepository>();
builder.Services.AddScoped<ILabOrderRepository, EfLabOrderRepository>();
builder.Services.AddScoped<IRadOrderRepository, EfRadOrderRepository>();
builder.Services.AddScoped<IBillingRepository, EfBillingRepository>();
builder.Services.AddScoped<IDispenseRepository, EfDispenseRepository>();
builder.Services.AddScoped<IDrugInventoryRepository, EfDrugInventoryRepository>();
builder.Services.AddScoped<IAuditLogRepository, EfAuditLogRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IRoleRepository, EfRoleRepository>();

// Register Application services
builder.Services.AddScoped<IPatientApplicationService, PatientApplicationService>();
builder.Services.AddScoped<ICampusApplicationService, CampusApplicationService>();
builder.Services.AddScoped<IDepartmentApplicationService, DepartmentApplicationService>();
builder.Services.AddScoped<IStaffApplicationService, StaffApplicationService>();
builder.Services.AddScoped<IDictionaryApplicationService, DictionaryApplicationService>();
builder.Services.AddScoped<IScheduleApplicationService, ScheduleApplicationService>();
builder.Services.AddScoped<IRegistrationApplicationService, RegistrationApplicationService>();
builder.Services.AddScoped<IEncounterApplicationService, EncounterApplicationService>();
builder.Services.AddScoped<IMedicalRecordApplicationService, MedicalRecordApplicationService>();
builder.Services.AddScoped<IDiagnosisApplicationService, DiagnosisApplicationService>();
builder.Services.AddScoped<IPrescriptionApplicationService, PrescriptionApplicationService>();
builder.Services.AddScoped<ILabOrderApplicationService, LabOrderApplicationService>();
builder.Services.AddScoped<ICashierApplicationService, CashierApplicationService>();
builder.Services.AddScoped<IDispenseApplicationService, DispenseApplicationService>();
builder.Services.AddScoped<IUserRoleApplicationService, UserRoleApplicationService>();
builder.Services.AddScoped<IUserRoleApplicationService, UserRoleApplicationService>();

// Register auth services
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;
var issuer = jwtSettings["Issuer"]!;
var audience = jwtSettings["Audience"]!;
var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

builder.Services.AddSingleton(new JwtTokenService(secretKey, issuer, audience, expirationHours));
builder.Services.AddSingleton<LocalUserStore>();
builder.Services.AddScoped<IAuthenticationService, LocalAuthenticationService>();
builder.Services.AddScoped<IAuthenticationApplicationService, AuthenticationApplicationService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
})
;
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<WeChatHttpClient>();
builder.Services.AddSingleton<PatientNoService>();
builder.Services.AddScoped<IWeChatAuthService, WeChatAuthService>();

// 配置 Serilog 文件日志
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

// 容器部署时 HTTPS 由反向代理（nginx / ingress）终结，后端无需重定向
// 如需直接公网暴露，可取消注释并配置 Kestrel HTTPS 证书
// app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

// 开发环境自动创建数据库表并填充种子数据
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
    db.Database.EnsureCreated();

    // 种子患者数据（仅首次运行）
    if (!db.Patients.Any())
    {
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

app.Run();

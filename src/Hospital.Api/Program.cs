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

// 数据库初始化：启动时尽力执行，失败由后台服务重试，不阻断应用启动（避免 SQL 抖动导致 500.30）
builder.Services.AddSingleton<DatabaseInitializationService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DatabaseInitializationService>());

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

// Register auth services（后台/桌面端 JWT）
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;
var issuer = jwtSettings["Issuer"]!;
var audience = jwtSettings["Audience"]!;
var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

builder.Services.AddSingleton(new JwtTokenService(secretKey, issuer, audience, expirationHours));

// 小程序患者 JWT（独立密钥/签发方/有效期）
var weChatJwt = builder.Configuration.GetSection("WeChat:Jwt");
var patientSecret = weChatJwt["SecretKey"]!;
var patientIssuer = weChatJwt["Issuer"]!;
var patientAudience = weChatJwt["Audience"] ?? "HospitalMiniProgram";
var patientExpirationHours = int.Parse(weChatJwt["AccessTokenExpiryMinutes"] ?? "120") / 60;

builder.Services.AddKeyedSingleton<JwtTokenService>("patient", (_, _) =>
    new JwtTokenService(patientSecret, patientIssuer, patientAudience, patientExpirationHours));

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
        ValidIssuers = new[] { issuer, patientIssuer },
        ValidAudiences = new[] { audience, patientAudience },
        IssuerSigningKeys = new[]
        {
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(patientSecret))
        },
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

// 启动时尽力初始化数据库（建表 + 种子数据）；失败不阻断启动，由 DatabaseInitializationService 后台重试
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
    await dbInitializer.EnsureDatabaseReadyAsync();
}

app.Run();

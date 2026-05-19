using System.Reflection;
using System.Text;
using Hospital.Api.Middleware;
using Hospital.Application.Services;
using Hospital.Application.Repositories;
using Hospital.Infrastructure.Repositories;
using Hospital.Infrastructure.Repositories.Ef;
using Hospital.Infrastructure.ExternalServices;
using Hospital.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Register EF Core DbContext
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("HospitalDb"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(3)));

// Add services to the container.
builder.Services.AddControllers();
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
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 容器部署时 HTTPS 由反向代理（nginx / ingress）终结，后端无需重定向
// 如需直接公网暴露，可取消注释并配置 Kestrel HTTPS 证书
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();

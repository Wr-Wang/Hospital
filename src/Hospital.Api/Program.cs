using System.Text;
using Hospital.Application.Services;
using Hospital.Application.Repositories;
using Hospital.Infrastructure.Repositories;
using Hospital.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DDD services
builder.Services.AddSingleton<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientApplicationService, PatientApplicationService>();
builder.Services.AddSingleton<ICampusRepository, CampusRepository>();
builder.Services.AddScoped<ICampusApplicationService, CampusApplicationService>();
builder.Services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentApplicationService, DepartmentApplicationService>();
builder.Services.AddSingleton<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffApplicationService, StaffApplicationService>();
builder.Services.AddSingleton<IDictionaryRepository, DictionaryRepository>();
builder.Services.AddScoped<IDictionaryApplicationService, DictionaryApplicationService>();

// Register Schedule and Registration services
builder.Services.AddSingleton<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IScheduleApplicationService, ScheduleApplicationService>();
builder.Services.AddSingleton<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddSingleton<IEncounterRepository, EncounterRepository>();
builder.Services.AddScoped<IRegistrationApplicationService, RegistrationApplicationService>();

// Register Encounter, MedicalRecord, Diagnosis, Prescription, LabOrder services
builder.Services.AddScoped<IEncounterApplicationService, EncounterApplicationService>();
builder.Services.AddSingleton<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IMedicalRecordApplicationService, MedicalRecordApplicationService>();
builder.Services.AddSingleton<IDiagnosisRepository, DiagnosisRepository>();
builder.Services.AddScoped<IDiagnosisApplicationService, DiagnosisApplicationService>();
builder.Services.AddSingleton<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IPrescriptionApplicationService, PrescriptionApplicationService>();
builder.Services.AddSingleton<ILabOrderRepository, LabOrderRepository>();
builder.Services.AddScoped<ILabOrderApplicationService, LabOrderApplicationService>();
builder.Services.AddSingleton<IRadOrderRepository, RadOrderRepository>();

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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

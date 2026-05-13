using Hospital.Application.Services;
using Hospital.Application.Repositories;
using Hospital.Infrastructure.Repositories;
using Hospital.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DDD services
builder.Services.AddSingleton<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientApplicationService, PatientApplicationService>();
builder.Services.AddScoped<IAuthenticationService, LocalAuthenticationService>();
builder.Services.AddScoped<IAuthenticationApplicationService, AuthenticationApplicationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

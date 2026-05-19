using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.Aggregates.Schedule;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    // Master Data
    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Staff> Staffs => Set<Staff>();
    public DbSet<DictionaryType> DictionaryTypes => Set<DictionaryType>();
    public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>();

    // Security
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    // Patient
    public DbSet<Patient> Patients => Set<Patient>();

    // Scheduling
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Registration> Registrations => Set<Registration>();

    // Clinical
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<LabOrder> LabOrders => Set<LabOrder>();
    public DbSet<RadOrder> RadOrders => Set<RadOrder>();

    // Finance & Pharmacy
    public DbSet<Billing> Billings => Set<Billing>();
    public DbSet<Dispensing> Dispensings => Set<Dispensing>();
    public DbSet<DrugInventory> DrugInventories => Set<DrugInventory>();

    // Audit
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalDbContext).Assembly);
    }
}

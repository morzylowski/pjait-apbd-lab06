using App.Models;

namespace App.Data;

using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Prescription_Medicament> Prescription_Medicaments { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prescription_Medicament>()
            .HasKey(pm => new { pm.IdMedicament, pm.IdPrescription });

        modelBuilder.Entity<Prescription_Medicament>()
            .HasOne(pm => pm.Medicament)
            .WithMany(m => m.Prescription_Medicaments)
            .HasForeignKey(pm => pm.IdMedicament);

        modelBuilder.Entity<Prescription_Medicament>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.Prescription_Medicaments)
            .HasForeignKey(pm => pm.IdPrescription);

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Patient)
            .WithMany(pat => pat.Prescriptions)
            .HasForeignKey(p => p.IdPatient);

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Doctor)
            .WithMany(d => d.Prescriptions)
            .HasForeignKey(p => p.IdDoctor);
    }
}
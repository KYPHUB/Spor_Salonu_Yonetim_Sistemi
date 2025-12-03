using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Web.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Gym> Gyms { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Service>()
            .Property(s => s.Price)
            .HasColumnType("decimal(18,2)");

        // CRITICAL FIX: Configure Many-to-Many relationship between Trainer and Service
        // This replaces the incorrect One-to-Many relationship from the original migration
        builder.Entity<Trainer>()
            .HasMany(t => t.Specialties)
            .WithMany()
            .UsingEntity(j => j.ToTable("ServiceTrainer"));
    }
}

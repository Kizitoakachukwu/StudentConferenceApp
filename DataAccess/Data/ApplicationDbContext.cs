using DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentConferenceApp.DAL.Entities;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Section> Sections { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Manager ↔ Participant (1:1)
        modelBuilder.Entity<Manager>()
            .HasOne(m => m.Participant)
            .WithOne(p => p.Manager)
            .HasForeignKey<Manager>(m => m.ParticipantId);

        modelBuilder.Entity<Manager>()
            .Property(m => m.LinkedUserId)
            .HasMaxLength(450);
        modelBuilder.Entity<Manager>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(m => m.LinkedUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // ApplicationUser ↔ Participant (1:1)
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Participant)
            .WithOne(p => p.ApplicationUser)
            .HasForeignKey<ApplicationUser>(u => u.ParticipantId)
            .IsRequired(false);
    }
}

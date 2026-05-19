using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<GeneratedDocument> GeneratedDocuments { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships here
        modelBuilder.Entity<Manager>()
            .HasOne(m => m.Participant)
            .WithOne(p => p.Manager)
            .HasForeignKey<Manager>(m => m.ParticipantId);
    }
}
    
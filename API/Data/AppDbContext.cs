using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Compound> Compounds => Set<Compound>();
    public DbSet<Cycle> Cycles => Set<Cycle>();
    public DbSet<DoseEvent> DoseEvents => Set<DoseEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cycle <-> DoseEvent (1-to-many)
        modelBuilder.Entity<DoseEvent>()
            .HasOne(d => d.Cycle)
            .WithMany(c => c.DoseEvents)
            .HasForeignKey(d => d.CycleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Compound <-> DoseEvent (1-to-many)
        modelBuilder.Entity<DoseEvent>()
            .HasOne(d => d.Compound)
            .WithMany(c => c.DoseEvents)
            .HasForeignKey(d => d.CompoundId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

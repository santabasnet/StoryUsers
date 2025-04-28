using Microsoft.EntityFrameworkCore;
using StoryUsers.Model;

namespace StoryUsers.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<StoryUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoryUser>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Make Email unique
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Email)
                  .IsRequired()
                  .HasMaxLength(64);

            entity.Property(e => e.Password)
                  .IsRequired()
                  .HasMaxLength(64);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(128);
        });
    }
}
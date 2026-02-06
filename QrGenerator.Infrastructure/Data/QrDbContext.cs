using Microsoft.EntityFrameworkCore;
using QrGenerator.Domain.Entities;

namespace QrGenerator.Infrastructure.Data;

public class QrDbContext : DbContext
{
    public QrDbContext(DbContextOptions<QrDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(e => e.PhoneNumber)
                .IsUnique();
        });
    }
}


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
    public DbSet<QrCode> QrCodes => Set<QrCode>();

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

        modelBuilder.Entity<QrCode>(entity =>
        {
            entity.HasKey(e => e.QrCodeId);

            entity.Property(e => e.DoorLockPassword)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.QrCodeData)
                .IsRequired();

            entity.Property(e => e.QrCodeImageBase64)
                .IsRequired();

            entity.Property(e => e.DataType)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}


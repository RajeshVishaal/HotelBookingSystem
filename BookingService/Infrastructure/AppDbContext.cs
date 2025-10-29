using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingRoomInfo> BookingRoomInfo { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.BookingReference)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(64)
                .IsRequired();

            entity.HasIndex(e => e.IdempotencyKey)
                .IsUnique();

            entity.HasMany(e => e.Rooms)
                .WithOne(r => r.Booking)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookingRoomInfo>(entity =>
        {
            entity.Property(e => e.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.RoomCategoryId)
                .HasColumnType("uuid")
                .IsRequired();
        });
    }
}
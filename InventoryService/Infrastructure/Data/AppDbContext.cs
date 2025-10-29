using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<RoomCategory> RoomCategories => Set<RoomCategory>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<RoomPricing> RoomPricings => Set<RoomPricing>();
    public DbSet<SeasonalRate> SeasonalRates => Set<SeasonalRate>();
    public DbSet<RoomBedConfig> RoomBedConfigs => Set<RoomBedConfig>();
    public DbSet<BedType> BedTypes => Set<BedType>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<RoomAvailability> RoomAvailabilities => Set<RoomAvailability>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureHotel(modelBuilder);
        ConfigureRoomCategory(modelBuilder);
        ConfigureRoomPricing(modelBuilder);
        ConfigureSeasonalRate(modelBuilder);
        ConfigureRoomBedConfig(modelBuilder);
        ConfigureRoomAvailability(modelBuilder);
        ConfigureReview(modelBuilder);
        ConfigureFacility(modelBuilder);
        ConfigureRoomType(modelBuilder);
        ConfigureBedType(modelBuilder);
        ConfigureFacilityRelations(modelBuilder);
    }

    private static void ConfigureHotel(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Hotel>();

        entity.HasKey(h => h.Id);
        entity.Property(h => h.Id).HasDefaultValueSql("gen_random_uuid()");

        entity.Property(h => h.Name).IsRequired().HasMaxLength(255);
        entity.Property(h => h.City).IsRequired().HasMaxLength(100);
        entity.Property(h => h.Country).IsRequired().HasMaxLength(100);
        entity.Property(h => h.AddressLine).HasMaxLength(255);
        entity.Property(h => h.PostalCode).HasMaxLength(20);
        entity.Property(h => h.AverageRating).HasPrecision(3, 1);
        entity.Property(h => h.ImageUrl).HasMaxLength(500);

        entity.HasMany(h => h.Rooms)
            .WithOne(rc => rc.Hotel)
            .HasForeignKey(rc => rc.HotelId)
            .OnDelete(DeleteBehavior.Cascade);


        entity.HasMany(h => h.Reviews)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);


        entity.HasIndex(h => h.Name).HasDatabaseName("IX_Hotels_Name");
        entity.HasIndex(h => new { h.City, h.Country }).HasDatabaseName("IX_Hotels_City_Country");
        entity.HasIndex(h => h.AverageRating).HasDatabaseName("IX_Hotels_AvgRating");
    }

    private static void ConfigureRoomCategory(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<RoomCategory>();

        entity.HasKey(rc => rc.Id);
        entity.Property(rc => rc.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.HasOne(rc => rc.RoomType)
            .WithMany()
            .HasForeignKey(rc => rc.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(rc => rc.BedConfigs)
            .WithOne(bc => bc.RoomCategory)
            .HasForeignKey(bc => bc.RoomCategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(rc => new { rc.HotelId, rc.RoomTypeId }).HasDatabaseName("IX_RoomCategories_Hotel_RoomType");
        entity.HasIndex(rc => rc.HotelId).HasDatabaseName("IX_RoomCategories_HotelId");
    }

    private static void ConfigureRoomPricing(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<RoomPricing>();

        entity.HasKey(rp => rp.Id);
        entity.Property(rp => rp.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.Property(rp => rp.BaseRate).HasPrecision(10, 2);

        entity.HasOne(rp => rp.RoomCategory)
            .WithOne(rc => rc.Pricing)
            .HasForeignKey<RoomPricing>(rp => rp.RoomCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(rp => rp.SeasonalRates)
            .WithOne(sr => sr.RoomPricing)
            .HasForeignKey(sr => sr.RoomPricingId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(rp => rp.RoomCategoryId).HasDatabaseName("IX_RoomPricings_Category");
        entity.HasIndex(rp => rp.BaseRate).HasDatabaseName("IX_RoomPricings_BaseRate");
    }

    private static void ConfigureSeasonalRate(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<SeasonalRate>();

        entity.HasKey(sr => sr.Id);
        entity.Property(sr => sr.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.Property(sr => sr.Rate).HasPrecision(10, 2);

        entity.HasIndex(sr => new { sr.RoomPricingId, sr.StartDate, sr.EndDate })
            .HasDatabaseName("IX_SeasonalRates_DateRange");
    }

    private static void ConfigureRoomBedConfig(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<RoomBedConfig>();

        entity.HasKey(bc => bc.Id);
        entity.Property(bc => bc.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.HasOne(bc => bc.BedType)
            .WithMany()
            .HasForeignKey(bc => bc.BedTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(bc => bc.BedTypeId).HasDatabaseName("IX_RoomBedConfigs_BedType");
    }

    private static void ConfigureRoomAvailability(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<RoomAvailability>();

        entity.HasKey(ra => ra.Id);
        entity.Property(ra => ra.Id).HasDefaultValueSql("gen_random_uuid()");

        entity.HasOne(ra => ra.Hotel)
            .WithMany()
            .HasForeignKey(ra => ra.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(ra => ra.RowVersion)
            .IsRowVersion();

        entity.HasOne(ra => ra.RoomCategory)
            .WithMany()
            .HasForeignKey(ra => ra.RoomCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // entity
        //     .HasIndex(ra => new { ra.RoomCategoryId, ra.Date })
        //     .IsUnique();

        entity.HasIndex(ra => new { ra.HotelId, ra.Date })
            .HasDatabaseName("IX_RoomAvailability_Hotel_Date");

        entity.HasIndex(ra => new { ra.HotelId, ra.RoomCategoryId, ra.Date })
            .HasDatabaseName("IX_RoomAvailability_Hotel_Category_Date");
    }

    private static void ConfigureReview(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Review>();

        entity.HasKey(r => r.Id);
        entity.Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.Property(r => r.Rating).HasPrecision(3, 1);
        entity.Property(r => r.Comment).HasMaxLength(1000).IsRequired();

        entity.HasOne(r => r.Hotel)
            .WithMany(h => h.Reviews)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(r => r.HotelId).HasDatabaseName("IX_Reviews_HotelId");
        entity.HasIndex(r => r.Rating).HasDatabaseName("IX_Reviews_Rating");
        entity.HasIndex(r => r.UserId).HasDatabaseName("IX_Reviews_UserId");
    }

    private static void ConfigureFacility(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Facility>();

        entity.HasKey(f => f.Id);
        entity.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.Property(f => f.Name).IsRequired().HasMaxLength(100);

        entity.HasIndex(f => f.Name).HasDatabaseName("IX_Facilities_Name");
    }

    private static void ConfigureRoomType(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<RoomType>();

        entity.HasKey(rt => rt.Id);
        entity.Property(rt => rt.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.Property(rt => rt.Name).IsRequired().HasMaxLength(100);

        entity.HasIndex(rt => rt.Name).HasDatabaseName("IX_RoomTypes_Name");
    }

    private static void ConfigureBedType(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<BedType>();

        entity.HasKey(bt => bt.Id);
        entity.Property(bt => bt.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.Property(bt => bt.Name).IsRequired().HasMaxLength(50);

        entity.HasIndex(bt => bt.Name).HasDatabaseName("IX_BedTypes_Name");
    }

    private static void ConfigureFacilityRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>()
            .HasMany(h => h.HotelFacilities)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "HotelFacilities",
                right => right.HasOne<Facility>()
                    .WithMany()
                    .HasForeignKey("FacilityId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Hotel>()
                    .WithMany()
                    .HasForeignKey("HotelId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("HotelId", "FacilityId");
                    join.ToTable("HotelFacilities");
                });

        modelBuilder.Entity<RoomCategory>()
            .HasMany(rc => rc.RoomFacilities)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "RoomCategoryFacilities",
                right => right.HasOne<Facility>()
                    .WithMany()
                    .HasForeignKey("FacilityId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<RoomCategory>()
                    .WithMany()
                    .HasForeignKey("RoomCategoryId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("RoomCategoryId", "FacilityId");
                    join.ToTable("RoomCategoryFacilities");
                });
    }
}
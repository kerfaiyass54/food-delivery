using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Data;

public sealed class FoodDeliveryDbContext : DbContext
{
    public FoodDeliveryDbContext(DbContextOptions<FoodDeliveryDbContext> options) : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Chef> Chefs => Set<Chef>();
    public DbSet<Candidature> Candidatures => Set<Candidature>();
    public DbSet<Day> Days => Set<Day>();
    public DbSet<RestaurantOperatingDay> RestaurantOperatingDays => Set<RestaurantOperatingDay>();
    public DbSet<RestaurantOffDay> RestaurantOffDays => Set<RestaurantOffDay>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.ToTable("Restaurants");
            entity.HasCheckConstraint("CK_Restaurants_Capacity_Positive", "\"Capacity\" > 0");
            entity.HasIndex(x => x.OwnerId).IsUnique();
            entity.Property(x => x.Budget).HasPrecision(18, 2);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Address).IsRequired().HasMaxLength(300);
            entity.HasOne(x => x.Owner)
                .WithOne(x => x.Restaurant)
                .HasForeignKey<Restaurant>(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(254);
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Chef>(entity =>
        {
            entity.Property(x => x.Salary).HasPrecision(18, 2);
            entity.HasOne(x => x.Restaurant)
                .WithMany(x => x.Chefs)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Candidature>(entity =>
        {
            entity.Property(x => x.Salary).HasPrecision(18, 2);
            entity.HasOne(x => x.Chef)
                .WithMany(x => x.Candidatures)
                .HasForeignKey(x => x.ChefId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Restaurant)
                .WithMany(x => x.Candidatures)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.ChefId, x.RestaurantId });
        });

        modelBuilder.Entity<Day>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(20);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.HasData(
                new Day { Id = 1, Name = "Monday" },
                new Day { Id = 2, Name = "Tuesday" },
                new Day { Id = 3, Name = "Wednesday" },
                new Day { Id = 4, Name = "Thursday" },
                new Day { Id = 5, Name = "Friday" },
                new Day { Id = 6, Name = "Saturday" },
                new Day { Id = 7, Name = "Sunday" });
        });

        modelBuilder.Entity<RestaurantOperatingDay>(entity =>
        {
            entity.HasKey(x => new { x.RestaurantId, x.DayId });
            entity.HasOne(x => x.Restaurant).WithMany(x => x.OperatingDays)
                .HasForeignKey(x => x.RestaurantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Day).WithMany(x => x.OperatingRestaurants)
                .HasForeignKey(x => x.DayId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RestaurantOffDay>(entity =>
        {
            entity.HasKey(x => new { x.RestaurantId, x.DayId });
            entity.HasOne(x => x.Restaurant).WithMany(x => x.OffDays)
                .HasForeignKey(x => x.RestaurantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Day).WithMany(x => x.RestaurantsOff)
                .HasForeignKey(x => x.DayId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}

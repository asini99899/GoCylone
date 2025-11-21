using GoCylone.Models;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Data
{
    public class GoCyloneDbContext : DbContext
    {
        public GoCyloneDbContext(DbContextOptions<GoCyloneDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<BusFare> BusFares { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<PaymentInfo> PaymentInfos { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Bus configuration
            modelBuilder.Entity<Bus>(entity =>
            {
                entity.HasKey(e => e.BusId);
                entity.Property(e => e.NumberPlate).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ConductorNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Condition).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.NumberPlate).IsUnique();
            });

            // Route configuration
            modelBuilder.Entity<Models.Route>(entity =>
            {
                entity.HasKey(e => e.RouteId);
                entity.Property(e => e.FromLocation).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ToLocation).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Distance).HasPrecision(10, 2);
            });

            // Schedule configuration
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId);
                entity.HasOne(e => e.Bus)
                    .WithMany(b => b.Schedules)
                    .HasForeignKey(e => e.BusId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Route)
                    .WithMany(r => r.Schedules)
                    .HasForeignKey(e => e.RouteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BusFare configuration
            modelBuilder.Entity<BusFare>(entity =>
            {
                entity.HasKey(e => e.FareId);
                entity.Property(e => e.FarePerKm).HasColumnType("decimal(10, 2)");
            });

            // Booking configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingId);
                entity.Property(e => e.TotalFare).HasColumnType("decimal(10, 2)");
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Schedule)
                    .WithMany()
                    .HasForeignKey(e => e.ScheduleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BookingSeat configuration
            modelBuilder.Entity<BookingSeat>(entity =>
            {
                entity.HasKey(e => e.BookingSeatId);
                entity.HasOne(e => e.Booking)
                    .WithMany(b => b.BookingSeats)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Schedule)
                    .WithMany()
                    .HasForeignKey(e => e.ScheduleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PaymentInfo configuration
            modelBuilder.Entity<PaymentInfo>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
                entity.HasOne(e => e.Booking)
                    .WithOne(b => b.Payment)
                    .HasForeignKey<PaymentInfo>(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Review configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Comment).HasMaxLength(2000);
                entity.Property(e => e.Stars).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.CreatedAt).IsDescending();
            });
        }
    }
}

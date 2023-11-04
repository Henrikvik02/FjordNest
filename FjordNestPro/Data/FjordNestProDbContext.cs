using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FjordNestPro.Models;

public class FjordNestProDbContext : IdentityDbContext<ApplicationUser>
{
    public FjordNestProDbContext(DbContextOptions<FjordNestProDbContext> options)
        : base(options)
    {
        // This can be used to ensure the database is created, but is commented out for now
        //Database.EnsureCreated();
    }

    // DbSet properties represent tables in the database
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Configure identity-related tables first

        // Relationship between Property and its owner (ApplicationUser)
        modelBuilder.Entity<Property>()
            .HasOne(p => p.User)
            .WithMany(u => u.Properties)
            .HasForeignKey(p => p.OwnerID)
            .OnDelete(DeleteBehavior.Cascade); // When an ApplicationUser is deleted, the associated Properties are also deleted

        // Relationship between Property and Booking with Cascade Delete
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Property)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PropertyID)
            .OnDelete(DeleteBehavior.Cascade); // When a Property is deleted, the associated Bookings are also deleted

        // Relationship between Booking and User (ApplicationUser)
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.GuestID)
            .OnDelete(DeleteBehavior.Cascade); // When an ApplicationUser is deleted, the associated Bookings are also deleted

        // Relationship between Review and User (ApplicationUser)
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.GuestID)
            .OnDelete(DeleteBehavior.Cascade); // When an ApplicationUser is deleted, the associated Reviews are also deleted

        // Relationship between Review and Booking
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Booking)
            .WithOne(b => b.Review)
            .HasForeignKey<Review>(r => r.BookingID)
            .OnDelete(DeleteBehavior.Cascade); // When a Booking is deleted, the associated Review is also deleted

        // Relationship between Property and Address
        modelBuilder.Entity<Property>()
            .HasOne(p => p.Address)
            .WithMany(a => a.Properties)
            .HasForeignKey(p => p.AddressID)
            .OnDelete(DeleteBehavior.Cascade); // When an Address is deleted, the associated Properties are also deleted
    }
}

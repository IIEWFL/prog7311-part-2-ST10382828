using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Prog7311_Part2.Models
{
    // Repository Pattern Implementation:
    // AppDbContext serves as a form of the Repository pattern by providing
    // a centralized data access layer through DbSet properties.
    // This aligns with the Repository pattern mentioned in the report
    // by abstracting data access operations from the rest of the application.
    
    /// <summary>
    /// Application database context for Entity Framework Core
    /// Implements data access layer with relationship configuration and seeding
    /// 
    /// DbContext implementation pattern based on: Microsoft Corporation (2024) 'DbContext Lifetime, Configuration, and Initialization',
    ///12 November 2024.[online] Available at: https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/
    /// (Accessed: 12 May 2025).
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Farmer)
                .WithMany(f => f.Products)
                .HasForeignKey(p => p.FarmerId);

            // Initial data seeding for bootstrap admin user
            // Data seeding approach adapted from: Microsoft Corporation (2025) 'Data Seeding in Entity Framework Core',
            //.15 January 2025.[online] Available at: https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding
            // (Accessed: 13 May 2025).
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@agrienergy.com",
                    Role = UserRole.Employee,
                    CreatedAt = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
            
            // Seed initial employee record
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Admin User",
                    Department = "Management",
                    EmployeeNumber = "EMP001",
                    UserId = 1,
                    CreatedAt = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
} 
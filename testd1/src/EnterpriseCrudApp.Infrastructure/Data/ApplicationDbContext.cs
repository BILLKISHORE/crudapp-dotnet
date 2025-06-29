using Microsoft.EntityFrameworkCore;
using EnterpriseCrudApp.Domain.Entities;

namespace EnterpriseCrudApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee entity configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Salary)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.HireDate)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt);

                // Computed column for FullName (not persisted)
                entity.Ignore(e => e.FullName);

                // Indexes for performance (simplified for InMemory database)
                entity.HasIndex(e => e.Department);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedEmployees = new[]
            {
                new Employee
                {
                    Id = 1,
                    FirstName = "Bill",
                    LastName = "Kishore",
                    Email = "bill.kishore@company.com",
                    PhoneNumber = "+91-9876543210",
                    Department = "Engineering",
                    Position = "Senior Software Engineer",
                    Salary = 95000m,
                    HireDate = new DateTime(2022, 3, 15),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-730)
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Kishore",
                    LastName = "Kumar",
                    Email = "kishore.kumar@company.com",
                    PhoneNumber = "+91-9876543211",
                    Department = "Marketing",
                    Position = "Marketing Manager",
                    Salary = 75000m,
                    HireDate = new DateTime(2021, 8, 20),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-1000)
                },
                new Employee
                {
                    Id = 3,
                    FirstName = "Manoj",
                    LastName = "Sharma",
                    Email = "manoj.sharma@company.com",
                    PhoneNumber = "+91-9876543212",
                    Department = "Finance",
                    Position = "Financial Analyst",
                    Salary = 65000m,
                    HireDate = new DateTime(2023, 1, 10),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-365)
                }
            };

            modelBuilder.Entity<Employee>().HasData(seedEmployees);
        }
    }
} 
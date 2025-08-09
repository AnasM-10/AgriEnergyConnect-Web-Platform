
using AgriEnergyConnect2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AgriEnergyConnect2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee to Farmer Relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Farmer)
                .WithMany(f => f.Employees)
                .HasForeignKey(e => e.FarmerID)
                .IsRequired(false);

            // Product to Farmer Relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Farmer)
                .WithMany(f => f.Products)
                .HasForeignKey(p => p.FarmerID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between Employee and IdentityUser
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.IdentityUser)
                .WithOne() 
                .HasForeignKey<Employee>(e => e.IdentityUserId)
                .IsRequired() 
                .OnDelete(DeleteBehavior.Cascade); 

            // Seed Roles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "role1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "role2", Name = "Farmer", NormalizedName = "FARMER" },
                new IdentityRole { Id = "role3", Name = "Employee", NormalizedName = "EMPLOYEE" }
            );

            // Seed Farmers
            modelBuilder.Entity<Farmer>().HasData(
                new Farmer
                {
                    FarmerID = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    ContactNumber = "1234567890",
                    Email = "john@example.com",
                    Address = "123 Farm Lane",
                    RegistrationDate = new DateTime(2024, 1, 10)
                },
                new Farmer
                {
                    FarmerID = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    ContactNumber = "0987654321",
                    Email = "jane@example.com",
                    Address = "456 Field Road",
                    RegistrationDate = new DateTime(2024, 2, 15)
                }
            );

            // Seed Products (linking to Farmers by FarmerID)
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductID = 1,
                    FarmerID = 1, // Linked to John Doe
                    Name = "Corn",
                    Category = "Grains",
                    ProductionDate = new DateTime(2024, 3, 1),
                    Description = "Fresh corn from John's farm.",
                    AddedDate = new DateTime(2024, 3, 5)
                },
                new Product
                {
                    ProductID = 2,
                    FarmerID = 2, // Linked to Jane Smith
                    Name = "Milk",
                    Category = "Dairy",
                    ProductionDate = new DateTime(2024, 3, 2),
                    Description = "Organic milk from Jane.",
                    AddedDate = new DateTime(2024, 3, 6)
                }
            );

        }
    }
}
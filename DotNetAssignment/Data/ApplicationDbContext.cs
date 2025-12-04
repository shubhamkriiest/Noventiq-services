using Microsoft.EntityFrameworkCore;
using DotNetAssignment.Models;

namespace DotNetAssignment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between User and Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)           // Each user has one role
                .WithMany(r => r.Users)         // Each role has many users
                .HasForeignKey(u => u.RoleId)   // Foreign key is RoleId
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting role if users exist

            // Seed initial data - these roles will be created when database is created
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Administrator with full access" },
                new Role { Id = 2, Name = "User", Description = "Regular user with limited access" }
            );
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
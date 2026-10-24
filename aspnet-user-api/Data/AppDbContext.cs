using AspNetUserApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetUserApi.Data
{
    /// <summary>
    /// Entity Framework Core database context configured for SQLite.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity constraints
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                // Enforce uniqueness constraint on email address
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.LastName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                      .IsRequired();
            });
        }
    }
}

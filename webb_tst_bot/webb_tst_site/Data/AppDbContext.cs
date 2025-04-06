using Microsoft.EntityFrameworkCore;
using webb_tst_site.Models;

namespace webb_tst_site.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Sphere> Spheres { get; set; }
        public DbSet<Rune> Runes { get; set; }
        public DbSet<RuneSphereDescription> RuneSphereDescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка составного ключа для описаний рун по сферам
            modelBuilder.Entity<RuneSphereDescription>()
                .HasKey(rsd => new { rsd.RuneId, rsd.SphereId });

            // Инициализация начальных данных
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    // Хеш для пароля "admin123"
                    PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                });

            modelBuilder.Entity<RuneSphereDescription>()
    .HasOne(rsd => rsd.Rune)
    .WithMany(r => r.SphereDescriptions)
    .HasForeignKey(rsd => rsd.RuneId)
    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
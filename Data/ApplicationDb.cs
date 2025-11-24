using Microsoft.EntityFrameworkCore;
using Bookify.Models;

namespace Bookify.Data
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<LivreLu> LivreLu { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rendre toutes les chaînes non nullables par défaut
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (!property.IsPrimaryKey() && property.ClrType == typeof(string))
                    {
                        property.IsNullable = false;
                    }
                }
            }
        }
    }
}

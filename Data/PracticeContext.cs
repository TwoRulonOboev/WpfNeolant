using Microsoft.EntityFrameworkCore;
using WpfNeolant.Model;
using WpfNeolant.Utils;

namespace WpfNeolant.Data
{
    class PracticeContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = ConfigUtility.Config["postgresConnectionString"];

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}

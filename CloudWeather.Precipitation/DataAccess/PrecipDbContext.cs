using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Precipitation.DataAccess
{
    public class PrecipDbContext : DbContext
    {
        public PrecipDbContext() { }
        public PrecipDbContext(DbContextOptions opts) : base(opts) { }

        public DbSet<Precipitation> Precipitation { get; set; }

        //Calling this before creating the table to apply snakecase identity
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }
        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Precipitation>(b => { b.ToTable("precipitation"); });
        }
    }
}

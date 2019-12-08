
using Microsoft.EntityFrameworkCore;

namespace Demo.Statistics.Infrastructure.AppContext
{
    public partial class StatisticsAppDbContext : DbContext
    {
        public StatisticsAppDbContext(DbContextOptions<StatisticsAppDbContext> options) : base(options) { }

        public DbSet<Statistics> Statistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.IsSqlServer())
            {
                //modelBuilder.ApplyAllConfigurationsForStatisticsAppDbContext();

                base.OnModelCreating(modelBuilder);
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using FrontendMonitoring.Models;

namespace FrontendMonitoring.Data
{
    public class FrontendMonitoringContext : DbContext
    {
        public FrontendMonitoringContext(DbContextOptions<FrontendMonitoringContext> options)
            : base(options)
        {
        }

        public DbSet<AfvalModel> Afval { get; set; }
        // Add other DbSets here as needed
    }
}

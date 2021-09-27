using Microsoft.EntityFrameworkCore;

namespace LogExceptions_CustomMiddleware.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Logs> Logs { get; set; }
    }
}

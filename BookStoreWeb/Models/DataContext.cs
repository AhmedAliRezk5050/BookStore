using Microsoft.EntityFrameworkCore;

namespace BookStoreWeb.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts)
       : base(opts) { }

        public DbSet<Category> Categories { get; set; } = null!;
    }
}

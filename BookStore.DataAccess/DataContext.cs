using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts)
       : base(opts) { }

        public DbSet<Category> Categories { get; set; } = null!;
        
        public DbSet<CoverType> CoverTypes { get; set; } = null!;
    }
}

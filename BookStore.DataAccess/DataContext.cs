using BookStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> opts)
       : base(opts) { }

        public DbSet<Category> Categories { get; set; } = null!;
        
        public DbSet<CoverType> CoverTypes { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<Company> Companies { get; set; } = null!;

        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
    }
}

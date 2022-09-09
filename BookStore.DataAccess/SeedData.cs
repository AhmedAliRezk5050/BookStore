using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess
{
    public static class SeedData
    {
        public static void SeedDatabase(DataContext context)
        {
            context.Database.Migrate();

            if (!context.Categories.Any())
            {
                var ctg1 = new Category()
                {
                    Name = "IT",
                    DisplayOrder = 1,
                    CreatedDateTime = DateTime.Now
                };

                var ctg2 = new Category()
                {
                    Name = "Science",
                    DisplayOrder = 2,
                    CreatedDateTime = DateTime.Now
                };

                var ctg3 = new Category()
                {
                    Name = "Art",
                    DisplayOrder = 3,
                    CreatedDateTime = DateTime.Now
                };

                context.Categories.AddRange(ctg1, ctg2, ctg3);
            }

            context.SaveChanges();
        }
    }
}

using BookStoreWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BookStoreWeb;
public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreConnection")));

    builder.Services.AddControllersWithViews();
  
    builder.Services.AddRazorPages();
    
    if (builder.Environment.IsDevelopment())
    {
      builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }
    else
    {
      app.UseExceptionHandler("/Home/Error");
      app.UseHsts();
    }

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    
    app.MapRazorPages();

    SeedDb(app);

    app.Run();
  }

  private static void SeedDb(WebApplication app)
  {
    using (var scope = app.Services.CreateScope())
    {
      var services = scope.ServiceProvider;

      try
      {
        var context = services.GetRequiredService<DataContext>();

        SeedData.SeedDatabase(context);
      }
      catch (Exception ex)
      {
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while seeding the database.");
      }
    }
  }
}





using BookStore.DataAccess;
using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Text.Json.Serialization;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace BookStoreWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAutoMapper(Assembly.Load("BookStore.Models"));

        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        
        builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreConnection"));
                options.EnableSensitiveDataLogging(true);
            }
        );

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IDbInitializer, DbInitializer>();
        
        builder.Services.AddControllersWithViews().AddJsonOptions(options => 
        { 
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        });
        
        builder.Services.AddDistributedMemoryCache();
        
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        {
        }).AddRoles<IdentityRole>().AddEntityFrameworkStores<DataContext>();

        builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
        {
            facebookOptions.AppId = builder.Configuration.GetSection("Facebook:AppId").Get<string>();
            facebookOptions.AppSecret = builder.Configuration.GetSection("Facebook:AppSecret").Get<string>();
        });
        
        builder.Services.Configure<GoogleReCaptchaSettings>(builder.Configuration.GetSection("GoogleReCaptchaSettings"));
        
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

        app.UseHttpsRedirection();
        
        app.UseStaticFiles();

        app.UseRouting();

        StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:Secretkey").Get<string>();

        app.UseAuthentication();

        app.UseAuthorization();
        
        app.UseSession();
        
        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

        SeedDb(app).GetAwaiter().GetResult();

        app.Run();
    }

    private static async Task SeedDb(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await dbInitializer.Initialize();
        }
    }
}


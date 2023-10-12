using net_il_mio_fotoalbum.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<PhotographerShowcaseContext>();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PhotographerShowcaseContext>();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.AddScoped<DbContext, PhotographerShowcaseContext>();
            builder.Services.AddScoped<PhotoManager, PhotoManager>();
            builder.Services.AddScoped<IRepository<Category>, EntityRepository<Category>>();
            builder.Services.AddScoped<UserManager<IdentityUser>, UserManager<IdentityUser>>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{slug?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
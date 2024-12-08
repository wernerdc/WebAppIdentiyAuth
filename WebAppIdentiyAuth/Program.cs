using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAppIdentiyAuth.Data;
using WebAppIdentiyAuth.Models;
    
namespace WebAppIdentiyAuth
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            // add (custom) identity
            //builder.Services.AddDefaultIdentity<IdentityUser>(options => 
            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
                options.SignIn.RequireConfirmedAccount = false)         // false: while in dev-mode
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // seeding the roles and admin user for initial setup
            using(var scope = app.Services.CreateScope())
            {
                /*
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "Manager", "Member" };
                
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
                */
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    //var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await ContextSeed.SeedRolesAsync(roleManager);
                    await ContextSeed.SeedDefaultAdminAsync(userManager);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            /*
            using (var scope = app.Services.CreateScope())
            {
                //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string email = "admin@admin.de";
                string password = "Test1234,";

                if (await userManager.FindByEmailAsync(email) == null)
                {
                    // create user
                    //var user = new IdentityUser();
                    var user = new ApplicationUser();
                    user.Email = email;
                    user.UserName = email;
                    // workaround if email confirmation is turned on
                    //user.EmailConfirmed = true;

                    // add user to db
                    await userManager.CreateAsync(user, password);
                    // add role to user
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            */
            app.Run();
        }
    }
}

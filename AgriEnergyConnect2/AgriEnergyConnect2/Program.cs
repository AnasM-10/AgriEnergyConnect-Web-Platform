using AgriEnergyConnect2.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Adding DbContext and Identity services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // Add support for roles like 'Farmer', 'Employee'
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Use ApplicationDbContext to store identity data

// Add controllers and views (MVC services)
builder.Services.AddControllersWithViews();

// Add Razor Pages (for Identity UI)
builder.Services.AddRazorPages();

// Set up the application pipeline
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // More detailed error page in development
    app.UseMigrationsEndPoint();    // Enables viewing migration history
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// --- Database Seeding and Migration Application ---
// This block ensures the database is migrated and seeded when the application starts.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Apply any pending migrations

        // Call your custom SeedData for Identity roles and users.
        // This is separate from HasData and crucial for hashing passwords for IdentityUser.
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>(); // Use ILogger from Microsoft.Extensions.Logging
        logger.LogError(ex, "An error occurred while applying migrations or seeding the database.");
    }
}
// --- End Database Seeding and Migration Application ---

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Define MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map routes for Farmer and Employee controllers (if you have specific default actions)
app.MapControllerRoute(
    name: "farmer",
    pattern: "Farmer/{action=Dashboard}/{id?}"); // Assuming Dashboard is the default for Farmer

app.MapControllerRoute(
    name: "employee",
    pattern: "Employee/{action=Dashboard}/{id?}", // Assuming Dashboard is the default for Employee
    defaults: new { controller = "Employee" });

// Map Razor Pages for Identity pages like login, register, etc.
app.MapRazorPages();

app.Run();
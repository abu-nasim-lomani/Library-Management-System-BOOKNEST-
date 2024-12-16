using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookNest.Repositories.Interfaces;
using BookNest.Repositories;
using BookNest.Data;
using BookNest.Models;
using Hangfire;
using Hangfire.SqlServer;
using System;

var builder = WebApplication.CreateBuilder(args);

//  Configure database connection
var connectionString = builder.Configuration.GetConnectionString("BookNestDB")
    ?? throw new InvalidOperationException("Connection string 'BookNestDB' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity and Authentication
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

//  Register repositories for dependency injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookIssueRepository, BookIssueRepository>();
builder.Services.AddScoped<IBookIssueRequestRepository, BookIssueRequestRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ExpiredBookJob>();

//   Hangfire for background jobs
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await SeedRolesAndAdminUser(roleManager, userManager);

    //  Schedule Hangfire job for expired books
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate(
        "CheckExpiredBooks",
        () => scope.ServiceProvider.GetService<ExpiredBookJob>().CheckExpiredBooks(),
        "40 0 * * *" // Runs at 12:40 AM daily BST
    );
}

// Configure middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();

// Configure routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//  Add routes for Book and BookIssue controllers
app.MapControllerRoute(
    name: "book",
    pattern: "{controller=Book}/{action=Index}");

app.MapControllerRoute(
    name: "bookIssue",
    pattern: "{controller=BookIssue}/{action=Last24HoursIssues}");

app.MapRazorPages();

// Run the application
app.Run();

// Method to seed roles and admin user
async Task SeedRolesAndAdminUser(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
{
    // Create "Admin" role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var adminEmail = "Admin@BookNest.com";
    var adminPassword = "@Shumonbd1@";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    //  Create admin user if it doesn't exist
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "Admin User" // Provide a value for FullName
        };
        await userManager.CreateAsync(adminUser, adminPassword);
    }

    // Add the admin user to the "Admin" role
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

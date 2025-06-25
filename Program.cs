using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prog7311_Part2.Data;
using Prog7311_Part2.Models;

var builder = WebApplication.CreateBuilder(args);

// If "seed-data" command is provided, run data seeding and exit
if (args.Length > 0 && args[0] == "seed-data")
{
    var serviceProvider = new ServiceCollection()
        .AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")))
        .AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        })
        .BuildServiceProvider();

    using var scope = serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing database with required data");
    DbInitializer.Initialize(dbContext);
    logger.LogInformation("Database seeding completed");
    
    return;
}

// Logging configuration adapted from: Microsoft Corporation (2024) 'Logging in .NET Core and ASP.NET Core',
// Microsoft Docs, 18 September 2024.[online] Available at: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging
// (Accessed: 12 May 2025).
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency injection for custom services
// Pattern adapted from: Microsoft Corporation (2024) 'Dependency injection in ASP.NET Core',
// Microsoft Docs, 18 September 2024.[online] Available at: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
// (Accessed: 12 May 2025).
builder.Services.AddScoped<Prog7311_Part2.Services.FileUploadService>();

// Observer Pattern Implementation:
// The authentication system below demonstrates aspects of the Observer pattern
// mentioned in the report. Events related to authentication (login/logout)
// trigger handlers and callbacks that respond to these events, similar to
// how observers respond to subjects in the Observer pattern.

// Cookie-based authentication configuration
// Implementation adapted from: Microsoft Corporation (2024) 'Cookie authentication in ASP.NET Core',
// Microsoft Docs, 25 April 2024.[online] Available at: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie
// (Accessed: 12 May 2025).
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("FarmerOnly", policy => policy.RequireClaim("Role", "Farmer"));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("Role", "Employee"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure seed product images exist
await SeedProductImages.EnsureSeedProductImagesExist(app.Environment);

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Initializing database");
        DbInitializer.Initialize(context);
        logger.LogInformation("Database initialization completed");
        
        // SeedMoreData is no longer needed since all products are now seeded in DbInitializer
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

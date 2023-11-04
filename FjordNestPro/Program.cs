using FjordNestPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;
using Serilog.Events;
using FjordNestPro.Data;
using FjordNestPro.Repositories;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Registering MVC controllers and views with the dependency injection container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Registering the database context with the dependency injection container.
builder.Services.AddDbContext<FjordNestProDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:FjordNestProDbContextConnection"]);
});



// Setting up ASP.NET Core Identity.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FjordNestProDbContext>()
    .AddDefaultTokenProviders();


// Dummy implementation of IEmailSender
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information() // levels: Trace< Information < Warning < Erorr < Fatal
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
                            e.Level == LogEventLevel.Information &&
                            e.MessageTemplate.Text.Contains("Executed DbCommand"));

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);
// Register the dummy IEmailSender
builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();



// Registering Razor pages with the dependency injection container.
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options => {
        options.Conventions.AddAreaPageRoute("/Area/Admin", "/Home/Index", "Admin/Home/Index");
    });

// Uncomment if you need this service.
// Registering a custom service (in this case, IPropertyRepository).


// Registering IQuestionRepository and its implementation.
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Path to your login page
    options.LoginPath = "/Identity/Account/Login";
});


var app = builder.Build();

// Seed Data: Initializing the database with seed data.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await SeedData.Initialize(services);
}

// Middleware Configuration.

// Check if the environment is NOT development.
if (!app.Environment.IsDevelopment())
{
    // Handles exceptions and redirects to an error page.
    app.UseExceptionHandler("/Home/Error");
    // Enforces the use of HTTPS.
    app.UseHsts();
}

// Setting up request routing.
app.UseRouting();

// Redirects HTTP requests to HTTPS.
app.UseHttpsRedirection();
// Serves static files (like CSS and JavaScript) from the wwwroot folder.
app.UseStaticFiles();

// Middleware to authenticate users.
app.UseAuthentication();
// Middleware to handle user authorizations (like roles and policies).
app.UseAuthorization();

// Admin area route
app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configuring Razor Pages.
app.MapRazorPages();

// Starts the application.
app.Run();
public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        // Log the message or do nothing
        return Task.CompletedTask;
    }
}

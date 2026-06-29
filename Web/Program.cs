using Data;
using Data.Data;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Formatting.Compact;
using Web;
using Web.Hubs;

var logPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Data", "Logs", "app-.clef");
logPath = Path.GetFullPath(logPath);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error)
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(), logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Ephemeral keys — every restart invalidates all session cookies,
// which is correct since EnsureDeleted wipes the DB on each run.
builder.Services.AddDataProtection()
    .UseEphemeralDataProtectionProvider();

// Add services to the container.
builder.Services
    .AddCoreServices(builder.Configuration)
    .AddIdentity()
    .AddCurrentUserServices()
    .AddGoogleAuth(builder.Configuration)
    .AddLiveMessaggingSignalR(builder.Configuration)
    .BundleAndMinify()
    .AddControllersWithViews();



builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/forbidden";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User" });
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin" });

    var seeder = new DataSeeder(
    scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>(),
    scope.ServiceProvider.GetRequiredService<StadiumStore>(),
    scope.ServiceProvider.GetRequiredService<PlayerStore>(),
    scope.ServiceProvider.GetRequiredService<MatchStore>(),
    scope.ServiceProvider.GetRequiredService<PartyStore>(),
    scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>(),
    app.Environment
);
    await seeder.SeedAsync();
}


// Configure the HTTP request pipeline.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature?.Error is { } ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        if (!app.Environment.IsDevelopment())
        {
            context.Response.Redirect("/Home/Error");
            return;
        }

        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync(exceptionFeature?.Error?.ToString() ?? "An error occurred.");
    });
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseWebOptimizer();

var contentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".avif"] = "image/avif";
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = contentTypeProvider });

app.UseRouting();

app.MapHub<ChatHub>("/hubs/chat");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/home", permanent: true));

app.MapRazorPages();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();

public partial class  Program { }

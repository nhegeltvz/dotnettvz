using Data;
using Microsoft.AspNetCore.Identity;
using Web;
using Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services
    .AddCoreServices(builder.Configuration)
    .AddIdentity()
    .AddGoogleAuth(builder.Configuration)
    .AddLiveMessaggingSignalR(builder.Configuration)
    .AddAiSupport()
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
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User" });
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin" });
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
using Data;
using Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddCoreServices(builder.Configuration)
    .BundleAndMinify()
    .AddControllersWithViews();

var app = builder.Build();



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

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

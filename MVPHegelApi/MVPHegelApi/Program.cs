var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient("RapidApi", (serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["RapidApi:BaseUrl"]);
    client.DefaultRequestHeaders.Add("x-rapidapi-key", config["RapidApi:Key"]);
    client.DefaultRequestHeaders.Add("x-rapidapi-host", config["RapidApi:Host"]);
});

builder.Services.AddSingleton<HnlService>();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run("http://0.0.0.0:5000");
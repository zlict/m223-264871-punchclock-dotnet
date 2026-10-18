using Microsoft.EntityFrameworkCore;
using M223PunchclockDotnet.Service;
using M223PunchclockDotnet.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Punchclock")
        ?? "Host=db;Database=postgres;Username=postgres;Password=postgres",
        // The application uses postgres itself; administer it through a different database.
        postgres => postgres.UseAdminDatabase("template1")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<EntryService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TagService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}


await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DatabaseContext>();
    // Drop-and-create: every application start resets the configured database.
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

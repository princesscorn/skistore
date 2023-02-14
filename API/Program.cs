using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// wwk add the dependency injection of product 
builder.Services.AddDbContext<StoreContext>( opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// wwk add repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// wwk remove it because it will cause some warning later on.
//app.UseHttpsRedirection();

app.UseAuthorization();

/* Then we've got the middleware to map controllers and so our API knows where to send the HTTP requests.
   This piece of middleware is effectively going to register our controller endpoints so that our API as
   an HTTP request comes in, it knows where to send it.
*/
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<StoreContext>();
var logger = services.GetRequiredService<ILogger<Program>>();
try
{
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occured during migration");
}

app.Run();

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using API.Middleware;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// wwk move the services to ApplicationServicesExtensions
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// wwk add error handling before the http request pipeline
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// wwk remove it because it will cause some warning later on.
//app.UseHttpsRedirection();

// wwk use static files
app.UseStaticFiles();

// wwk use Cors Policy
app.UseCors("CorsPolicy");

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

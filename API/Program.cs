using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using API.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using API.Errors;

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
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<ApiBehaviorOptions>(options => 
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var errors = actionContext.ModelState
                            .Where(e => e.Value.Errors.Count > 0)
                            .SelectMany(x => x.Value.Errors)
                            .Select(x => x.ErrorMessage)
                            .ToArray();

            var errorResponse = new ApiValidationErrorResponse 
            {
                Errors = errors
            };

            return new BadRequestObjectResult(errorResponse);
        };
    });

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

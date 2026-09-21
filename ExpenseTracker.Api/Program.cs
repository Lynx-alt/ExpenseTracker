using ExpenseTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ExpenseTrackerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();


var app = builder.Build();

app.UseMiddleware<ExpenseTracker.Api.Middleware.ExceptionHandlingMiddleware>();

//esempio middleware 'light' in program.cs
//app.Use(async (context, next) =>
//{
//    Console.WriteLine($"Richiesta in arrivo: {context.Request.Path}");
//    await next(context); // passa al middleware successivo
//    Console.WriteLine($"Risposta inviata: {context.Response.StatusCode}");
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();


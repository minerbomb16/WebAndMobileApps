using Microsoft.EntityFrameworkCore;
using RealTimeTicketing.Data;
using RealTimeTicketing.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Dodanie bazy danych w pamiêci
builder.Services.AddDbContext<TicketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TicketDbConnection")));


// Dodanie us³ug MVC i SignalR
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware
app.UseCors("AllowReactApp");
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Obs³uga kontrolerów
    endpoints.MapHub<RealTimeHub>("/realtimehub");
});

app.Run();

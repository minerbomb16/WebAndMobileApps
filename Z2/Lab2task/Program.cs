using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Dodaj us³ugi do kontenera
builder.Services.AddControllersWithViews();

// Dodaj us³ugi sesji
builder.Services.AddDistributedMemoryCache(); // U¿ywamy pamiêci w pamiêci jako magazynu sesji
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Czas wygasania sesji
    options.Cookie.HttpOnly = true; // Bezpieczeñstwo ciasteczek
    options.Cookie.IsEssential = true; // Ciasteczka s¹ niezbêdne
});

var app = builder.Build();

// Konfiguracja pipeline HTTP request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// U¿yj sesji
app.UseSession();

app.UseAuthorization();

// Zdefiniuj trasê domyœln¹
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calculator}/{action=Index}/{id?}");

app.Run();

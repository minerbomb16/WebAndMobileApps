var builder = WebApplication.CreateBuilder(args);

// Dodaj MVC
builder.Services.AddControllersWithViews();

// Rejestrujemy HttpClient "ApiClient"
builder.Services.AddHttpClient("ApiClient", client =>
{
    // Tu wstaw URL do Twojego API
    client.BaseAddress = new Uri("https://localhost:7034/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todo}/{action=Index}/{id?}");

app.Run();

using CommunityToolkit.Maui; // <-- do UseMauiCommunityToolkit()
using Microsoft.Extensions.DependencyInjection; // <-- by AddHttpClient działało

namespace TodoListSolution.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            // kluczowe:
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // wstrzykiwanie HttpClient
        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://10.0.2.2:7034");
        });

        return builder.Build();
    }
}

using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using TodoListSolution.Mobile.ViewModels;

namespace TodoListSolution.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Configure HttpClient with BaseAddress
        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://10.0.2.2:7034/"); // Ensure this matches your API's URL
        });

        builder.Services.AddSingleton<MainViewModel>();

        return builder.Build();
    }
}

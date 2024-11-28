using Microsoft.Extensions.Logging;

namespace OnlineStore.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Rejestracja stron w kontenerze DI
            builder.Services.AddSingleton<ProductsPage>();
            builder.Services.AddSingleton<OrdersPage>();
            builder.Services.AddSingleton<CategoriesPage>();

            // Rejestracja AppShell z wymaganymi stronami
            builder.Services.AddSingleton<AppShell>(s =>
            {
                var productsPage = s.GetRequiredService<ProductsPage>();
                var ordersPage = s.GetRequiredService<OrdersPage>();
                var categoriesPage = s.GetRequiredService<CategoriesPage>();
                return new AppShell(productsPage, ordersPage, categoriesPage);
            });

            return builder.Build();
        }
    }
}

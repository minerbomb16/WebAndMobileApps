using aplikacjaMobilna.Services;
using aplikacjaMobilna.ViewModels;
using aplikacjaMobilna.Views;

namespace aplikacjaMobilna
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
                });

            ConfigureServices(builder.Services);

            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureAppServices(services);
            ConfigureViewModels(services);
            ConfigureViews(services);
        }

        private static void ConfigureAppServices(IServiceCollection services)
        {
            services.AddSingleton<ITaskService, TaskService>();
        }

        private static void ConfigureViewModels(IServiceCollection services)
        {
            services.AddTransient<TasksViewModel>();
        }

        private static void ConfigureViews(IServiceCollection services)
        {
            services.AddTransient<TasksPage>();
        }
    }
}

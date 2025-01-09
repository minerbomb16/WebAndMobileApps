using Microsoft.Maui.Controls;

namespace TodoListSolution.Mobile
{
    public partial class App : Application
    {
    public static HttpClient HttpClient { get; private set; }

    public App()
    {
        InitializeComponent();
        HttpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7034") };
        MainPage = new AppShell();
    }
    }
}
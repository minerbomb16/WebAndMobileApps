using aplikacjaMobilna.Views;

namespace aplikacjaMobilna
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = new NavigationPage(serviceProvider.GetRequiredService<TasksPage>());
        }
    }
}

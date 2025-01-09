using Microsoft.Maui.Controls;

namespace TodoListSolution.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("EditTaskPage", typeof(Views.EditTaskPage));
        }
    }
}
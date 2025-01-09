using TodoListSolution.Mobile.Models;
using TodoListSolution.Mobile.ViewModels;

namespace TodoListSolution.Mobile.Views
{
    [QueryProperty(nameof(Task), "Task")]
    public partial class EditTaskPage : ContentPage
    {
        private TodoItemDTO _task;

        public TodoItemDTO Task
        {
            get => _task;
            set
            {
                _task = value;
                Console.WriteLine($"Task set: {value?.Title}");
                if (BindingContext is EditTaskViewModel vm)
                {
                    vm.Initialize(_task);
                }
            }
        }

        public EditTaskPage()
        {
            InitializeComponent();
            var viewModel = new EditTaskViewModel(App.HttpClient);
            BindingContext = viewModel;

            // Subscribe to TaskSaved event
            viewModel.TaskSaved += OnTaskSaved;
        }

        private void OnTaskSaved()
        {
            if (Application.Current.MainPage is AppShell shell)
            {
                if (shell.BindingContext is MainViewModel mainViewModel)
                {
                    mainViewModel.ReloadData(); // Refresh the list
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Console.WriteLine("EditTaskPage appeared.");
        }

        private void OnRefreshClicked(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel vm)
            {
                vm.ReloadData();
            }
        }
    }
}

using TodoListSolution.Mobile.ViewModels;

namespace TodoListSolution.Mobile.Views
{
    [QueryProperty(nameof(TaskId), "taskId")]
    [QueryProperty(nameof(Owner), "owner")]
    public partial class EditTaskPage : ContentPage
    {
        public string TaskId { get; set; }
        public string Owner { get; set; }

        public EditTaskPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is EditTaskViewModel vm)
            {
                // Pass the query parameters to the ViewModel
                if (!string.IsNullOrWhiteSpace(TaskId) && !string.IsNullOrWhiteSpace(Owner))
                {
                    vm.LoadTask(TaskId, Owner);
                }
                else
                {
                    Console.WriteLine("Error: TaskId or Owner is missing.");
                }
            }
        }
    }
}

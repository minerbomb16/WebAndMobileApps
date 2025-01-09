using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Json;
using TodoListSolution.Mobile.Models;

namespace TodoListSolution.Mobile.ViewModels
{
    public partial class EditTaskViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;

        [ObservableProperty] private string title;
        [ObservableProperty] private string description;

        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }

        private TodoItemDTO _task;

        public event Action TaskSaved;

        public EditTaskViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            SaveCommand = new AsyncRelayCommand(OnSaveAsync);
            CancelCommand = new AsyncRelayCommand(OnCancelAsync);
        }

        public void Initialize(TodoItemDTO task)
        {
            if (task == null) return;

            _task = task;
            Title = task.Title;
            Description = task.Description;

            Console.WriteLine($"ViewModel initialized with task: {task.Title}");
        }

        private async Task OnSaveAsync()
        {
            try
            {
                Console.WriteLine("Save button clicked");
                _task.Title = Title;
                _task.Description = Description;

                var updatedTask = new TodoItemDTO
                {
                    Id = _task.Id,
                    Title = _task.Title,
                    Description = _task.Description,
                    IsCompleted = _task.IsCompleted
                };

                var response = await _httpClient.PutAsJsonAsync($"api/todo/{_task.Id}", updatedTask);

                if (response.IsSuccessStatusCode)
                {
                    TaskSaved?.Invoke(); // Trigger the event
                    await Shell.Current.GoToAsync(".."); // Navigate back
                }
                else
                {
                    Console.WriteLine("Failed to save the task.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveCommand: {ex.Message}");
            }
        }

        private async Task OnCancelAsync()
        {
            try
            {
                Console.WriteLine("Cancel button clicked");

                // Navigate back
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CancelCommand: {ex.Message}");
            }
        }
    }
}

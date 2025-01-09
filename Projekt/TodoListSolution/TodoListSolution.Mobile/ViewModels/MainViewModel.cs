using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows.Input;
using TodoListSolution.Mobile.Models;
using Microsoft.Maui.Controls;


namespace TodoListSolution.Mobile.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Handler ignorujący błędy SSL
        private static readonly HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        // HttpClient z handlerem
        private static readonly HttpClient _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://10.0.2.2:7034")
        };

        // Properties for new task creation
        [ObservableProperty]
        private string newTitle;

        [ObservableProperty]
        private string newDescription;

        // List of tasks
        [ObservableProperty]
        private ObservableCollection<TodoItemDTO> todoItems = new();

        // Convenience properties for undone/done tasks
        public IEnumerable<TodoItemDTO> UndoneTasks => TodoItems.Where(t => !t.IsCompleted);
        public IEnumerable<TodoItemDTO> DoneTasks => TodoItems.Where(t => t.IsCompleted);

        // Commands
        public ICommand AddTodoCommand { get; }
        public ICommand MarkDoneCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditTodoCommand { get; }

        public MainViewModel()
        {

            // Initialize commands
            AddTodoCommand = new RelayCommand(async () => await AddTodo());
            MarkDoneCommand = new AsyncRelayCommand<TodoItemDTO>(MarkDone);
            DeleteCommand = new AsyncRelayCommand<TodoItemDTO>(DeleteItem);
            EditTodoCommand = new AsyncRelayCommand<TodoItemDTO>(OnEditTodo);

            // Load tasks initially
            LoadData();
        }

        // Reload data manually
        public void ReloadData()
        {
            LoadData();
        }

        // Navigate to EditTaskPage
        private async Task OnEditTodo(TodoItemDTO task)
        {
            if (task == null)
            {
                Console.WriteLine("EditTodoCommand received a null task");
                return;
            }

            try
            {
                Console.WriteLine($"Navigating to EditTaskPage with task: {task.Title}");
                await Shell.Current.GoToAsync("EditTaskPage", new Dictionary<string, object>
        {
            { "Task", task }
        });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to EditTaskPage: {ex.Message}");
            }
        }




        // Load tasks from API
        private async void LoadData()
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<TodoItemDTO>>("api/todo");
                if (items != null)
                {
                    TodoItems = new ObservableCollection<TodoItemDTO>(items);
                    OnPropertyChanged(nameof(UndoneTasks));
                    OnPropertyChanged(nameof(DoneTasks));
                }
            }
            catch (Exception ex)
            {
                // Log or display error
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }
        }

        // Add a new task
        private async Task AddTodo()
        {
            if (string.IsNullOrWhiteSpace(NewTitle)) return;

            var createPayload = new { Title = NewTitle, Description = NewDescription };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/todo", createPayload);
                if (response.IsSuccessStatusCode)
                {
                    LoadData();
                    NewTitle = string.Empty;
                    NewDescription = string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Log or display error
                Console.WriteLine($"Error adding task: {ex.Message}");
            }
        }

        // Mark a task as done
        private async Task MarkDone(TodoItemDTO task)
        {
            if (task == null) return;

            var updatePayload = new UpdateTodoItemDTO
            {
                Title = task.Title,
                Description = task.Description,
                IsCompleted = true
            };

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/todo/{task.Id}", updatePayload);
                if (response.IsSuccessStatusCode)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                // Log or display error
                Console.WriteLine($"Error marking task as done: {ex.Message}");
            }
        }

        // Delete a task
        private async Task DeleteItem(TodoItemDTO task)
        {
            if (task == null) return;

            try
            {
                var response = await _httpClient.DeleteAsync($"api/todo/{task.Id}");
                if (response.IsSuccessStatusCode)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                // Log or display error
                Console.WriteLine($"Error deleting task: {ex.Message}");
            }
        }

        // Update a task
        public async Task UpdateTaskAsync(TodoItemDTO task)
        {
            if (task == null) return;

            var updatePayload = new UpdateTodoItemDTO
            {
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted
            };

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/todo/{task.Id}", updatePayload);
                if (response.IsSuccessStatusCode)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                // Log or display error
                Console.WriteLine($"Error updating task: {ex.Message}");
            }
        }
    }
}

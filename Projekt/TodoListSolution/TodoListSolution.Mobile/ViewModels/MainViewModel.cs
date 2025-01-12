using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows.Input;
using TodoListSolution.Mobile.Models;

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

        [ObservableProperty]
        private string currentOwner;

        [ObservableProperty]
        private string newOwner;

        [ObservableProperty]
        private ObservableCollection<TodoItemDTO> todoItems;

        [ObservableProperty]
        private string newTitle;

        [ObservableProperty]
        private string newDescription;

        public IEnumerable<TodoItemDTO> UndoneTasks => TodoItems?.Where(t => !t.IsCompleted);
        public IEnumerable<TodoItemDTO> DoneTasks => TodoItems?.Where(t => t.IsCompleted);

        public ICommand AddTodoCommand { get; }
        public ICommand MarkDoneCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditTodoCommand { get; }
        public ICommand UpdateOwnerCommand { get; }
        public ICommand RefreshCommand { get; }

        // Default constructor for XAML compatibility
        public MainViewModel()
        {
            TodoItems = new ObservableCollection<TodoItemDTO>(); // Initialize an empty collection
        }

        // Constructor for runtime with dependency injection
        public MainViewModel(HttpClient httpClient)
        {

            // Initialize commands
            AddTodoCommand = new RelayCommand(async () => await AddTodo());
            MarkDoneCommand = new AsyncRelayCommand<TodoItemDTO>(MarkDone);
            DeleteCommand = new AsyncRelayCommand<TodoItemDTO>(DeleteItem);
            EditTodoCommand = new AsyncRelayCommand<TodoItemDTO>(OnEditTodo);
            UpdateOwnerCommand = new RelayCommand(UpdateOwner);
            RefreshCommand = new RelayCommand(LoadData); // Added RefreshCommand

            // Subscribe to refresh tasks after an update
            MessagingCenter.Subscribe<EditTaskViewModel>(this, "TaskUpdated", (sender) =>
            {
                LoadData();
            });

            // Load tasks initially
            LoadData();
        }


        private void UpdateOwner()
        {
            if (!string.IsNullOrWhiteSpace(NewOwner))
            {
                CurrentOwner = NewOwner;
                LoadData();
            }
        }

        public async void LoadData()
        {
            try
            {
                string endpoint = string.IsNullOrWhiteSpace(CurrentOwner) ? "api/todo" : $"api/todo?owner={Uri.EscapeDataString(CurrentOwner)}";
                string fullUrl = $"{_httpClient.BaseAddress}{endpoint}";
                Console.WriteLine($"[LOG] API URL: {fullUrl}");

                var items = await _httpClient.GetFromJsonAsync<List<TodoItemDTO>>(endpoint);
                if (items != null)
                {
                    TodoItems = new ObservableCollection<TodoItemDTO>(items);
                    OnPropertyChanged(nameof(UndoneTasks));
                    OnPropertyChanged(nameof(DoneTasks));
                }
                else
                {
                    TodoItems = new ObservableCollection<TodoItemDTO>(); // Empty collection if API returns null
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
                TodoItems = new ObservableCollection<TodoItemDTO>(); // Reset to empty collection on error
            }
        }


        private async Task AddTodo()
        {
            if (string.IsNullOrWhiteSpace(NewTitle))
            {
                Console.WriteLine("Task title is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentOwner))
            {
                Console.WriteLine("Current owner must be set to add a task.");
                return;
            }

            var newTask = new { Title = NewTitle, Description = NewDescription, Owner = CurrentOwner };
            var response = await _httpClient.PostAsJsonAsync("api/todo", newTask);

            if (response.IsSuccessStatusCode)
            {
                // Clear the fields after successfully adding the task
                NewTitle = string.Empty;
                NewDescription = string.Empty;

                // Notify the UI about the changes
                OnPropertyChanged(nameof(NewTitle));
                OnPropertyChanged(nameof(NewDescription));

                LoadData();
            }
            else
            {
                Console.WriteLine($"Failed to add task: {response.StatusCode}");
            }
        }


        private async Task MarkDone(TodoItemDTO task)
        {
            if (task == null || task.IsCompleted) return;

            task.IsCompleted = true;

            var response = await _httpClient.PutAsJsonAsync($"api/todo/{task.Id}", task);
            if (response.IsSuccessStatusCode)
            {
                LoadData();
            }
            else
            {
                Console.WriteLine($"Failed to mark task as done: {response.StatusCode}");
            }
        }


        private async Task DeleteItem(TodoItemDTO task)
        {
            if (task == null) return;

            var response = await _httpClient.DeleteAsync($"api/todo/{task.Id}");
            if (response.IsSuccessStatusCode)
            {
                TodoItems.Remove(task);
                OnPropertyChanged(nameof(UndoneTasks));
                OnPropertyChanged(nameof(DoneTasks));
            }
            else
            {
                Console.WriteLine($"Failed to delete task: {response.StatusCode}");
            }
        }

        private async Task OnEditTodo(TodoItemDTO task)
        {
            if (task == null) return;

            await Shell.Current.GoToAsync($"EditTaskPage?taskId={task.Id}&owner={CurrentOwner}");
        }

    }
}

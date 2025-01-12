using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoListSolution.Mobile.Models;

namespace TodoListSolution.Mobile.ViewModels
{
    public partial class EditTaskViewModel : ObservableObject
    {

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
        private string taskId;

        [ObservableProperty]
        private string taskTitle;

        [ObservableProperty]
        private string taskDescription;

        [ObservableProperty]
        private string currentOwner;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditTaskViewModel()
        {

            SaveCommand = new AsyncRelayCommand(SaveTask);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        public EditTaskViewModel(HttpClient httpClient)
        {

            SaveCommand = new AsyncRelayCommand(SaveTask);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        public async void LoadTask(string id, string owner)
        {
            TaskId = id;
            CurrentOwner = owner;

            try
            {
                var task = await _httpClient.GetFromJsonAsync<TodoItemDTO>($"api/todo/{id}");
                if (task != null)
                {
                    TaskTitle = task.Title;
                    TaskDescription = task.Description;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading task: {ex.Message}");
            }
        }

        private async Task SaveTask()
        {
            if (string.IsNullOrWhiteSpace(TaskTitle) || string.IsNullOrWhiteSpace(TaskDescription))
            {
                Console.WriteLine("Title and Description are required.");
                return;
            }

            var updatedTask = new
            {
                Title = TaskTitle,
                Description = TaskDescription,
                IsCompleted = false, // Preserve current state
                Owner = CurrentOwner
            };

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/todo/{TaskId}", updatedTask);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Task updated successfully.");

                    // Send a message to refresh tasks in the main view
                    MessagingCenter.Send(this, "TaskUpdated");

                    // Navigate back to the main page
                    await Shell.Current.GoToAsync($"..?owner={CurrentOwner}");
                }
                else
                {
                    Console.WriteLine($"Failed to update task: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating task: {ex.Message}");
            }
        }

        private async void CancelEdit()
        {
            await Shell.Current.GoToAsync($"..?owner={CurrentOwner}");
        }
    }
}

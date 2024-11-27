using aplikacjaMobilna.Models;
using aplikacjaMobilna.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace aplikacjaMobilna.ViewModels
{
    public partial class TasksViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;

        [ObservableProperty]
        private ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();

        [ObservableProperty]
        private string newTaskTitle = string.Empty;

        [ObservableProperty]
        private TaskItem? editingTask;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private bool isAdding = true;

        public TasksViewModel(ITaskService taskService)
        {
            _taskService = taskService;

            LoadTasksCommand = new AsyncRelayCommand(LoadTasksAsync);
            AddTaskCommand = new AsyncRelayCommand(AddTaskAsync);
            UpdateTaskCommand = new AsyncRelayCommand<TaskItem?>(UpdateTaskAsync);
            DeleteTaskCommand = new AsyncRelayCommand<TaskItem?>(DeleteTaskAsync);
            EditTaskCommand = new RelayCommand<TaskItem?>(BeginEditTask);
            SaveEditCommand = new AsyncRelayCommand(SaveEditAsync);

            Task.Run(LoadTasksAsync);
        }

        public IAsyncRelayCommand LoadTasksCommand { get; }
        public IAsyncRelayCommand AddTaskCommand { get; }
        public IAsyncRelayCommand<TaskItem> UpdateTaskCommand { get; }
        public IAsyncRelayCommand<TaskItem> DeleteTaskCommand { get; }
        public IRelayCommand<TaskItem> EditTaskCommand { get; }
        public IAsyncRelayCommand SaveEditCommand { get; }

        private async void Task_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is TaskItem task && e.PropertyName == nameof(TaskItem.IsCompleted))
            {
                await _taskService.UpdateTaskAsync(task);
            }
        }

        private async Task LoadTasksAsync()
        {
            var tasks = await _taskService.GetTasksAsync();

            foreach (var task in tasks)
            {
                task.PropertyChanged += Task_PropertyChanged;
            }

            Tasks = new ObservableCollection<TaskItem>(tasks);
        }

        private async Task AddTaskAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle)) return;

            var task = new TaskItem { Title = NewTaskTitle };
            await _taskService.AddTaskAsync(task);
            await LoadTasksAsync();
            NewTaskTitle = string.Empty;
        }

        private async Task UpdateTaskAsync(TaskItem? task)
        {
            if (task == null) return;
            await _taskService.UpdateTaskAsync(task);
            await LoadTasksAsync();
        }

        private async Task DeleteTaskAsync(TaskItem? task)
        {
            if (task == null) return;
            await _taskService.DeleteTaskAsync(task.Id);
            await LoadTasksAsync();
        }

        private void BeginEditTask(TaskItem? task)
        {
            if (task == null) return;
            EditingTask = new TaskItem { Id = task.Id, Title = task.Title, IsCompleted = task.IsCompleted };
            IsEditing = true;
            IsAdding = false;
        }

        private async Task SaveEditAsync()
        {
            if (EditingTask == null) return;

            await _taskService.UpdateTaskAsync(EditingTask);
            await LoadTasksAsync();
            EditingTask = null;
            IsEditing = false;
            IsAdding = true;
        }
    }
}

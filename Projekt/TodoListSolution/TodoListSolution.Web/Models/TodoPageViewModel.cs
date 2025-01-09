using System.ComponentModel.DataAnnotations;

namespace TodoListSolution.Web.Models
{
    public class TodoItemDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TodoPageViewModel
    {
        public List<TodoItemDTO> Tasks { get; set; } = new();

        // For adding
        public string? NewTitle { get; set; }
        public string? NewDescription { get; set; }

        // For marking done
        public Guid? MarkDoneId { get; set; }

        // For deleting
        public Guid? DeleteId { get; set; }

        // For editing
        public Guid? EditId { get; set; }
        public string? EditTitle { get; set; }
        public string? EditDescription { get; set; }
        public bool? EditIsCompleted { get; set; }
    }

}

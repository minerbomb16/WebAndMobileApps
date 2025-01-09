namespace TodoListSolution.Mobile.Models
{
    public class TodoItemDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}

namespace TodoListSolution.API.DTOs
{
    public class CreateTodoItemDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Owner { get; set; } // Ensure this is present
    }

}

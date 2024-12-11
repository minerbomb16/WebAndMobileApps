namespace RealTimeTicketing.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }


}

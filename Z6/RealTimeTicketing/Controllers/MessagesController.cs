using Microsoft.AspNetCore.Mvc;
using RealTimeTicketing.Data;
using RealTimeTicketing.Models;
using RealTimeTicketing.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace RealTimeTicketing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly TicketDbContext _context;
        private readonly IHubContext<RealTimeHub> _hubContext;

        public MessagesController(TicketDbContext context, IHubContext<RealTimeHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/messages/{ticketId}
        [HttpGet("{ticketId}")]
        public IActionResult GetMessages(int ticketId)
        {
            if (!_context.Tickets.Any(t => t.Id == ticketId))
            {
                return NotFound(new { message = $"Ticket with ID {ticketId} not found." });
            }

            var messages = _context.Messages
                .Where(m => m.TicketId == ticketId)
                .OrderBy(m => m.SentAt)
                .ToList();

            return Ok(messages);
        }

        // POST: api/messages
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            if (!_context.Tickets.Any(t => t.Id == message.TicketId))
            {
                return BadRequest(new { message = $"Ticket with ID {message.TicketId} does not exist." });
            }

            if (string.IsNullOrWhiteSpace(message.Author) || string.IsNullOrWhiteSpace(message.Content))
            {
                return BadRequest(new { message = "Message author and content cannot be empty." });
            }

            message.SentAt = DateTime.UtcNow; // Ensure the timestamp is set

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Broadcast the message to the specific ticket group
            await _hubContext.Clients.Group($"ticket-{message.TicketId}").SendAsync("ReceiveMessage", message);

            return Ok(message);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using RealTimeTicketing.Data;
using RealTimeTicketing.Models;
using Microsoft.AspNetCore.SignalR;
using RealTimeTicketing.Hubs;

namespace RealTimeTicketing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly TicketDbContext _context;
        private readonly IHubContext<RealTimeHub> _hubContext;

        public TicketsController(TicketDbContext context, IHubContext<RealTimeHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetTickets()
        {
            return Ok(_context.Tickets.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("NewTicket", ticket); // Broadcast new ticket
            return Ok(ticket);
        }

        public class TicketUpdateRequest
        {
            public string Status { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] TicketUpdateRequest updateRequest)
        {
            Console.WriteLine($"Update request for ticket ID: {id} with status: {updateRequest?.Status}");

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                Console.WriteLine("Ticket not found.");
                return NotFound(new { message = $"Ticket with ID {id} not found" });
            }

            if (string.IsNullOrWhiteSpace(updateRequest?.Status))
            {
                Console.WriteLine("Invalid status provided.");
                return BadRequest(new { message = "Status is required" });
            }

            ticket.Status = updateRequest.Status;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Broadcast the updated ticket to all clients
            await _hubContext.Clients.All.SendAsync("UpdateTicket", ticket);

            Console.WriteLine($"Ticket {id} updated successfully to status {ticket.Status}");
            return Ok(ticket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            Console.WriteLine($"Delete request for ticket ID: {id}");

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                Console.WriteLine("Ticket not found.");
                return NotFound(new { message = $"Ticket with ID {id} not found" });
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            // Broadcast the deleted ticket ID to all clients
            await _hubContext.Clients.All.SendAsync("DeleteTicket", id);

            Console.WriteLine($"Ticket {id} deleted successfully.");
            return Ok(new { message = "Ticket deleted successfully." });
        }


    }
}

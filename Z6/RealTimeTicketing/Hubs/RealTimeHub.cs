using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RealTimeTicketing.Hubs
{
    public class RealTimeHub : Hub
    {
        private readonly ILogger<RealTimeHub> _logger;

        public RealTimeHub(ILogger<RealTimeHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinTicketGroup(int ticketId)
        {
            var groupName = $"ticket-{ticketId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation($"User {Context.ConnectionId} joined group {groupName}.");

            // Optionally notify others in the group
            await Clients.Group(groupName).SendAsync("UserJoined", Context.ConnectionId, groupName);
        }

        public async Task LeaveTicketGroup(int ticketId)
        {
            var groupName = $"ticket-{ticketId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation($"User {Context.ConnectionId} left group {groupName}.");

            // Optionally notify others in the group
            await Clients.Group(groupName).SendAsync("UserLeft", Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"User {Context.ConnectionId} connected.");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"User {Context.ConnectionId} disconnected. Reason: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}

using AsyncMessageOrchestrator.Application.Common.Interfaces;
using AsyncMessageOrchestrator.Application.Dtos;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace AsyncMessageOrchestrator.WebApi.Hubs
{
    public class NotificationHub : Hub, INotificationHub
    {
        private static ConcurrentBag<string> UserConnections = new();
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHub(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            // Add connection ID to user connections
            UserConnections.Add(Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var connectionIds = new ConcurrentBag<string>(
                UserConnections.Where(x => x != connectionId));

            UserConnections = connectionIds;

            await base.OnDisconnectedAsync(exception);
        }

        public Task SendNotificationToUsers(MessageDto message)
        {
            return _hubContext
                .Clients
                .Clients([.. UserConnections])
                .SendAsync("ReceiveNotification", message);
        }
    }
}

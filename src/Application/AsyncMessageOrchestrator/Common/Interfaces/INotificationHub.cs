using AsyncMessageOrchestrator.Application.Dtos;

namespace AsyncMessageOrchestrator.Application.Common.Interfaces
{
    public interface INotificationHub
    {
        Task SendNotificationToUsers(MessageDto message);
    }
}
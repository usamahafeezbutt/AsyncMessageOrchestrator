using AsyncMessageOrchestrator.Application.Common.Interfaces;
using AsyncMessageOrchestrator.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AsyncMessageOrchestrator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly INotificationHub _notificationHub;
        public MessagesController(INotificationHub notificationHub)
        {
            _notificationHub = notificationHub;
        }

        [HttpPost(nameof(NotifyComplete))]
        public async Task<ActionResult> NotifyComplete([FromBody] MessageDto messageDto)
        {
            await _notificationHub.SendNotificationToUsers(messageDto);
            return Ok(new ResponseDto<string>(true, "Your message is notified successfully", string.Empty));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Smart_Freelance_Infrastructure.Notifications;

namespace Smart_Freelance_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationTestController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationTestController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("send")]
        public async Task<IActionResult> SendTestNotification()
        {
            await _hubContext.Clients.All.SendAsync("NotificationReceived", "🔥 Test notification from server!");
            return Ok("Notification sent!");
        }
    }
}

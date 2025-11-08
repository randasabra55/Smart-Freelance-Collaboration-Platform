using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Smart_Freelance_Infrastructure.Notifications
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var userId =
                connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                connection.User?.FindFirst("sub")?.Value ??
                connection.User?.FindFirst("userId")?.Value;

            return userId;
        }
    }
}

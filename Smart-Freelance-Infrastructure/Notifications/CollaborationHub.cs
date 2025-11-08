using Microsoft.AspNetCore.SignalR;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Infrastructure.Data;
using System.Security.Claims;

namespace Smart_Freelance_Infrastructure.Notifications
{
    public class CollaborationHub(Context context) : Hub
    {

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            //register connection in ConnectedUsersRepository
            return base.OnConnectedAsync();
        }

        public Task OnDisConnectedAsync(Exception? ex)
        {
            //remove connection
            return base.OnDisconnectedAsync(ex);
        }
        //اسم الميثود هينادى عليها الفرونت
        //اسم الايفنت هيليسن عليها الطرف التانى عشان يشوف الرسالة المبعوته من الطرف الاولانى
        public async Task SendMessage(long roomId, string content)
        {
            var sender = Context.UserIdentifier ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var message = new Message { RoomId = roomId, SenderId = long.Parse(sender!), Content = content, SentAt = DateTime.Now };
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
            //persist message via service or mediator
            await Clients.Group($"room-{roomId}").SendAsync("MessageReceived", message);
        }

        public async Task JoinRoom(long roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{roomId}");
        }
    }
}

//"SendMessage"=> الميثود اللي الفرونت بيناديها

//"MessageReceived"  الإيفينت اللي الفرونت بيسمع عليه

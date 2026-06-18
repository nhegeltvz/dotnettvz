using Data.Data;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatMessageStore _store;
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(ChatMessageStore store, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _store = store;
        }

        public async Task JoinParty(string partyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, partyId);
        }

        public async Task LeaveParty(string partyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, partyId);
        }

        public async Task SendMessage(string partyId, string Text)
        {
            if (string.IsNullOrWhiteSpace(Text) || Text.Length > 500)
                return;

            var user = await _userManager.GetUserAsync(Context.User!);

            if (user == null) return;

            var pid = Guid.Parse(partyId);
            var isMember = await _store.IsUserMemberOfParty(pid, user.Id);

            if (!isMember) return;

            var message = new ChatMessage
            {
                PartyId = pid,
                SenderUserId = user.Id,
                SenderUsername = user.UserName!,
                Text = Text.Trim(),
                SentAt = DateTime.UtcNow,
            };

            await _store.AddChatMessage(message);

            await Clients.Group(partyId).SendAsync("ReceiveMessage", new
            {
                id = message.Id,
                senderUsername = message.SenderUsername,
                text = message.Text,
                sentAt = message.SentAt
            });
        }

    }
}

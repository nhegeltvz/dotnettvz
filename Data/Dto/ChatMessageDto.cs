using Data.Dto.CRUD.Party;
using Data.Models;
using System.Linq.Expressions;

namespace Data.Dto
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }

        public static Expression<Func<ChatMessage, ChatMessageDto>> ToDto()
        {
            return chatMessage => new ChatMessageDto
            {
                Id = chatMessage.Id,
                Username = chatMessage.SenderUsername,
                Text = chatMessage.Text,
                SentAt = chatMessage.SentAt
            };
        }
    }
}

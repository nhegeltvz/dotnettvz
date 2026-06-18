using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Data.Services.Stores
{
    public class ChatMessageStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<ChatMessage> _chatMessageValidator;

        public ChatMessageStore(MatchTrackerDbContext dbContext, IValidator<ChatMessage> chatMessageValidator)
        {
            _dbContext = dbContext;
            _chatMessageValidator = chatMessageValidator;
        }

        public IQueryable<ChatMessage> QueryPartyChatHistory(Guid partyId, int take = 50, int skip = 0)
             => _dbContext.ChatMessages
                .Where(m => m.PartyId == partyId)
                .OrderByDescending(m => m.SentAt)
                .Skip(skip)
                .Take(take);

    public async Task<Result<int>> AddChatMessage(IChatMessage model)
        {
            var chatMessage = new ChatMessage();
            UpdateChatMessage(model, chatMessage);
            var validationResult = _chatMessageValidator.Validate(chatMessage);
            if (!validationResult.IsSuccess)
                return Result<int>.FromResult(validationResult);

            _dbContext.ChatMessages.Add(chatMessage);
            await _dbContext.SaveChangesAsync();

            return Result<int>.Success(chatMessage.Id);
        }

        public async Task<bool> IsUserMemberOfParty(Guid pid, Guid UserId)
            => await _dbContext.Players.AnyAsync(p => p.JoinedParties.Any(party => party.Id == pid) && p.UserId == UserId);

        private ChatMessage UpdateChatMessage(IChatMessage model, ChatMessage chatMessage)
        {
            chatMessage.Id = model.Id;
            chatMessage.PartyId = model.PartyId;
            chatMessage.SenderUserId = model.SenderUserId;
            chatMessage.SenderUsername = model.SenderUsername;
            chatMessage.Text = model.Text;
            chatMessage.SentAt = model.SentAt;
            return chatMessage;
            
        }

    }
}

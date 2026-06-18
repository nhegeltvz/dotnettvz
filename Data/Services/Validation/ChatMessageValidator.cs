using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class ChatMessageValidator : IValidator<ChatMessage>
    {
        public Result Validate(ChatMessage entity)
        {
            //Yet to implement
            return Result.Success();
        }
    }
}

using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class PlayingFieldValidator : IValidator<PlayingField>
    {
        public Result Validate(PlayingField entity)
        {
            return Result.Success();
        }
    }
}

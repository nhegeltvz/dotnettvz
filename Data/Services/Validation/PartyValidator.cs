using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class PartyValidator : IValidator<Party>
    {
        public Result Validate(Party entity)
        {
            return Result.Success();
        }
    }
}

using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class MatchRecordValidator : IValidator<MatchRecord>
    {
        public Result Validate(MatchRecord entity)
        {
            return Result.Success();
        }
    }
}

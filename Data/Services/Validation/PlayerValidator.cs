using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class PlayerValidator : IValidator<Player>
    {
        public Result Validate(Player entity)
        {
            throw new NotImplementedException();
        }
    }
}

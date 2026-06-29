using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class MatchVoteValidator : IValidator<MatchVote>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public MatchVoteValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(MatchVote entity)
        {
            var errors = new List<Error>();

            if (entity.MatchRecordId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Utakmica je obavezna."));
            }
            else if (!_dbContext.MatchRecords.Any(record => record.Id == entity.MatchRecordId))
            {
                errors.Add(new Error(ErrorType.Validation, "Utakmica ne postoji."));
            }

            if (entity.PlayerId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač je obavezan."));
            }
            else if (!_dbContext.Users.Any(player => player.Id == entity.PlayerId))
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač ne postoji."));
            }

            if (entity.PlayerId != Guid.Empty && entity.MatchRecordId != Guid.Empty)
            {
                var duplicate = _dbContext.MatchVotes.Any(vote =>
                    vote.Id != entity.Id
                    && vote.PlayerId == entity.PlayerId
                    && vote.MatchRecordId == entity.MatchRecordId);
                if (duplicate)
                {
                    errors.Add(new Error(ErrorType.Validation, "Igrač je već glasao za ovu utakmicu."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}

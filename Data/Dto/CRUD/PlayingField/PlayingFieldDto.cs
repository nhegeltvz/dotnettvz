using Data.Data;
using System.Linq.Expressions;
using PlayingFieldModel = Data.Models.PlayingField;

namespace Data.Dto.CRUD.PlayingField
{
    public class PlayingFieldDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public FieldStatus Status { get; set; }
        public bool IsOutdoor { get; set; }
        public SurfaceType SurfaceType { get; set; }
        public byte[]? Image { get; set; }
        public int PlayedMatchesCount { get; set; }
        public int UpcomingScheduledMatchesCount { get; set; }

        public static Expression<Func<PlayingFieldModel, PlayingFieldDto>> ToDto()
        {
            var now = DateTime.UtcNow;
            return playingField => new PlayingFieldDto
            {
                Id = playingField.Id,
                Name = playingField.Name,
                Description = playingField.Description,
                Longitude = playingField.Longitude,
                Latitude = playingField.Latitude,
                ContactNumber = playingField.ContactNumber,
                Status = playingField.Status,
                IsOutdoor = playingField.IsOutdoor,
                SurfaceType = playingField.SurfaceType,
                PlayedMatchesCount = playingField.MatchRecords.Count(record => record.WasMatchHeld),
                UpcomingScheduledMatchesCount = playingField.ScheduledMatches.Count(match => match.MatchDate > now)
            };
        }
    }
}

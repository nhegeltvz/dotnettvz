namespace Data.Data.Common
{

    public record Error(ErrorType Type, string Description);

    public enum ErrorType
    {
        NotFound,
        Validation,
        Unauthorized,
        DataOperation
    }

    public static class MatchPlayerErrors
    {
        public static Error MatchPlayerNotFound { get; } = new(ErrorType.NotFound, "Match player not found.");
        public static Error MatchPlayerNotUpdated { get; } = new(ErrorType.DataOperation, "Match player not updated.");
        public static Error MatchPlayerNotDeleted { get; } = new(ErrorType.DataOperation, "Match player not deleted.");
    }

    public static class MatchRecordErrors
    {
        public static Error MatchRecordNotFound { get; } = new(ErrorType.NotFound, "Match record not found.");
        public static Error MatchRecordNotUpdated { get; } = new(ErrorType.DataOperation, "Match record not updated.");
        public static Error MatchRecordNotDeleted { get; } = new(ErrorType.DataOperation, "Match record not deleted.");
    }

    public static class MatchVoteErrors
    {
        public static Error MatchVoteNotFound { get; } = new(ErrorType.NotFound, "Match vote not found.");
        public static Error MatchVoteNotUpdated { get; } = new(ErrorType.DataOperation, "Match vote not updated.");
        public static Error MatchVoteNotDeleted { get; } = new(ErrorType.DataOperation, "Match vote not deleted.");
    }

    public static class PartyErrors
    {
        public static Error PartyNotFound { get; } = new(ErrorType.NotFound, "Party not found.");
        public static Error PartyNotUpdated { get; } = new(ErrorType.DataOperation, "Party not updated.");
        public static Error PartyNotDeleted { get; } = new(ErrorType.DataOperation, "Party not deleted.");
    }

    public static class PlayerErrors
    {
        public static Error PlayerNotFound { get; } = new(ErrorType.NotFound, "Player not found.");
        public static Error PlayerNotUpdated { get; } = new(ErrorType.DataOperation, "Player not updated.");
        public static Error PlayerNotDeleted { get; } = new(ErrorType.DataOperation, "Player not deleted.");
    }

    public static class PlayerRatingErrors
    {
        public static Error PlayerRatingNotFound { get; } = new(ErrorType.NotFound, "Player rating not found.");
        public static Error PlayerRatingNotUpdated { get; } = new(ErrorType.DataOperation, "Player rating not updated.");
        public static Error PlayerRatingNotDeleted { get; } = new(ErrorType.DataOperation, "Player rating not deleted.");
    }

    public static class PlayingFieldErrors
    {
        public static Error PlayingFieldNotFound { get; } = new(ErrorType.NotFound, "Playing field not found.");
        public static Error PlayingFieldNotUpdated { get; } = new(ErrorType.DataOperation, "Playing field not updated.");
        public static Error PlayingFieldNotDeleted { get; } = new(ErrorType.DataOperation, "Playing field not deleted.");
    }

    public static class PreferredPlayingDateErrors
    {
        public static Error PreferredPlayingDateNotFound { get; } = new(ErrorType.NotFound, "Preferred playing date not found.");
        public static Error PreferredPlayingDateNotUpdated { get; } = new(ErrorType.DataOperation, "Preferred playing date not updated.");
        public static Error PreferredPlayingDateNotDeleted { get; } = new(ErrorType.DataOperation, "Preferred playing date not deleted.");
    }

    public static class ScheduledMatchErrors
    {
        public static Error ScheduledMatchNotFound { get; } = new(ErrorType.NotFound, "Scheduled match not found.");
        public static Error ScheduledMatchNotUpdated { get; } = new(ErrorType.DataOperation, "Scheduled match not updated.");
        public static Error ScheduledMatchNotDeleted { get; } = new(ErrorType.DataOperation, "Scheduled match not deleted.");
    }

    public static class ScheduledMatchAttendanceErrors
    {
        public static Error ScheduledMatchAttendanceNotFound { get; } = new(ErrorType.NotFound, "Scheduled match attendance not found.");
        public static Error ScheduledMatchAttendanceNotUpdated { get; } = new(ErrorType.DataOperation, "Scheduled match attendance not updated.");
        public static Error ScheduledMatchAttendanceNotDeleted { get; } = new(ErrorType.DataOperation, "Scheduled match attendance not deleted.");
    }
}

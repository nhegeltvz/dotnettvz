namespace Data.Dto.CRUD.MatchRecord
{
    public class MatchRecordListDto
    {
        public Guid Id { get; set; }
        public int GoalsTeamA { get; set; }
        public int GoalsTeamB { get; set; }
        public Guid PlayingFieldId { get; set; }
        public string PlayingFieldName { get; set; } = string.Empty;
        public DateTime MatchHeld { get; set; }
        public bool WasMatchHeld { get; set; }
    }
}

namespace Data.Dto.CRUD.Party
{
    public class PartyListDto
    {
        public Guid Id { get; set; }
        public string PartyDescription { get; set; }
        public DateTime DateCreated { get; set; }
        public int MaxMembers { get; set; }
        public int NumberOfMembers { get; set; }
        public string PlayerCreatedUsername { get; set; }
    }
}

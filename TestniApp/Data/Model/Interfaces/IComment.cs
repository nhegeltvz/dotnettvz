namespace Data.Model.Interfaces
{
    public interface IComment
    {
        string Text { get; }
        string AuthorId { get; }
        DateTime CommentedAt { get; }
        Guid TicketId { get; }
    }
}

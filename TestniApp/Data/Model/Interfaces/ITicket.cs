using static Data.Data.Enums;

namespace Data.Model.Interfaces
{
    public interface ITicket
    {
        string Title { get; }
        string Description { get; }

        TicketStatus Status { get; }
        int Priority { get; }
        DateTime CreatedAt { get; }

        int CategoryId { get; }
        string AssigneeId { get; }
        string ReporterId { get; }
    }
}

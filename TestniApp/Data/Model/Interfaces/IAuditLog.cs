using static Data.Data.Enums;

namespace Data.Model.Interfaces
{
    public interface IAuditLog
    {
        DateTime DateTicketStatusChanged { get; }
        Guid TicketId { get; }
        string ChangedByUserId { get; }
        TicketStatus OldStatus { get; }
        TicketStatus NewStatus { get; }
    }
}

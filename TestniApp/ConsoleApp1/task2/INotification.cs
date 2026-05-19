namespace ConsoleApp1.task2
{
    public interface INotification
    {
        string LogSentTime();
        string SenderAddress { get; }
    }
}

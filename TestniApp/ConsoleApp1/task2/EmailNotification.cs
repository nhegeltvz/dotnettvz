namespace ConsoleApp1.task2
{
    public class EmailNotification : INotification
    {
        public string SenderAddress { get; set; }

        public string LogSentTime()
        {
            return DateTime.Now.ToString() + " from an Email";
        }
    }
}

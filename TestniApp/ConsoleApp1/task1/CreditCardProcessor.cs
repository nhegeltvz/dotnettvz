namespace ConsoleApp1.task1
{
    public class CreditCardProcessor : PaymentProcessor
    {
        public override void Process()
        {
            Console.WriteLine("Paying via credit card...");
        }
    }
}

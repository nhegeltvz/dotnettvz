namespace ConsoleApp1.task1
{
    public class PayPalProcessor : PaymentProcessor
    {
        public override void Process()
        {
            Console.WriteLine("Paying via paypal...");
        }
    }
}

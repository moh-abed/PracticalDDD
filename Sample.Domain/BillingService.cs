using System;

namespace Sample.Domain
{
    public class BillingService
    {
        public void Pay(string nameOnCard, string cardNumber)
        {
            Printer.Print(ConsoleColor.Green);
        }
    }
}

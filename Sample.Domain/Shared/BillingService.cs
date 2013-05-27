using System;

namespace Sample.Domain.Shared
{
    public class BillingService
    {
        public void Pay(string nameOnCard, string cardNumber)
        {
            Printer.Print(ConsoleColor.Green);
        }
    }
}

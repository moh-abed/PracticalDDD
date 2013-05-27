using System;

namespace Sample.Domain
{
    public interface ICreditValidationService
    {
        bool IsValid(string nameOnCard, string cardNumber);
    }

    public class CreditValidationService : ICreditValidationService
    {
        public bool IsValid(string nameOnCard, string cardNumber)
        {
            Printer.Print(ConsoleColor.Green);
            return !string.IsNullOrEmpty(nameOnCard) && !string.IsNullOrEmpty(cardNumber);
        }
    }
}

using System;

namespace Sample.Domain.V3
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T customer);
    }

    public class ValidCustomerCreditCardSpecification : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer)
        {
            Printer.Print(ConsoleColor.Cyan);
            return customer.CreditCard != null;
        }
    }
}
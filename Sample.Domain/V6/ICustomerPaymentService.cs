using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V6
{
    public interface ICustomerPaymentService
    {
        void Pay(Customer customer);
    }

    public class MyCustomerPaymentService : ICustomerPaymentService
    {
        public void Pay(Customer customer)
        {
            Printer.Print(ConsoleColor.Cyan);
        }
    }
}

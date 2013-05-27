﻿using System;

namespace Sample.Domain.V3
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

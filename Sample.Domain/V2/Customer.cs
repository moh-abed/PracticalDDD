using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V2
{
    public class Customer
    {
        public Guid Id { get; set; }
        public FullName Name { get; set; }
        public Email Email { get; set; }
        public string Phone { get; set; } // what if we want type?
        public CreditCardDetails CreditCard { get; set; }

        private Customer(Guid id, string firstName, string familyName, string email = null, string phone = null)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone))
                throw new Exception("At least one contact should be provided (email or phone)");

            Id = id;
            Name = new FullName(firstName, familyName);
            if (!string.IsNullOrEmpty(email))
                Email = email;
            Phone = phone;
        }

        public static Customer Register(string firstName, string familyName, string email = null, string phone = null,
                                        string nameOnCredit = null, string cardNumber = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            var customer = new Customer(Guid.NewGuid(), firstName, familyName, email, phone);

            if(!string.IsNullOrEmpty(nameOnCredit) && !string.IsNullOrEmpty(cardNumber))
                customer.CreditCard = new CreditCardDetails(nameOnCredit, cardNumber);

            return customer;
        }
    }
}

﻿using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V6
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public FullName Name { get; private set; }
        public Email Email { get; private set; }
        public string Phone { get; private set; }
        public CreditCardDetails CreditCard { get; private set; }
        public bool IsBlackListed { get; private set; }

        private Customer(Guid id, string firstName, string familyName, string email = null, string phone = null)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone))
                throw new Exception("At least one contact should be provided (email or phone)");

            Id = id;
            Name = new FullName(firstName, familyName);
            if(!string.IsNullOrEmpty(email))
                Email = email;
            Phone = phone;
        }

        public void Rename(string firstName, string lastName)
        {
            var newName = new FullName(firstName, lastName);
            Name = newName;
        }

        public void Charge(ICustomerPaymentService paymentService, INotificationService notificationService)
        {
            paymentService.Pay(this);
            notificationService.SendEmail(Email, "Paid successfully", "bla bla bla");
        }

        public void Charge()
        {
            // do some internal logic regarding payment

            DomainEvents.Publish(new CustomerReadyForPayment(UserProfile.Name, Id));
        }

        public void UpdateContact(string email, string phone)
        {
            Email = !string.IsNullOrEmpty(email) ? email : null;
            Phone = phone;
        }

        public void UpdateCrediCard(string nameOnCard, string cardNumber)
        {
            CreditCard = new CreditCardDetails(nameOnCard, cardNumber);
        }

        public bool ValidateCreditCard(ICreditValidationService creditValidator)
        {
            return creditValidator.IsValid(CreditCard.NameOnCard, CreditCard.CardNumber);
        }

        public bool ValidateCreditCard(ValidCustomerCreditCardSpecification specification)
        {
            return specification.IsSatisfiedBy(this);
        }

        public static Customer Register(string firstName, string familyName, string email = null, string phone = null,
                                        string nameOnCredit = null, string cardNumber = null)
        {
            var customer = new Customer(Guid.NewGuid(), firstName, familyName, email, phone);

            if(!string.IsNullOrEmpty(nameOnCredit) && !string.IsNullOrEmpty(cardNumber))
                customer.CreditCard = new CreditCardDetails(nameOnCredit, cardNumber);

            return customer;
        }
    }

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

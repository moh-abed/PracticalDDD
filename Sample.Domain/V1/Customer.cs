using System;

namespace Sample.Domain.V1
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string NameOnCreditCard { get; set; }
        public string CreditCardNumber { get; set; }
    }
}

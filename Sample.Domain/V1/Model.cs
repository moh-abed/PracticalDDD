using System;
using System.Collections.Generic;
using Sample.Domain.Shared;

namespace Sample.Domain.V1
{
    public class StaffMember
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class Job
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public Status Status { get; set; }
        public string Location { get; set; }
        public List<Appointment> Appointments { get; set; }
    }

    public class Appointment
    {
        public Guid Id { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public StaffMember StaffMember { get; set; }
        public string Notes { get; set; }
        public Status Status { get; set; }
    }

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

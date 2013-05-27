using System;
using System.Collections.Generic;

namespace Sample.Domain.V2
{
    public class Job
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public Status Status { get; set; }
        public string Location { get; set; }
        public List<Appointment> Appointments { get; set; }

        private Job(Guid id, Customer customer, string location = null)
        {
            if (customer == null)
                throw new Exception("Customer should be provided");

            Id = id;
            Customer = customer;
            Location = location;
            Status = Status.Initiated;
        }

        public static Job Create(Customer customer, string location = null, List<Appointment> appointments = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            return new Job(Guid.NewGuid(), customer, location) { Appointments = appointments };
        }
    }
}


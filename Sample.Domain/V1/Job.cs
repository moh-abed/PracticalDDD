using System;
using System.Collections.Generic;

namespace Sample.Domain.V1
{
    public class Job
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public Status Status { get; set; }
        public string Location { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}


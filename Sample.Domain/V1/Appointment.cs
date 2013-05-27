using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V1
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public StaffMember StaffMember { get; set; }
        public string Notes { get; set; }
        public Status Status { get; set; }
    }
}

using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V2
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public StaffMember StaffMember { get; set; }
        public Status Status { get; set; }
        public string Notes { get; set; }

        private Appointment(Guid id,
                           DateTime? from = null,
                           DateTime? to = null,
                           StaffMember staffMember = null,
                           string notes = null)
        {
            if (from == null && to == null && staffMember == null)
                throw new Exception("You have to either select time frame or select staff member");

            if(from != null && to != null)
                TimeSlot = new TimeSlot(from.Value, to.Value);

            Id = id;
            StaffMember = staffMember;
            Notes = notes;
            Status = Status.Initiated;
        }

        public static Appointment ScheduleNew(DateTime from, DateTime to, StaffMember member = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            return new Appointment(Guid.NewGuid(), from, to, member);
        }
    }
}

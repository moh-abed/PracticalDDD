using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V3
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public TimeSlot TimeSlot { get; private set; }
        public StaffMember StaffMember { get; private set; }
        public Status Status { get; private set; }
        public string Notes { get; private set; }

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

        public void AssignStaffMember(StaffMember staffMember)
        {
            Printer.Print(ConsoleColor.Cyan);

            StaffMember = staffMember;
        }

        public void Reschedule(DateTime from, DateTime to)
        {
            Printer.Print(ConsoleColor.Cyan);

            TimeSlot = new TimeSlot(from, to);
        }

        public static Appointment New(DateTime from, DateTime to, StaffMember member = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            return new Appointment(Guid.NewGuid(), from, to, member);
        }

        public void InProgress()
        {
            Printer.Print(ConsoleColor.Cyan);
            Status = Status.InProgress;
        }
        public void Finished()
        {
            Printer.Print(ConsoleColor.Cyan);
            Status = Status.Finished;
        }
        public bool IsInProgress()
        {
            return Status == Status.InProgress;
        }

        public void Cancel()
        {
            Printer.Print(ConsoleColor.Cyan);
            Status = Status.Canceled;
        }
        public bool IsCanceled()
        {
            return Status == Status.Canceled;
        }
    }
}

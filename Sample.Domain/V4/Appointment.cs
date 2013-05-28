using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V4
{
    public class Appointment
    {
        public Guid Id { get; private set; }
        public Guid JobId { get; set; }
        public TimeSlot TimeSlot { get; private set; }
        public Guid? StaffMemberId { get; private set; }
        public Status Status { get; private set; }
        public string Comments { get; private set; }

        private Appointment(Guid id,
                           Guid jobId,
                           DateTime? from = null,
                           DateTime? to = null,
                           Guid? staffMemberId = null)
        {
            if (from == null && to == null && staffMemberId == null)
                throw new Exception("You have to either select time frame or select staff member");

            if(from != null && to != null)
                TimeSlot = new TimeSlot(from.Value, to.Value);

            Id = id;
            JobId = jobId;
            StaffMemberId = staffMemberId;
            Status = Status.Initiated;
        }

        public void AssignStaffMember(Guid staffMemberId)
        {
            Printer.Print(ConsoleColor.Cyan);

            StaffMemberId = staffMemberId;
        }

        public void Reschedule(DateTime from, DateTime to)
        {
            Printer.Print( ConsoleColor.Cyan);

            TimeSlot = new TimeSlot(from, to);
        }

        public void Start()
        {
            Printer.Print(ConsoleColor.Cyan);

            if (Status == Status.Canceled)
                throw new Exception("Appointment is cancelled, can not mark it in progress");

            Status = Status.InProgress;

            DomainEvents.Publish(new AppointmentStarted(this));
        }
        public void Finish(string comments = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            Status = Status.Finished;
            Comments = comments;

            DomainEvents.Publish(new AppointmentFinished(this));
        }
        public bool IsInProgress()
        {
            return Status == Status.InProgress;
        }
        public bool IsFinished()
        {
            return Status == Status.Finished;
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

        public static Appointment Schedule(Guid jobId, DateTime from, DateTime to, Guid? memberId = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            return new Appointment(Guid.NewGuid(), jobId, from, to, memberId);
        }
    }
}

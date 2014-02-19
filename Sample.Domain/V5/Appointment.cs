using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V5
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

            var @event = new AppointmentScheduled(id, jobId, staffMemberId, from, to);
            Apply(@event);
            DomainEvents.Publish(@event);
        }

        public static Appointment Schedule(Guid jobId, DateTime from, DateTime to, Guid? memberId = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            return new Appointment(Guid.NewGuid(), jobId, from, to, memberId);
        }

        public void AssignStaffMember(Guid staffMemberId)
        {
            Printer.Print(ConsoleColor.Cyan);

            var @event = new StaffAssignedToAppointment(Id, staffMemberId);
            Apply(@event);
            DomainEvents.Publish(@event);
        }

        public void Reschedule(DateTime from, DateTime to)
        {
            Printer.Print(ConsoleColor.Cyan);

            var @event = new AppointmentRescheduled(Id, from, to);
            Apply(@event);
            DomainEvents.Publish(@event);
        }

        public void Start()
        {
            Printer.Print(ConsoleColor.Cyan);

            if (Status == Status.Canceled)
                throw new Exception("Appointment is cancelled, can not mark it in progress");

            var @event = new AppointmentStarted(Id);
            Apply(@event);
            DomainEvents.Publish(@event);
        }
        public void Finish(string comments = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            var @event = new AppointmentCompleted(Id, comments);
            Apply(@event);
            DomainEvents.Publish(@event);
        }
        public bool IsInProgress()
        {
            return Status == Status.InProgress;
        }
        public bool IsCompleted()
        {
            return Status == Status.Completed;
        }

        public void Cancel()
        {
            Printer.Print(ConsoleColor.Cyan);

            var @event = new AppointmentCancelled(Id);
            Apply(@event);
            DomainEvents.Publish(@event);
        }
        public bool IsCanceled()
        {
            return Status == Status.Canceled;
        }

        public void Apply(AppointmentScheduled @event)
        {
            if (@event.From != null && @event.To != null)
                TimeSlot = new TimeSlot(@event.From.Value, @event.To.Value);

            Id = @event.AppointmentId;
            JobId = @event.JobId;
            StaffMemberId = @event.StaffMemberId;
            Status = Status.Initiated;
        }

        public void Apply(AppointmentStarted @event)
        {
            Status = Status.InProgress;
        }

        public void Apply(AppointmentCompleted @event)
        {
            Status = Status.Completed;
            Comments = @event.Comments;
        }

        public void Apply(AppointmentCancelled @event)
        {
            Status = Status.Canceled;
        }

        public void Apply(StaffAssignedToAppointment @event)
        {
            StaffMemberId = @event.StaffId;
        }

        public void Apply(AppointmentRescheduled @event)
        {
            TimeSlot = new TimeSlot(@event.From, @event.To);
        }
    }
}

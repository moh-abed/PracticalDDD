using System;
using System.Collections.Generic;
using Sample.Domain.Shared;

namespace Sample.Domain.V6
{
    [Serializable]
    public class Appointment : AggregateRoot
    {
        public Guid JobId { get; private set; }

        private TimeSlot TimeSlot { get; set; }
        private Guid? StaffMemberId { get; set; }
        private Status Status { get; set; }
        private string Comments { get; set; }

        private Appointment(Guid id,
                           Guid jobId,
                           DateTime? from = null,
                           DateTime? to = null,
                           Guid? staffMemberId = null) : base(id)
        {
            if (from == null && to == null && staffMemberId == null)
                throw new Exception("You have to either select time frame or select staff member");

            Apply(new AppointmentScheduled(UserProfile.Name, id, jobId, staffMemberId, from, to));
        }

        public static Appointment Schedule(Guid jobId, DateTime from, DateTime to, Guid? memberId = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            return new Appointment(Guid.NewGuid(), jobId, from, to, memberId);
        }

        public void AssignStaffMember(Guid staffMemberId)
        {
            Printer.Print(ConsoleColor.Cyan);

            Apply(new StaffAssignedToAppointment(UserProfile.Name, Id, staffMemberId));
        }

        public void Reschedule(DateTime from, DateTime to)
        {
            Printer.Print(ConsoleColor.Cyan);

            Apply(new AppointmentRescheduled(UserProfile.Name, Id, from, to));
        }

        public void Start()
        {
            Printer.Print(ConsoleColor.Cyan);

            if (Status == Status.Canceled)
                throw new Exception("Appointment is cancelled, can not mark it in progress");

            Apply(new AppointmentStarted(UserProfile.Name, Id));
        }
        public void Complete(string comments = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            Apply(new AppointmentCompleted(UserProfile.Name, Id, comments));
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

            Apply(new AppointmentCancelled(UserProfile.Name, Id));
        }
        public bool IsCanceled()
        {
            return Status == Status.Canceled;
        }

        public void Handle(AppointmentScheduled @event)
        {
            if (@event.From != null && @event.To != null)
                TimeSlot = new TimeSlot(@event.From.Value, @event.To.Value);

            Id = @event.AppointmentId;
            JobId = @event.JobId;
            StaffMemberId = @event.StaffMemberId;
            Status = Status.Initiated;
        }

        public void Handle(AppointmentStarted @event)
        {
            Status = Status.InProgress;
        }

        public void Handle(AppointmentCompleted @event)
        {
            Status = Status.Completed;
            Comments = @event.Comments;
        }

        public void Handle(AppointmentCancelled @event)
        {
            Status = Status.Canceled;
        }

        public void Handle(StaffAssignedToAppointment @event)
        {
            StaffMemberId = @event.StaffId;
        }

        public void Handle(AppointmentRescheduled @event)
        {
            TimeSlot = new TimeSlot(@event.From, @event.To);
        }

        public override bool TryResolveConflicts(IEnumerable<DomainEvent> missingEvents)
        {
            //if (UncommittedEvents.Any(IsConflictingEvent))
            //    return false;

            //if (missingEvents.Any(IsConflictingEvent))
            //    return false;

            return true;
        }

        private bool IsConflictingEvent(IDomainEvent e)
        {
            return e.GetType() == typeof (AppointmentStarted) || e.GetType() == typeof (AppointmentCompleted);
        }
    }
}

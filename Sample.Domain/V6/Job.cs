using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Sample.Domain.Shared;

namespace Sample.Domain.V6
{
    [Serializable]
    public class Job : AggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public Status Status { get; private set; }
        public string Location { get; private set; }

        private Job(Guid id, Customer customer, string location = null)
            : base(id)
        {
            if (customer.IsBlackListed)
                throw new Exception("Customer is black listed");

            Apply(new JobCreated(UserProfile.Name, id, customer.Id, location));
        }

        public void Relocate(string location)
        {
            Printer.Print(ConsoleColor.Cyan);

            Apply(new JobRelocated(UserProfile.Name, Id, location));
        }

        public void Start()
        {
            Printer.Print(ConsoleColor.Cyan);

            Apply(new JobStarted(UserProfile.Name, Id));
        }
        public void Complete()
        {
            Printer.Print(ConsoleColor.Cyan);

            Apply(new JobCompleted(UserProfile.Name, Id));
        }
        public bool IsInProgress()
        {
            return Status == Status.InProgress;
        }

        public void Cancel()
        {
            Printer.Print(ConsoleColor.Cyan);

            Apply(new JobCancelled(UserProfile.Name, Id));
        }
        public bool IsCanceled()
        {
            return Status == Status.Canceled;
        }

        public static Job Create(Customer customer, string location = null)
        {
            Printer.Print(ConsoleColor.Cyan);

            return new Job(Guid.NewGuid(), customer, location);
        }

        public void Handle(JobCreated @event)
        {
            Id = @event.JobId;
            CustomerId = @event.CustomerId;
            Location = @event.Location;
            Status = Status.Initiated;
        }

        public void Handle(JobRelocated @event)
        {
            Location = @event.Location;
        }

        public void Handle(JobStarted @event)
        {
            Status = Status.InProgress;
        }

        public void Handle(JobCompleted @event)
        {
            Status = Status.Completed;
        }

        public void Handle(JobCancelled @event)
        {
            Status = Status.Canceled;
        }
    }

    public class JobViewDenormalizer
    {
        public void Execute(DomainEvent @event)
        {
           if(@event.GetType() == typeof(JobCreated)) Execute(@event as JobCreated);
           else if (@event.GetType() == typeof(AppointmentScheduled)) Execute(@event as AppointmentScheduled);
           else if (@event.GetType() == typeof(StaffAssignedToAppointment)) Execute(@event as StaffAssignedToAppointment);
           else if (@event.GetType() == typeof(AppointmentRescheduled)) Execute(@event as AppointmentRescheduled);
           else throw new Exception("No handler for such event type");
        }

        public void Execute(JobCreated @event)
        {
            Printer.Print("Updating JobView in effect of JobCreated event", ConsoleColor.Magenta);

            var customerRepository = new MyRepository<Customer>();
            var customer = customerRepository.Fetch(@event.CustomerId);

            var jobView = new JobView
            {
                Id = @event.JobId,
                CustomerName = customer.Name.ToString(),
                Location = @event.Location
            };

            var jobViewRepository = new MyRepository<JobView>();
            jobViewRepository.Add(jobView);

            Publish(jobView);
        }

        public void Execute(AppointmentScheduled @event)
        {
            Printer.Print("Updating JobView in effect of AppointmentScheduled event", ConsoleColor.Magenta);

            var staffRepository = new MyRepository<StaffMember>();
            var staff = @event.StaffMemberId.HasValue ? staffRepository.Fetch(@event.StaffMemberId.Value) : null;

            var jobViewRepository = new MyRepository<JobView>();
            var jobView = jobViewRepository.Fetch(@event.JobId);

            jobView.Appointments.Add(new AppointmentView
            {
                Id = @event.AppointmentId,
                AssignedTo = staff == null ? null : staff.Name,
                From = @event.From,
                To = @event.To
            });

            jobViewRepository.Update(jobView);

            Publish(jobView);
        }

        public void Execute(StaffAssignedToAppointment @event)
        {
            Printer.Print("Updating JobView in effect of StaffAssignedToAppointment event", ConsoleColor.Magenta);

            var staffRepository = new MyRepository<StaffMember>();
            var staff = staffRepository.Fetch(@event.StaffId);

            var jobViewRepository = new MyRepository<JobView>();
            var jobView = jobViewRepository.FetchAll().Single(j => j.Appointments.Any(a => a.Id == @event.AppointmentId));
            jobView.Appointments.Single(a => a.Id == @event.AppointmentId).AssignedTo = staff.Name;
            
            jobViewRepository.Update(jobView);

            Publish(jobView);
        }

        public void Execute(AppointmentRescheduled @event)
        {
            Printer.Print("Updating JobView in effect of AppointmentRescheduled event", ConsoleColor.Magenta);

            var jobViewRepository = new MyRepository<JobView>();
            var jobView = jobViewRepository.FetchAll().Single(j => j.Appointments.Any(a => a.Id == @event.AppointmentId));
            jobView.Appointments.Single(a => a.Id == @event.AppointmentId).From = @event.From;
            jobView.Appointments.Single(a => a.Id == @event.AppointmentId).To = @event.To;

            jobViewRepository.Update(jobView);

            Publish(jobView);
        }

        private void Publish(JobView job)
        {
            using (var pipeStream = new NamedPipeClientStream("EventSourcingSample"))
            {
                pipeStream.Connect();

                using (var sw = new StreamWriter(pipeStream))
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(JsonConvert.SerializeObject(job, Formatting.Indented));
                }
            }
        }
    }

    public class JobView
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Location { get; set; }

        public List<AppointmentView> Appointments { get; set; }

        public JobView()
        {
            Appointments = new List<AppointmentView>();
        }
    }

    public class AppointmentView
    {
        public Guid Id { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string AssignedTo { get; set; }
    }
}


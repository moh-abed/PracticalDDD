using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Domain.Shared;

namespace Sample.Domain.V6
{
    public static class DomainEvents
    {
        private static readonly Dictionary<Type, List<dynamic>> Subscribers = new Dictionary<Type, List<dynamic>>();

        public static void Publish<T>(T @event) where T : IDomainEvent
        {
            Printer.Print(@event.GetType().Name, ConsoleColor.Green);

            if (!Subscribers.ContainsKey(@event.GetType())) return;

            foreach (var subscriber in Subscribers[@event.GetType()])
            {
                subscriber.Execute(@event);
            }
        }

        public static void Subscribe<T>(object subscriber)
        {
            if (Subscribers.ContainsKey(typeof(T)))
            {
                Subscribers[typeof(T)].Add(subscriber);
            }
            else
            {
                Subscribers.Add(typeof(T), new List<dynamic> { subscriber });
            }
        }
    }

    public class CustomerReadyForPayment : DomainEvent
    {
        public Guid CustomerId { get; set; }

        public CustomerReadyForPayment(string by, Guid customerId) : base(by)
        {
            CustomerId = customerId;
        }
    }

    public class CustomerAccountCharged : DomainEvent
    {
        public Guid CustomerId { get; set; }

        public CustomerAccountCharged(string by, Guid customerId): base(by)
        {
            CustomerId = customerId;
        }
    }

    [Serializable]
    public class AppointmentScheduled : DomainEvent
    {
        public Guid AppointmentId { get; set; }
        public Guid JobId { get; set; }
        public Guid? StaffMemberId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public AppointmentScheduled(string by, Guid appointmentId, Guid jobId, Guid? staffMemberId, DateTime? from, DateTime? to) : base(by)
        {
            AppointmentId = appointmentId;
            JobId = jobId;
            StaffMemberId = staffMemberId;
            From = from;
            To = to;
        }
    }

    [Serializable]
    public class AppointmentStarted : DomainEvent
    {
        public Guid AppointmentId { get; set; }

        public AppointmentStarted(string by, Guid appointmentId) : base(by)
        {
            AppointmentId = appointmentId;
        }
    }

    [Serializable]
    public class AppointmentCompleted : DomainEvent
    {
        public Guid AppointmentId { get; set; }
        public string Comments { get; set; }

        public AppointmentCompleted(string by, Guid appointmentId, string comments) : base(by)
        {
            AppointmentId = appointmentId;
            Comments = comments;
        }
    }

    [Serializable]
    public class AppointmentCancelled : DomainEvent
    {
        public Guid AppointmentId { get; set; }

        public AppointmentCancelled(string by, Guid appointmentId) : base(by)
        {
            AppointmentId = appointmentId;
        }
    }

    [Serializable]
    public class StaffAssignedToAppointment : DomainEvent
    {
        public Guid AppointmentId { get; set; }
        public Guid StaffId { get; set; }

        public StaffAssignedToAppointment(string by, Guid appointmentId, Guid staffId) : base(by)
        {
            AppointmentId = appointmentId;
            StaffId = staffId;
        }
    }

    [Serializable]
    public class AppointmentRescheduled : DomainEvent
    {
        public Guid AppointmentId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public AppointmentRescheduled(string by, Guid appointmentId, DateTime from, DateTime to) : base(by)
        {
            AppointmentId = appointmentId;
            From = from;
            To = to;
        }
    }

    [Serializable]
    public class JobCreated : DomainEvent
    {
        public Guid JobId { get; set; }
        public Guid CustomerId { get; set; }
        public string Location { get; set; }

        public JobCreated(string by, Guid jobId, Guid customerId, string location)
            : base(by)
        {
            JobId = jobId;
            CustomerId = customerId;
            Location = location;
        }
    }

    [Serializable]
    public class JobStarted : DomainEvent
    {
        public Guid JobId { get; set; }

        public JobStarted(string by, Guid jobId)
            : base(by)
        {
            JobId = jobId;
        }
    }

    [Serializable]
    public class JobCompleted : DomainEvent
    {
        public Guid JobId { get; set; }

        public JobCompleted(string by, Guid jobId)
            : base(by)
        {
            JobId = jobId;
        }
    }

    [Serializable]
    public class JobCancelled : DomainEvent
    {
        public Guid JobId { get; set; }

        public JobCancelled(string by, Guid jobId)
            : base(by)
        {
            JobId = jobId;
        }
    }

    [Serializable]
    public class JobRelocated : DomainEvent
    {
        public Guid JobId { get; set; }
        public string Location { get; set; }

        public JobRelocated(string by, Guid jobId, string location)
            : base(by)
        {
            JobId = jobId;
            Location = location;
        }
    }

    public class VerifyPaymentAccountProcessor
    {
        public void Execute(CustomerReadyForPayment @event)
        {
            Printer.Print(ConsoleColor.Magenta);

            // Charge credit card

            DomainEvents.Publish(new CustomerAccountCharged(UserProfile.Name, @event.CustomerId));
        }
    }

    public class AccountEmailProcessor
    {
        public void Execute(CustomerAccountCharged @event)
        {
            Printer.Print(ConsoleColor.Magenta);
            // Send Email
        }
    }

    public class JobStatusProcessor
    {
        public void Execute(AppointmentStarted @event)
        {
            Printer.Print(ConsoleColor.Magenta);

            var appointmentRepository = new MyRepository<Appointment>();
            var appointment = appointmentRepository.Fetch(@event.AppointmentId);
            var jobRepository = new MyRepository<Job>();
            var job = jobRepository.Fetch(appointment.JobId);

            job.Start();

            //Printer.Print("Job marked as in progress", ConsoleColor.Magenta);
        }

        public void Execute(AppointmentCompleted @event)
        {
            Printer.Print(ConsoleColor.Magenta);

            var appointmentRepository = new MyRepository<Appointment>();
            var appointment = appointmentRepository.Fetch(@event.AppointmentId);
            var appointmentsOfJob = new MyRepository<Appointment>().FetchAll().Where(a => a.JobId == appointment.JobId);
            if (appointmentsOfJob.All(a => a.IsCompleted()))
            {
                var jobRepository = new MyRepository<Job>();
                var job = jobRepository.Fetch(appointment.JobId);

                job.Complete();
            }

            //Printer.Print("Job marked as in progress", ConsoleColor.Magenta);
        }
    }
}

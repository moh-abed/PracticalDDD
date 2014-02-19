using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Domain.Shared;

namespace Sample.Domain.V5
{
    public static class DomainEvents
    {
        private static readonly Dictionary<Type, List<dynamic>> Subscribers = new Dictionary<Type, List<dynamic>>();

        public static void Publish<T>(T @event) where T : IDomainEvent
        {
            Printer.Print(@event.GetType().Name, ConsoleColor.Green);

            if (!Subscribers.ContainsKey(typeof(T))) return;

            foreach (var subscriber in Subscribers[typeof(T)])
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

    public class CustomerReadyForPayment : IDomainEvent
    {
        public Guid CustomerId { get; set; }

        public CustomerReadyForPayment(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid EventId { get; private set; }
    }

    public class CustomerAccountCharged : IDomainEvent
    {
        public Guid CustomerId { get; set; }

        public CustomerAccountCharged(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid EventId { get; private set; }
    }

    public class AppointmentScheduled : IDomainEvent
    {
        public Guid AppointmentId { get; set; }
        public Guid JobId { get; set; }
        public Guid? StaffMemberId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public AppointmentScheduled(Guid appointmentId, Guid jobId, Guid? staffMemberId, DateTime? from, DateTime? to)
        {
            AppointmentId = appointmentId;
            JobId = jobId;
            StaffMemberId = staffMemberId;
            From = from;
            To = to;
        }

        public Guid EventId { get; private set; }
    }

    public class AppointmentStarted : IDomainEvent
    {
        public Guid AppointmentId { get; set; }

        public AppointmentStarted(Guid appointmentId)
        {
            AppointmentId = appointmentId;
        }

        public Guid EventId { get; private set; }
    }

    public class AppointmentCompleted : IDomainEvent
    {
        public Guid AppointmentId { get; set; }
        public string Comments { get; set; }

        public AppointmentCompleted(Guid appointmentId, string comments)
        {
            AppointmentId = appointmentId;
            Comments = comments;
        }

        public Guid EventId { get; private set; }
    }

    public class AppointmentCancelled : IDomainEvent
    {
        public Guid AppointmentId { get; set; }

        public AppointmentCancelled(Guid appointmentId)
        {
            AppointmentId = appointmentId;
        }

        public Guid EventId { get; private set; }
    }

    public class StaffAssignedToAppointment : IDomainEvent
    {
        public Guid AppointmentId { get; set; }
        public Guid StaffId { get; set; }

        public StaffAssignedToAppointment(Guid appointmentId, Guid staffId)
        {
            AppointmentId = appointmentId;
            StaffId = staffId;
        }

        public Guid EventId { get; private set; }
    }

    public class AppointmentRescheduled : IDomainEvent
    {
        public Guid AppointmentId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public AppointmentRescheduled(Guid appointmentId, DateTime from, DateTime to)
        {
            AppointmentId = appointmentId;
            From = from;
            To = to;
        }

        public Guid EventId { get; private set; }
    }

    public class VerifyPaymentAccountProcessor
    {
        public void Execute(CustomerReadyForPayment @event)
        {
            Printer.Print(ConsoleColor.Magenta);

            // Charge credit card

            DomainEvents.Publish(new CustomerAccountCharged(@event.CustomerId));
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

                job.Finish();
            }

            //Printer.Print("Job marked as in progress", ConsoleColor.Magenta);
        }
    }
}

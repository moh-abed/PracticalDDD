using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Domain.Shared;

namespace Sample.Domain.V4
{
    public static class DomainEvents
    {
        private static readonly Dictionary<Type, List<dynamic>> Subscribers = new Dictionary<Type, List<dynamic>>();

        public static void Publish<T>(T @event)
        {
            foreach (var subscriber in Subscribers[typeof(T)])
            {
                subscriber.Execute(@event);
            }
        }

        public static void Subscribe<T>(object subscriber)
        {
            if (Subscribers.ContainsKey(typeof (T)))
            {
                Subscribers[typeof (T)].Add(subscriber);
            }
            else
            {
                Subscribers.Add(typeof (T), new List<dynamic>{subscriber});
            }
        }
    }

    public class CustomerReadyForPayment
    {
        public Customer Customer { get; set; }

        public CustomerReadyForPayment(Customer customer)
        {
            Printer.Print(ConsoleColor.Cyan);

            this.Customer = customer;
        }
    }

    public class CustomerAccountCharged
    {
        public Customer Customer { get; set; }

        public CustomerAccountCharged(Customer customer)
        {
            Printer.Print(ConsoleColor.Cyan);

            this.Customer = customer;
        }
    }

    public class AppointmentStarted
    {
        public Appointment Appointment { get; set; }

        public AppointmentStarted(Appointment appointment)
        {
            Printer.Print(ConsoleColor.Cyan);

            this.Appointment = appointment;
        }
    }

    public class AppointmentCompleted
    {
        public Appointment Appointment { get; set; }

        public AppointmentCompleted(Appointment appointment)
        {
            Printer.Print(ConsoleColor.Cyan);

            this.Appointment = appointment;
        }
    }

    public class VerifyPaymentAccountProcessor
    {
        public void Execute(CustomerReadyForPayment @event)
        {
            Printer.Print(ConsoleColor.Magenta);

            // Charge credit card
            
            DomainEvents.Publish(new CustomerAccountCharged(@event.Customer));
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

            var jobRepository = new MyRepository<Job>();
            var job = jobRepository.Fetch(@event.Appointment.JobId);

            job.Start();

            //Printer.Print("Job marked as in progress", ConsoleColor.Magenta);
        }

        public void Execute(AppointmentCompleted @event)
        {
            Printer.Print(ConsoleColor.Magenta);

            var appointmentsOfJob = new MyRepository<Appointment>().FetchAll().Where(a => a.JobId == @event.Appointment.JobId);
            if (appointmentsOfJob.All(a => a.IsCompleted()))
            {
                var jobRepository = new MyRepository<Job>();
                var job = jobRepository.Fetch(@event.Appointment.JobId);

                job.Finish();
            }

            //Printer.Print("Job marked as in progress", ConsoleColor.Magenta);
        }
    }
}

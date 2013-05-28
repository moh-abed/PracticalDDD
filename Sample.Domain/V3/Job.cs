using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Domain.Shared;

namespace Sample.Domain.V3
{
    public class Job
    {
        private readonly List<Appointment> appointments = new List<Appointment>();

        public Guid Id { get; private set; }
        public Customer Customer { get; private set; }
        public Status Status { get; private set; }
        public string Location { get; private set; }
        public IEnumerable<Appointment> Appointments { get { return appointments; } } 
        
        private Job(Guid id, Customer customer, string location = null)
        {
            if (customer == null)
                throw new Exception("Customer should be provided");

            Id = id;
            Customer = customer;
            Location = location;
            Status = Status.Initiated;
        }

        public void UpdateLocation(string location)
        {
            Printer.Print(ConsoleColor.Cyan);

            Location = location;
        }

        public void ScheduleAppointment(Appointment appointment)
        {
            appointments.Add(appointment);
        }

        public void UnscheduleAppiontment(Appointment appointment)
        {
            appointments.Remove(appointment);
        }

        public Appointment GetAppointment(Guid id)
        {
            return Appointments.Single(a => a.Id == id);
        }

        public void UpdateAppointment(Guid id, Action<Appointment> callback)
        {
            callback(Appointments.Single(a => a.Id == id));
        }

        public void InProgress()
        {
            Printer.Print(ConsoleColor.Cyan);

            if(Status == Status.Canceled)
                throw new Exception("Job is cancelled, can not mark it in progress");

            Status = Status.InProgress;
        }
        public bool IsInProgress()
        {
            return Status == Status.InProgress;
        }

        public void Finished()
        {
            Printer.Print(ConsoleColor.Cyan);
            Status = Status.Finished;
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

        public static Job Create(Customer customer, string location = null)
        {
            Printer.Print(ConsoleColor.Cyan);
            
            return new Job(Guid.NewGuid(), customer, location);
        }
    }
}


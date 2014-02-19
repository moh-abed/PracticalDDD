using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V5
{
    public class Job
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public Status Status { get; private set; }
        public string Location { get; private set; }
        
        private Job(Guid id, Guid customerId, string location = null)
        {
            Id = id;
            CustomerId = customerId;
            Location = location;
            Status = Status.Initiated;
        }

        public void UpdateLocation(string location)
        {
            Printer.Print(ConsoleColor.Cyan);
            Location = location;
        }

        public void Start()
        {
            Printer.Print(ConsoleColor.Cyan);
            Status = Status.InProgress;
        }
        public void Finish()
        {
            Printer.Print(ConsoleColor.Cyan);
            Status = Status.Completed;
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

        public static Job Create(Guid customerId, string location = null)
        {
            Printer.Print(ConsoleColor.Cyan);
            
            return new Job(Guid.NewGuid(), customerId, location);
        }
    }
}


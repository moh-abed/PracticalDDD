using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Domain.Shared;
using v1 = Sample.Domain.V1;
using v2 = Sample.Domain.V2;
using v3 = Sample.Domain.V3;
using v4 = Sample.Domain.V4;

namespace Sample
{
    class Program
    {
        static void Main()
        {
            try
            {
                PrintHeader("Version 1");
                Version1();

                PrintHeader("Version 2");
                Version2();

                PrintHeader("Version 3");
                Version3();

                PrintHeader("Version 4");
                Version4();
            }
            catch (Exception exception)
            {
                Printer.Print(exception.Message, ConsoleColor.Red);
            }
            Console.Read();
        }

        private static void Version1()
        {
            var tom = new v1.StaffMember { Id = Guid.NewGuid(), Name = "Tom"};
            var jack = new v1.StaffMember { Id = Guid.NewGuid(), Name = "Jack" };
            Repository<v1.StaffMember>().Add(tom);
            Repository<v1.StaffMember>().Add(jack);

            var customer = new v1.Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    FamilyName = "Smith"
                };
            
            var job = new v1.Job
                {
                    Id = Guid.NewGuid(),
                    Status = Status.Initiated,
                    Customer = customer,
                    Location = "1 George Street, blablabla"
                };

            job.Appointments = new List<v1.Appointment>
                {
                     new v1.Appointment
                                {
                                    Id = Guid.NewGuid(),
                                    From = DateTime.Now,
                                    To = DateTime.Now.AddHours(2),
                                    StaffMember = tom
                                }
                };

            customer.FirstName = "Michael";
            customer.FamilyName = "Harris";
            customer.Email = "my@email.com";
            customer.Phone = "123456";
            customer.NameOnCreditCard = "Michael";
            customer.CreditCardNumber = "123 456 789";

            var appointment = job.Appointments.First();
            appointment.StaffMember = jack;
            appointment.From = DateTime.Now.AddDays(1);
            appointment.To = DateTime.Now.AddDays(1).AddHours(2);

            Repository<v1.Customer>().Add(customer);
            Repository<v1.Job>().Add(job);

            appointment.Status = Status.InProgress;
            job.Status = Status.InProgress;

            appointment.Status = Status.Finished;
            if (job.Appointments.All(a => a.Status != Status.InProgress))
                job.Status = Status.Finished; 

            var creditValidator = new CreditValidationService();
            if (creditValidator.IsValid(customer.NameOnCreditCard, customer.CreditCardNumber))
            {
                new BillingService().Pay(customer.NameOnCreditCard, customer.CreditCardNumber);
                new NotificationService().SendEmail(customer.Email, "Invoice charged", "Bla bla bla");
            }
            else
            {
                new NotificationService().SendEmail(customer.Email, "Credit not valid", "Bla bla bla");
            }
        }

        private static void Version2()
        {
            var tom = v2.StaffMember.Register("Tom");
            var jack = v2.StaffMember.Register("Jack");
            Repository<v2.StaffMember>().Add(tom);
            Repository<v2.StaffMember>().Add(jack);

            var customer = v2.Customer.Register("John", "Smith", "any@email.com");

            var job = v2.Job.Create(customer, "1 George Street");
            var appointment = v2.Appointment.ScheduleNew(DateTime.Now, DateTime.Now.AddHours(2), tom);
            job.Appointments = new List<v2.Appointment>{ appointment };

            customer.Name = new v2.FullName("Michael", "Harris");
            customer.Email = "my@email.com";
            customer.Phone = "123456";
            customer.CreditCard = new v2.CreditCardDetails("Michael", "123 456 789");

            appointment = job.Appointments.Single(a => a.Id == appointment.Id);
            appointment.StaffMember = jack;
            appointment.TimeSlot = new v2.TimeSlot(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(2));

            Repository<v2.Customer>().Add(customer);
            Repository<v2.Job>().Add(job);

            appointment.Status = Status.InProgress;
            job.Status = Status.InProgress;

            appointment.Status = Status.Finished;
            if (job.Appointments.All(a => a.Status != Status.InProgress))
                job.Status = Status.Finished; 

            var creditValidator = new CreditValidationService();
            if (creditValidator.IsValid(customer.CreditCard.NameOnCard, customer.CreditCard.CardNumber))
            {
                new BillingService().Pay(customer.CreditCard.NameOnCard, customer.CreditCard.CardNumber);
                new NotificationService().SendEmail(customer.Email, "Invoice charged", "Bla bla bla");
            }
            else
            {
                new NotificationService().SendEmail(customer.Email, "Credit not valid", "Bla bla bla");
            }
        }

        private static void Version3()
        {
            var tom = v3.StaffMember.Register("Tom");
            var jack = v3.StaffMember.Register("Jack");
            Repository<v3.StaffMember>().Add(tom);
            Repository<v3.StaffMember>().Add(jack);

            var customer = v3.Customer.Register("John", "Smith", "any@email.com");

            var job = v3.Job.Create(customer, "1 George Street");
            var appointment = v3.Appointment.New(DateTime.Now, DateTime.Now.AddHours(2), tom);
            job.ScheduleAppointment(appointment);

            customer.Rename("Michael", "Harris");
            customer.UpdateContact("john2@smith.com", "12335679");
            customer.UpdateCrediCard("John Smith", "123 356 789 653");

            string error;
            if (!customer.TryUpdateContact("bademail", "123456", out error))
                Printer.Print(error, ConsoleColor.Red);

            job.UpdateAppointment(appointment.Id, a => a.AssignStaffMember(jack));
            job.UpdateAppointment(appointment.Id, a => a.Reschedule(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(2)));

            Repository<v3.Customer>().Add(customer);
            Repository<v3.Job>().Add(job);

            job.UpdateAppointment(appointment.Id, a => a.InProgress());
            job.InProgress();     // Dependent logic leaked

            job.UpdateAppointment(appointment.Id, a => a.Finished());
            if (job.Appointments.All(a => !a.IsInProgress())) // Dependent logic leaked
                job.Finished(); 
           
            // Option 1: double dispatch to service
            //customer.ValidateCreditCard(new CreditValidationService());

            // Option 2: double dispatch to specifications
            if (customer.ValidateCreditCard(new v3.ValidCustomerCreditCardSpecification()))
            {
                customer.Charge(new v3.MyCustomerPaymentService(), new NotificationService());
            }

            // Option 3: use specifications explicitly
            //var validCreditSpecs = new ValidCustomerCreditCardSpecification();
            //if (validCreditSpecs.IsSatisfiedBy(customer))
            //{
            //    customer.Pay(new MyCustomerPaymentService(), new NotificationService());
            //}
        }

        private static void Version4()
        {
            RegisterEventProcessors();

            var tom = v4.StaffMember.Register("Tom");
            var jack = v4.StaffMember.Register("Jack");
            Repository<v4.StaffMember>().Add(tom);
            Repository<v4.StaffMember>().Add(jack);

            var customer = v4.Customer.Register("John", "Smith", "john@smith.com");
           
            var job = v4.Job.Create(customer.Id, "1 George Street");
            
            var appointment = v4.Appointment.Schedule(job.Id, DateTime.Now, DateTime.Now.AddHours(2), tom.Id);
            
            customer.Rename("Michael", "Harris");
            customer.UpdateContact("john2@smith.com", "12344679");
            customer.UpdateCrediCard("John Smith", "123 446 789 644");

            appointment.AssignStaffMember(jack.Id);
            appointment.Reschedule(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(2));
           
            Repository<v4.Customer>().Add(customer);
            Repository<v4.Job>().Add(job);
            Repository<v4.Appointment>().Add(appointment);

            appointment.Start();

            appointment.Finish("Some comments");

            customer.Charge();
        }

        private static void RegisterEventProcessors()
        {
            v4.DomainEvents.Subscribe<v4.CustomerReadyForPayment>(new v4.VerifyPaymentAccountProcessor());
            v4.DomainEvents.Subscribe<v4.CustomerAccountCharged>(new v4.AccountEmailProcessor());
            v4.DomainEvents.Subscribe<v4.AppointmentStarted>(new v4.JobStatusProcessor());
            v4.DomainEvents.Subscribe<v4.AppointmentFinished>(new v4.JobStatusProcessor());
        }

        private static void PrintHeader(string header)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("===================================================");
            Console.WriteLine(header);
            Console.WriteLine("===================================================");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static MyRepository<T> Repository<T>() where T : class
        {
            return new MyRepository<T>();
        }
    }
}
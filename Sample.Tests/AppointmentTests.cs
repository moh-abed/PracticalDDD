using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Domain.Shared;
using Sample.Domain.V6;

namespace Sample.Tests
{
    [TestClass]
    public class AppointmentTests : EventSourcedTestCase<Appointment>
    {
        [TestMethod]
        public void AssigningStaffMemberShouldRaiseStaffMemberAssignedEvent()
        {
            var jobId = Guid.NewGuid();
            var appointmentId = Guid.NewGuid();
            var staffMemberId = Guid.NewGuid();

            Given(appointmentId, new AppointmentScheduled(UserProfile.Name, appointmentId, jobId, null, DateTime.Now.Date, DateTime.Now.Date.AddHours(2)))
                .When(appointment => appointment.AssignStaffMember(staffMemberId))
                .Then(new StaffAssignedToAppointment(UserProfile.Name, appointmentId, staffMemberId));
        }

        [TestMethod]
        public void StartingCancelledAppointmentShouldNotRaiseEvent()
        {
            var jobId = Guid.NewGuid();
            var appointmentId = Guid.NewGuid();

            Given(appointmentId,
                    new DomainEvent[]
                    {
                        new AppointmentScheduled(UserProfile.Name, appointmentId, jobId, null, DateTime.Now.Date, DateTime.Now.Date.AddHours(2)),
                        new AppointmentCancelled(UserProfile.Name, appointmentId)
                    })
                .When(appointment => appointment.Start())
                .ThenItThrows<Exception>();
        }
    }
}

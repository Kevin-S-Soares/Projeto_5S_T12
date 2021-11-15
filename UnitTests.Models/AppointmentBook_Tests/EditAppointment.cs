using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.AppointmentBook_Tests
{
    [TestClass]
    public class EditAppointment
    {
        private readonly AppointmentServiceDependecy _appointmentService = new AppointmentServiceDependecy();
        private readonly IDentistService _dentistService = new DentistServiceDependecy();
        private readonly TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();

        AppointmentBook Model;

        [TestInitialize]
        public void Initialize()
        {
            Model = new AppointmentBook(_appointmentService, _dentistService, _timeZoneService);
        }

        [TestMethod]
        public async Task EditingAppointments_Succeed()
        {
            Appointment oldAppointment = await _appointmentService.FindByIdAsync(18);
            Appointment newAppointment = GetSuccessfulAppointment();

            await Model.EditAppointment(oldAppointment, newAppointment);

        }

        [TestMethod]
        public async Task AppointmentWithNonExistingDentistAndCorrectAppointment_DomainException()
        {
            Appointment oldAppointment = GetAppointmentWithNonExistentDentist();
            Appointment newAppointment = GetSuccessfulAppointment();
            try
            {
                await Model.EditAppointment(oldAppointment, newAppointment);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Dentista inexistente!", e.Message);
            }
        }

        [TestMethod]
        public async Task CorrectAppointmentAndAppointmentWithNonExistingDentist_DomainException()
        {
            Appointment oldAppointment = await _appointmentService.FindByIdAsync(18);
            Appointment newAppointment = GetAppointmentWithNonExistentDentist();
            try
            {
                await Model.EditAppointment(oldAppointment, newAppointment);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Dentista inexistente!", e.Message);
            }
        }

        [TestMethod]
        public async Task NullAppointmentAndCorrectAppointment_DomainException()
        {
            Appointment oldAppointment = null;
            Appointment newAppointment = await _appointmentService.FindByIdAsync(18);
            try
            {
                await Model.EditAppointment(oldAppointment, newAppointment);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        [TestMethod]
        public async Task CorrectAppointmentAndNullAppointment_DomainException()
        {
            Appointment oldAppointment = await _appointmentService.FindByIdAsync(18);
            Appointment newAppointment = null;
            try
            {
                await Model.EditAppointment(oldAppointment, newAppointment);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        private Appointment GetSuccessfulAppointment()
        {
            return new Appointment()
            {
                Id = 0,
                Date = _timeZoneService.GetTomorrowOnly(),
                DentistId = 1,
                DurationInMinutes = 60,
                Time = new TimeSpan(10, 0, 0)
            };
        }
        private Appointment GetAppointmentWithNonExistentDentist()
        {
            return new Appointment()
            {
                Id = 0,
                Date = _timeZoneService.GetTomorrowOnly(),
                DentistId = -1,
                DurationInMinutes = 60,
                Time = new TimeSpan(10, 0, 0)
            };
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.AppointmentBook_Tests
{
    [TestClass]
    public class RemoveAppointment
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
        public async Task Int_RemovingAppointment_Succeed()
        {
            await Model.RemoveAppointment(4);
        }

        [TestMethod]
        public async Task Int_RemovingNonExistentAppointment_DomainException()
        {
            try
            {
                await Model.RemoveAppointment(-1);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        [TestMethod]
        public async Task Appointment_RemovingAppointment_Succeed()
        {
            Appointment appointment = await GetExistingAppointment();
            await Model.RemoveAppointment(appointment);
        }
        private async Task<Appointment> GetExistingAppointment()
        {
            return await _appointmentService.FindByIdAsync(1);
        }

        [TestMethod]
        public async Task Appointment_RemovingAppointmentWithNonExistentDentist_DomainException()
        {
            Appointment appointment = GetAppointmentWithNonExistentDentist();
            try
            {
                await Model.RemoveAppointment(appointment);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Dentista inexistente!", e.Message);
            }
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

        [TestMethod]
        public async Task Appointment_RemovingNullAppointment_DomainException()
        {
            try
            {
                await Model.RemoveAppointment(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }
    }
}

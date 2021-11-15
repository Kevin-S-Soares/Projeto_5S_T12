using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.AppointmentBook_Tests
{
    [TestClass]
    public class AddAppointment
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
        public async Task AddingAppointment_Succeed()
        {
            _timeZoneService.ChangeToNineteen();
            Appointment appointment = GetSuccessfulAppointment();
            await Model.AddAppointment(appointment);

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

        [TestMethod]
        public async Task AddingAppointmentWithNonExistentDentist_DomainException()
        {
            try
            {
                Appointment appointment = GetAppointmentWithNonExistentDentist();
                await Model.AddAppointment(appointment);
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
        public async Task AddingAppointmentOfAPastDate_DomainException()
        {
            try
            {
                Appointment appointment = GetAppointmentOfAPasteDate();
                await Model.AddAppointment(appointment);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Data inválida!", e.Message);
            }
        }
        private Appointment GetAppointmentOfAPasteDate()
        {
            return new Appointment()
            {
                Id = 0,
                Date = _timeZoneService.GetYesterdayOnly(),
                DentistId = 1,
                DurationInMinutes = 60,
                Time = new TimeSpan(10, 0, 0)
            };
        }

        [TestMethod]
        public async Task AddingAppointmentOfAPastTime_DomainException()
        {
            _timeZoneService.ChangeToFifteen();
            try
            {
                Appointment appointment = GetAppointmentOfAPasteTime();
                await Model.AddAppointment(appointment);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Data inválida!", e.Message);
            }
        }
        private Appointment GetAppointmentOfAPasteTime()
        {
            return new Appointment()
            {
                Id = 0,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = 1,
                DurationInMinutes = 60,
                Time = new TimeSpan(14, 45, 0)
            };
        }

        [TestMethod]
        public async Task AddingNullAppointment_DomainException()
        {
            try
            {
                await Model.AddAppointment(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }
    }
}

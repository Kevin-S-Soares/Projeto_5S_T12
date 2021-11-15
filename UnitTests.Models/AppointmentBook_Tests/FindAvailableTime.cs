using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.AppointmentBook_Tests
{
    [TestClass]
    public class FindAvailableTime
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
        public async Task FindAvailableTime_AppointmentWithNonExistingDentist_DomainException()
        {
            Appointment appointment = GetAppointmentWithNonExistentDentist();
            try
            {
                await Model.FindAvailableTime(appointment);
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
        public async Task FindAvailableTime_NullAppointment_DomainException()
        {
            try
            {
                await Model.FindAvailableTime(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        [TestMethod]
        public async Task FindAvailableTime_FindingAvailableTimeBeforeNine_IdenticalList()
        {
            List<TimeSpan> correctList = GetListOfTimes_BeforeNine();
            Appointment appointment = await GetAppointmentAtNine();
            List<TimeSpan> beingTested = await Model.FindAvailableTime(appointment);
            CollectionAssert.AreEqual(correctList, beingTested);
        }
        private async Task<Appointment> GetAppointmentAtNine()
        {
            return new Appointment()
            {
                Id = 255,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = 3,
                Dentist = await _dentistService.FindByIdAsync(3),
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 0, 0)
            };
        }
        private List<TimeSpan> GetListOfTimes_BeforeNine()
        {
            var result = new List<TimeSpan>();
            for (int i = 0; i < 12; i++)
            {
                result.Add(new TimeSpan(9, i * 15, 0));
            }
            for (int i = 16; i < 36; i++)
            {
                result.Add(new TimeSpan(9, i * 15, 0));
            }
            return result;
        }

        [TestMethod]
        public async Task FindAvailableTime_FindingAvailableTimeAfterFifteen_IdenticalList()
        {
            _timeZoneService.ChangeToFifteen();
            List<TimeSpan> correctList = GetListOfTimes_AfterFifteen();
            Appointment appointment = await GetAppointmentAtNine();
            List<TimeSpan> beingTested = await Model.FindAvailableTime(appointment);
            CollectionAssert.AreEqual(correctList, beingTested);
        }
        private List<TimeSpan> GetListOfTimes_AfterFifteen()
        {
            var result = new List<TimeSpan>();
            for (int i = 24; i < 36; i++)
            {
                result.Add(new TimeSpan(9, i * 15, 0));
            }
            return result;
        }

        [TestMethod]
        public async Task FindAvailableTime_FindingAvailableTimeAfterNineteen_IdenticalList()
        {
            _timeZoneService.ChangeToNineteen();
            List<TimeSpan> correctList = GetListOfTimes_AfterNineteen();
            Appointment appointment = await GetAppointmentAtNine();
            List<TimeSpan> beingTested = await Model.FindAvailableTime(appointment);
            CollectionAssert.AreEqual(correctList, beingTested);
        }
        private List<TimeSpan> GetListOfTimes_AfterNineteen()
        {
            return new List<TimeSpan>();
        }
    }
}

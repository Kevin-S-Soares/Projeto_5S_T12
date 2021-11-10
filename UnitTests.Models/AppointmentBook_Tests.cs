using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Models.AppointmentBookDependecies;
using WebOdontologista.Models;
using WebOdontologista.Models.Exceptions;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models
{
    [TestClass]
    public class AppointmentBook_Tests
    {
        /*
         * TODO
         * Everything is right
         * If Dentist Doest Exist
         * Expected Result:
         * Before 9:00
         * After 15:00
         * After 18:00
         */

        private AppointmentServiceDependecy _appointmentService = new AppointmentServiceDependecy();
        private IDentistService _dentistService = new DentistServiceDependecy();
        TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();

        AppointmentBook Model;

        [TestInitialize]
        public void Initialize()
        {
            Model = new AppointmentBook(_appointmentService, _dentistService, _timeZoneService);
        }

        [TestMethod]
        public async Task AddAppointment_AddingAppointment_Succeed()
        {
            try
            {
                Appointment toSucceed = new Appointment()
                {
                    Id = 0,
                    Date = _timeZoneService.GetTomorrowOnly(),
                    DentistId = 1,
                    DurationInMinutes = 60,
                    Time = new TimeSpan(10, 0, 0)
                };
                await Model.AddAppointment(toSucceed);
            }
            catch(Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task AddAppointment_AddingAppointmentWithNonExistentDentist_DomainException()
        {
            try
            {
                Appointment toTriggerException = new Appointment()
                {
                    Id = 0,
                    Date = _timeZoneService.GetTomorrowOnly(),
                    DentistId = -1,
                    DurationInMinutes = 60,
                    Time = new TimeSpan(10, 0, 0)
                };
                await Model.AddAppointment(toTriggerException);
                Assert.Fail();
            }
            catch (DomainException e)
            {
                Assert.AreEqual("Dentista inexistente!", e.Message);
            }
        }

        [TestMethod]
        public async Task AddAppointment_AddingAppointmentOfAPastDate_DomainException()
        {
            try
            {
                Appointment toTriggerException = new Appointment()
                {
                    Id = 0,
                    Date = _timeZoneService.GetYesterdayOnly(),
                    DentistId = 1,
                    DurationInMinutes = 60,
                    Time = new TimeSpan(10, 0, 0)
                };
                await Model.AddAppointment(toTriggerException);
                Assert.Fail();
            }
            catch (DomainException e)
            {
                Assert.AreEqual("Data inválida!", e.Message);
            }
        }

        [TestMethod]
        public async Task AddAppointment_AddingAppointmentOfAPastTime_DomainException()
        {
            _timeZoneService.ChangeToFifteen();
            try
            {
                Appointment toTriggerException = new Appointment()
                {
                    Id = 0,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    DurationInMinutes = 60,
                    Time = new TimeSpan(14, 45, 0)
                };
                await Model.AddAppointment(toTriggerException);
                Assert.Fail();
            }
            catch (DomainException e)
            {
                Assert.AreEqual("Data inválida!", e.Message);
            }
        }

        [TestMethod]
        public async Task RemoveAppointment_CancellingAppointmentIntParam_Succeed()
        {
            try
            {
                await Model.RemoveAppointment(4);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RemoveAppointment_CancellingAppointmentAppointmentParam_Succeed()
        {
            Appointment toRemove = new Appointment()
            {
                Id = 4,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = 1,
                Dentist = await _dentistService.FindByIdAsync(1),
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 45, 0)
            };
            try
            {
                await Model.RemoveAppointment(toRemove);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task EditingAppointment_CancellingAppointmentIntParam_Succeed()
        {
            try
            {
                await Model.EditingAppointment(4);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task EditingAppointment_CancellingAppointmentAppointmentParam_Succeed()
        {
            Appointment toRemove = new Appointment()
            {
                Id = 4,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = 1,
                Dentist = await _dentistService.FindByIdAsync(1),
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 45, 0)
            };
            try
            {
                await Model.EditingAppointment(toRemove);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task FindAvailableTime_FindingAvailableTimeBeforeNine_IdenticalList()
        {
            List<TimeSpan> correctList = BeforeNine();
            Appointment appointment = new Appointment()
            {
                Id = 255,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = 3,
                Dentist = await _dentistService.FindByIdAsync(3),
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 0, 0)
            };
            List<TimeSpan> beingTested = await Model.FindAvailableTime(appointment);
            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task FindAvailableTime_FindingAvailableTimeAfterFifteen_IdenticalList()
        {
            _timeZoneService.ChangeToFifteen();
            List<TimeSpan> correctList = AfterFifteen();
            Appointment appointment = new Appointment()
            {
                Id = 255,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = 3,
                Dentist = await _dentistService.FindByIdAsync(3),
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 0, 0)
            };
            List<TimeSpan> beingTested = await Model.FindAvailableTime(appointment);
            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task FindAvailableTime_FindingAvailableTimeAfterNineteen_IdenticalList()
        {
            _timeZoneService.ChangeToNineteen();
            List<TimeSpan> correctList = AfterNineteen();
            Appointment appointment = new Appointment()
            {
                Id = 255,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = 3,
                Dentist = await _dentistService.FindByIdAsync(3),
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 0, 0)
            };
            List<TimeSpan> beingTested = await Model.FindAvailableTime(appointment);
            CollectionAssert.AreEqual(correctList, beingTested);
        }

        private List<TimeSpan> BeforeNine()
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
        private List<TimeSpan> AfterFifteen()
        {
            var result = new List<TimeSpan>();
            for (int i = 24; i < 36; i++)
            {
                result.Add(new TimeSpan(9, i * 15, 0));
            }
            return result;
        }
        private List<TimeSpan> AfterNineteen()
        {
            return new List<TimeSpan>();
        }

    }
}

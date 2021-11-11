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
            _timeZoneService.ChangeToNineteen();
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
            catch (Exception e)
            {
                Assert.Fail(e.Message);
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
        public async Task AddAppointment_AddingNullAppointment_DomainException()
        {
            try
            {
                await Model.AddAppointment(null);
                Assert.Fail();
            }
            catch (DomainException e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        [TestMethod]
        public async Task RemoveAppointment_Int_RemovingAppointment_Succeed()
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
        public async Task RemoveAppointment_Int_RemovingNonExistentAppointment_DomainException()
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
        public async Task RemoveAppointment_Appointment_RemovingAppointment_Succeed()
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
        public async Task RemoveAppointment_Appointment_RemovingAppointmentWithNonExistentDentist_DomainException()
        {
            Appointment toRemove = new Appointment()
            {
                Id = 4,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = -1,
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 45, 0)
            };
            try
            {
                await Model.RemoveAppointment(toRemove);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Dentista inexistente!", e.Message);
            }
        }

        [TestMethod]
        public async Task RemoveAppointment_Appointment_RemovingNullAppointment_DomainException()
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

        [TestMethod]
        public async Task EditingAppointment_Int_EditingAppointment_Succeed()
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
        public async Task EditingAppointment_Int_EditingNonExistentAppointment_DomainException()
        {
            try
            {
                await Model.EditingAppointment(-1);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        [TestMethod]
        public async Task EditingAppointment_Appointment_EditingAppointment_Succeed()
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
        public async Task EditingAppointment_Appointment_EditingAppointmentWithNonExistingDentist_DomainException()
        {
            Appointment toRemove = new Appointment()
            {
                Id = 4,
                Date = _timeZoneService.GetTodayOnly(),
                DentistId = -1,
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 45, 0)
            };
            try
            {
                await Model.EditingAppointment(toRemove);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Dentista inexistente!", e.Message);
            }
        }

        [TestMethod]
        public async Task EditingAppointment_Appointment_EditingNullAppointment_DomainException()
        {
            try
            {
                await Model.EditingAppointment(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        [TestMethod]
        public async Task EditAppointment_EditingAppointments_Succeed()
        {
            Appointment oldAppointment = await _appointmentService.FindByIdAsync(18);
            Appointment newAppointment = new Appointment()
            {
                Id = 0,
                DentistId = 2,
                Date = _timeZoneService.GetTodayOnly(),
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 60
            };
            try
            {
                await Model.EditAppointment(oldAppointment, newAppointment);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task EditAppointment_AppointmentWithNonExistingDentistAndCorrectAppointment_DomainException()
        {
            Appointment oldAppointment = new Appointment()
            {
                Id = 0,
                DentistId = -1,
                Date = _timeZoneService.GetTodayOnly(),
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 60
            };
            Appointment newAppointment = new Appointment()
            {
                Id = 0,
                DentistId = 2,
                Date = _timeZoneService.GetTodayOnly(),
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 60
            };
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
        public async Task EditAppointment_CorrectAppointmentAndAppointmentWithNonExistingDentist_DomainException()
        {
            Appointment oldAppointment = await _appointmentService.FindByIdAsync(18);
            Appointment newAppointment = new Appointment()
            {
                Id = 0,
                DentistId = -1,
                Date = _timeZoneService.GetTodayOnly(),
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 60
            };
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
        public async Task EditAppointment_NullAppointmentAndCorrectAppointment_DomainException()
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
        public async Task EditAppointment_CorrectAppointmentAndNullAppointment_DomainException()
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

        [TestMethod]
        public async Task FindAvailableTime_AppointmentWithNonExistingDentist_DomainException()
        {
            Appointment appointment = new Appointment()
            {
                Id = 0,
                DentistId = -1,
                Date = _timeZoneService.GetTodayOnly(),
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 60
            };
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

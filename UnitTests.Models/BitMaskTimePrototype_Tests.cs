using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Models;
using WebOdontologista.Models.CollectionTimePrototype;

namespace UnitTests.Models
{
    [TestClass]
    public class BitMaskTimePrototype_Tests
    {
        public BitMaskTimePrototype Model { get; set; }
        private readonly AppointmentServiceDependecy _appointmentService = new AppointmentServiceDependecy();
        private readonly TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Model = new BitMaskTimePrototype(null);
            Model.SetSchedule(null);
        }

        [TestMethod]
        public async Task MakeAppointment_AddingMultipleAppointments_SucceedToAdd()
        {
            try
            {
                foreach (Appointment obj in await AppointmentsToSucceed())
                {
                    Model.MakeAppointment(obj);
                }
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        private async Task<List<Appointment>> AppointmentsToSucceed()
        {
            return await _appointmentService.FindAllAsync(obj => obj.Date == _timeZoneService.GetTodayOnly());
        }

        [TestMethod]
        public async Task MakeAppointment_AddingAppointmentsColliding_DomainException()
        {
            try
            {
                foreach (Appointment obj in await CollidingAppointments())
                {
                    Model.MakeAppointment(obj);
                }
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Não foi possivel adicionar a consulta!", e.Message);
            }
        }
        private async Task<List<Appointment>> CollidingAppointments()
        {
            var list = await AppointmentsToSucceed();
            list.Add(new Appointment()
            {
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 15, 0)
            });
            return list;
        }

        [TestMethod]
        public void MakeAppointment_AddingAppointmentBeforeExpectedTime_DomainException()
        {
            try
            {
                Appointment beforeExpectedTime = GetAppointmentBeforeExpectedTime();
                Model.MakeAppointment(beforeExpectedTime);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta fora dos limites!", e.Message);
            }
        }
        private Appointment GetAppointmentBeforeExpectedTime()
        {
            return new Appointment() { Time = new TimeSpan(8, 45, 0), DurationInMinutes = 30 };
        }

        [TestMethod]
        public void MakeAppointment_AddingAppointmentAfterExpectedTime_DomainException()
        {
            try
            {
                Appointment afterExpectedTime = GetAppointmentAfterExpectedTime();
                Model.MakeAppointment(afterExpectedTime);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta fora dos limites!", e.Message);
            }
        }
        private Appointment GetAppointmentAfterExpectedTime()
        {
            return new Appointment() { Time = new TimeSpan(17, 15, 0), DurationInMinutes = 60 };
        }

        [TestMethod]
        public void MakeAppointment_NullAppointment_DomainException()
        {
            try
            {
                Model.MakeAppointment(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        [TestMethod]
        public async Task CancelAppointment_CancellingMultipleAppointments_SucceedToCancel()
        {
            try
            {
                List<Appointment> list = await AppointmentsToSucceed();
                SetAppointments(list);
                foreach (Appointment obj in list)
                {
                    Model.CancelAppointment(obj);
                }
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        private void SetAppointments(List<Appointment> list)
        {
            foreach (Appointment obj in list)
            {
                Model.MakeAppointment(obj);
            }
        }

        [TestMethod]
        public void CancelAppointment_CancellingNonExistentAppointment_DomainException()
        {
            try
            {
                Appointment nonExistentAppointment = GetNonExistentAppointment();
                Model.CancelAppointment(nonExistentAppointment);
            }
            catch (Exception ae)
            {
                Assert.AreEqual("Cancelamento de consulta proíbido!", ae.Message);
            }
        }
        private Appointment GetNonExistentAppointment()
        {
            return new Appointment() { DurationInMinutes = 15, Time = new TimeSpan(16, 0, 0) };
        }

        [TestMethod]
        public void CancelAppointment_NullAppointment_DomainException()
        {
            try
            {
                Model.CancelAppointment(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }

        [TestMethod]
        public void GetAvailableTimes_FifteenMinutesAppointmentDuration_IdenticalList()
        {
            Appointment appointment = GetFifteenMinutesAppointment();
            List<TimeSpan> beingTested = Model.GetAvailableTimes(appointment);
            List<TimeSpan> correctList = GetListOfAppointments_FifteenMinutesDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        private Appointment GetFifteenMinutesAppointment()
        {
            return new Appointment() { Time = new TimeSpan(9, 0, 0), DurationInMinutes = 15 };
        }
        public List<TimeSpan> GetListOfAppointments_FifteenMinutesDuration()
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
        public void GetAvailableTimes_ThirtyMinutesAppointmentDuration_IdenticalList()
        {
            Appointment appointment = GetThirtyMinutesAppointment();
            List<TimeSpan> beingTested = Model.GetAvailableTimes(appointment);
            List<TimeSpan> correctList = GetListOfAppointments_ThirtyMinutesDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        private Appointment GetThirtyMinutesAppointment()
        {
            return new Appointment() { Time = new TimeSpan(9, 0, 0), DurationInMinutes = 30 };
        }
        private List<TimeSpan> GetListOfAppointments_ThirtyMinutesDuration()
        {
            var result = GetListOfAppointments_FifteenMinutesDuration();
            result.Remove(new TimeSpan(11, 45, 0));
            result.Remove(new TimeSpan(17, 45, 0));
            return result;
        }

        [TestMethod]
        public void GetAvailableTimes_FourtyFiveMinutesAppointmentDuration_IdenticalList()
        {
            Appointment appointment = GetFourtyFiveMinutesAppointment();
            List<TimeSpan> beingTested = Model.GetAvailableTimes(appointment);
            List<TimeSpan> correctList = GetListOfAppointments_FourtyFiveMinutesDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        private Appointment GetFourtyFiveMinutesAppointment()
        {
            return new Appointment() { Time = new TimeSpan(9, 0, 0), DurationInMinutes = 45 };
        }
        private List<TimeSpan> GetListOfAppointments_FourtyFiveMinutesDuration()
        {
            var result = GetListOfAppointments_ThirtyMinutesDuration();
            result.Remove(new TimeSpan(11, 30, 0));
            result.Remove(new TimeSpan(17, 30, 0));
            return result;
        }

        [TestMethod]
        public void GetAvailableTimes_SixtyMinutesAppointmentDuration_IdenticalList()
        {
            Appointment appointment = GetSixtyMinutesAppointment();
            List<TimeSpan> beingTested = Model.GetAvailableTimes(appointment);
            List<TimeSpan> correctList = GetListOfAppointments_SixtyMinutesDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        private Appointment GetSixtyMinutesAppointment()
        {
            return new Appointment() { Time = new TimeSpan(9, 0, 0), DurationInMinutes = 60 };
        }
        private List<TimeSpan> GetListOfAppointments_SixtyMinutesDuration()
        {
            var result = GetListOfAppointments_FourtyFiveMinutesDuration();
            result.Remove(new TimeSpan(11, 15, 0));
            result.Remove(new TimeSpan(17, 15, 0));
            return result;
        }

        [TestMethod]
        public void GetAvailableTimes_NullAppointment_DomainException()
        {
            try
            {
                Model.GetAvailableTimes(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta não fornecida!", e.Message);
            }
        }
    }
}

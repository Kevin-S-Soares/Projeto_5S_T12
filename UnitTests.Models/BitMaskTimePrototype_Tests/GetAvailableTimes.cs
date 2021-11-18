using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using WebOdontologista.Models;
using WebOdontologista.Models.CollectionTimePrototype;

namespace UnitTests.Models.BitMaskTimePrototype_Tests
{
    [TestClass]
    public class GetAvailableTimes
    {
        public BitMaskTimePrototype Model { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Model = new BitMaskTimePrototype(null);
            Model.SetSchedule(null);
        }

        [TestMethod]
        public void FifteenMinutesAppointmentDuration_IdenticalList()
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
        public void ThirtyMinutesAppointmentDuration_IdenticalList()
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
        public void FourtyFiveMinutesAppointmentDuration_IdenticalList()
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
        public void SixtyMinutesAppointmentDuration_IdenticalList()
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
        public List<TimeSpan> GetListOfAppointments_SixtyMinutesDuration()
        {
            var result = GetListOfAppointments_FourtyFiveMinutesDuration();
            result.Remove(new TimeSpan(11, 15, 0));
            result.Remove(new TimeSpan(17, 15, 0));
            return result;
        }

        [TestMethod]
        public void NullAppointment_DomainException()
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using WebOdontologista.Models;
using WebOdontologista.Models.CollectionTimePrototype;
using System.Linq;
using System.Diagnostics;

namespace UnitTests.Models
{
    [TestClass]
    public class BitMaskTimePrototype_Tests
    {
        public BitMaskTimePrototype Model { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            Model = new BitMaskTimePrototype(null);
            Model.SetSchedule(null);
        }
        [TestMethod]
        public void MakeAppointment_AddingAppointments_SucceedToAdd()
        {
            try
            {
                foreach (Appointment obj in AppointmentsToSucceed())
                {
                    Model.MakeAppointment(obj);
                }
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void MakeAppointment_AddingAppointmentsColliding_DomainException()
        {
            try
            {
                foreach (Appointment obj in AppointmentsToFail())
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
        [TestMethod]
        public void MakeAppointment_AddingAppointmentsBeforeExpectedTime_DomainException()
        {
            try
            {
                Appointment toFail = new Appointment() { Time = new TimeSpan(8, 45, 0), DurationInMinutes = 30 };
                Model.MakeAppointment(toFail);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta fora dos limites!", e.Message);
            }
        }
        [TestMethod]
        public void MakeAppointment_AddingAppointmentsAfterExpectedTime_DomainException()
        {
            try
            {
                Appointment toFail = new Appointment() { Time = new TimeSpan(17, 15, 0), DurationInMinutes = 60 };
                Model.MakeAppointment(toFail);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Consulta fora dos limites!", e.Message);
            }
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
        public void CancelAppointment_CancellingAppointments_SucceedToCancel()
        {
            try
            {
                var list = AppointmentsToSucceed();
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
        [TestMethod]
        public void CancelAppointment_CancellingNonExistentAppointments_DomainException()
        {
            try
            {
                var toFail = new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(16, 0, 0),
                };
                Model.CancelAppointment(toFail);
            }
            catch (Exception ae)
            {
                Assert.AreEqual("Cancelamento de consulta proíbido!", ae.Message);
            }
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
            var beingTested = Model.GetAvailableTimes(new Appointment() { DurationInMinutes = 15 });
            var correctList = FifteenMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        [TestMethod]
        public void GetAvailableTimes_ThirtyMinutesAppointmentDuration_IdenticalList()
        {
            var beingTested = Model.GetAvailableTimes(new Appointment() { DurationInMinutes = 30 });
            var correctList = ThirtyMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        [TestMethod]
        public void GetAvailableTimes_FourtyFiveMinutesAppointmentDuration_IdenticalList()
        {
            var beingTested = Model.GetAvailableTimes(new Appointment() { DurationInMinutes = 45 });
            var correctList = FourtyFiveMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        [TestMethod]
        public void GetAvailableTimes_SixtyMinutesAppointmentDuration_IdenticalList()
        {
            var beingTested = Model.GetAvailableTimes(new Appointment() { DurationInMinutes = 60 });
            var correctList = SixtyMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
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
        private List<Appointment> AppointmentsToFail()
        {
            var list = AppointmentsToSucceed();
            list.Add(new Appointment()
            {
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 15, 0)
            });
            return list;
        }
        private List<Appointment> AppointmentsToSucceed()
        {
            return new List<Appointment>()
            {
                new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(9, 0, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(9, 15, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(9, 30, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(9, 45, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 60,
                    Time = new TimeSpan(10, 0, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 30,
                    Time = new TimeSpan(11, 0, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 30,
                    Time = new TimeSpan(11, 30, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(13, 0, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 45,
                    Time = new TimeSpan(13, 15, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 60,
                    Time = new TimeSpan(14, 0, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 45,
                    Time = new TimeSpan(15, 0, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(15, 45, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(16, 0, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(16, 15, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 30,
                    Time = new TimeSpan(16, 30, 0)
                },
                new Appointment()
                {
                    DurationInMinutes = 60,
                    Time = new TimeSpan(17, 0, 0)
                },
            };
        }
        private void SetAppointments(List<Appointment> list)
        {
            foreach (Appointment obj in list)
            {
                Model.MakeAppointment(obj);
            }
        }
        private List<TimeSpan> FifteenMinutesAppointmentDuration()
        {
            var result = new List<TimeSpan>();
            for(int i = 0; i < 12; i++)
            {
                result.Add(new TimeSpan(9, i * 15, 0));
            }
            for (int i = 16; i < 36; i++)
            {
                result.Add(new TimeSpan(9, i * 15, 0));
            }
            return result;
        }
        private List<TimeSpan> ThirtyMinutesAppointmentDuration()
        {
            var result = FifteenMinutesAppointmentDuration();
            result.Remove(new TimeSpan(11, 45, 0));
            result.Remove(new TimeSpan(17, 45, 0));
            return result;
        }
        private List<TimeSpan> FourtyFiveMinutesAppointmentDuration()
        {
            var result = ThirtyMinutesAppointmentDuration();
            result.Remove(new TimeSpan(11, 30, 0));
            result.Remove(new TimeSpan(17, 30, 0));
            return result;
        }
        private List<TimeSpan> SixtyMinutesAppointmentDuration()
        {
            var result = FourtyFiveMinutesAppointmentDuration();
            result.Remove(new TimeSpan(11, 15, 0));
            result.Remove(new TimeSpan(17, 15, 0));
            return result;
        }
    }
}

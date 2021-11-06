using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using WebOdontologista.Models;
using WebOdontologista.Models.CollectionTimePrototype;
using WebOdontologista.Models.Exceptions;
using WebOdontologista.Models.Interfaces;
using System.Linq;


namespace UnitTests.Models
{
    [TestClass]
    public class BitMaskTimePrototype_Tests
    {
        public BitMaskTimePrototype model { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            model = new BitMaskTimePrototype(null);
            model.SetSchedule(null);
        }
        [TestMethod]
        public void MakeAppointment_AddingAppointments_SucceedToAdd()
        {
            try
            {
                foreach (Appointment obj in AppointmentsToSucceed())
                {
                    model.MakeAppointment(obj);
                }
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void MakeAppointment_AddingAppointments_ColisionAdding()
        {
            try
            {
                foreach (Appointment obj in AppointmentsToFail())
                {
                    model.MakeAppointment(obj);
                }
                Assert.Fail();
            }
            catch (DomainException e)
            {
                Assert.AreEqual("Não foi possivel adicionar a consulta!", e.Message);
            }
            catch (Exception)
            {
                Assert.Fail();
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
                    model.CancelAppointment(obj);
                }
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void CancelAppointment_CancellingAppointments_FailToCancel()
        {
            try
            {
                var toFail = new Appointment()
                {
                    DurationInMinutes = 15,
                    Time = new TimeSpan(16, 0, 0),
                };
                model.CancelAppointment(toFail);
            }
            catch (DomainException ae)
            {
                Assert.AreEqual("Cancelamento de consulta proíbido!", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("exception :{0}\nmessage:{1}", e.GetType(), e.Message));
            }
        }
        [TestMethod]
        public void GetAvailableTimes_FifteenMinutesAppointmentDuration_IdenticalList()
        {
            var beingTested = model.GetAvailableTimes(new Appointment() { DurationInMinutes = 15 });
            var correctList = FifteenMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        [TestMethod]
        public void GetAvailableTimes_ThirtyMinutesAppointmentDuration_IdenticalList()
        {
            var beingTested = model.GetAvailableTimes(new Appointment() { DurationInMinutes = 30 });
            var correctList = ThirtyMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        [TestMethod]
        public void GetAvailableTimes_FourtyFiveMinutesAppointmentDuration_IdenticalList()
        {
            var beingTested = model.GetAvailableTimes(new Appointment() { DurationInMinutes = 45 });
            var correctList = FourtyFiveMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }
        [TestMethod]
        public void GetAvailableTimes_SixtyMinutesAppointmentDuration_IdenticalList()
        {
            var beingTested = model.GetAvailableTimes(new Appointment() { DurationInMinutes = 60 });
            var correctList = SixtyMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
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
                model.MakeAppointment(obj);
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

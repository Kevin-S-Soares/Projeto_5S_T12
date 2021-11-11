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

        [TestMethod]
        public void MakeAppointment_AddingAppointmentBeforeExpectedTime_DomainException()
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
        public void MakeAppointment_AddingAppointmentAfterExpectedTime_DomainException()
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

        [TestMethod]
        public void CancelAppointment_CancellingNonExistentAppointment_DomainException()
        {
            try
            {
                Appointment toFail = new Appointment()
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
            Appointment appointment = new Appointment()
            {
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 15,
            };
            List<TimeSpan> beingTested = Model.GetAvailableTimes(appointment);
            List<TimeSpan> correctList = FifteenMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }

        [TestMethod]
        public void GetAvailableTimes_ThirtyMinutesAppointmentDuration_IdenticalList()
        {
            Appointment appointment = new Appointment()
            {
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 30,
            };
            List<TimeSpan> beingTested = Model.GetAvailableTimes(appointment);
            List<TimeSpan> correctList = ThirtyMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }

        [TestMethod]
        public void GetAvailableTimes_FourtyFiveMinutesAppointmentDuration_IdenticalList()
        {
            Appointment appointment = new Appointment()
            {
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 45,
            };
            List<TimeSpan> beingTested = Model.GetAvailableTimes(appointment);
            List<TimeSpan> correctList = FourtyFiveMinutesAppointmentDuration();
            CollectionAssert.AreEqual(beingTested, correctList);
        }

        [TestMethod]
        public void GetAvailableTimes_SixtyMinutesAppointmentDuration_IdenticalList()
        {
            Appointment appointment = new Appointment()
            {
                Time = new TimeSpan(9, 0, 0),
                DurationInMinutes = 60,
            };
            List<TimeSpan> beingTested = Model.GetAvailableTimes(appointment);
            List<TimeSpan> correctList = SixtyMinutesAppointmentDuration();
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
        private async Task<List<Appointment>> AppointmentsToSucceed()
        {
            return await _appointmentService.FindAllAsync(obj => obj.Date == _timeZoneService.GetTodayOnly());
        }
        public List<TimeSpan> FifteenMinutesAppointmentDuration()
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
        private void SetAppointments(List<Appointment> list)
        {
            foreach (Appointment obj in list)
            {
                Model.MakeAppointment(obj);
            }
        }
    }
}

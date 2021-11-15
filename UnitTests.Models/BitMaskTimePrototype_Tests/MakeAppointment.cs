using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Models;
using WebOdontologista.Models.CollectionTimePrototype;

namespace UnitTests.Models.BitMaskTimePrototype_Tests
{
    [TestClass]
    public class MakeAppointment
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
        public async Task AddingMultipleAppointments_SucceedToAdd()
        {
            foreach (Appointment obj in await AppointmentsToSucceed())
            {
                Model.MakeAppointment(obj);
            }
        }
        private async Task<List<Appointment>> AppointmentsToSucceed()
        {
            return await _appointmentService.FindAllAsync(obj => obj.Date == _timeZoneService.GetTodayOnly());
        }

        [TestMethod]
        public async Task AddingAppointmentsColliding_DomainException()
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
        public void AddingAppointmentBeforeExpectedTime_DomainException()
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
        public void AddingAppointmentAfterExpectedTime_DomainException()
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


    }
}

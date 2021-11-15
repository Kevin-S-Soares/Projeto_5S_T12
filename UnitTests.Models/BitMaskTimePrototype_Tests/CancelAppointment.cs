using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.CollectionTimePrototype;

namespace UnitTests.Models.BitMaskTimePrototype_Tests
{
    [TestClass]
    public class CancelAppointment
    {
        public BitMaskTimePrototype Model { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Model = new BitMaskTimePrototype(null);
            Model.SetSchedule(null);
        }
        [TestMethod]
        public async Task CancellingMultipleAppointments_SucceedToCancel()
        {
            try
            {
                List<Appointment> list = await new MakeAppointment().AppointmentsToSucceed();
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
        public void CancellingNonExistentAppointment_DomainException()
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
        public void NullAppointment_DomainException()
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

    }
}

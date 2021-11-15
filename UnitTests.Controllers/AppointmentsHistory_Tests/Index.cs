using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;

namespace UnitTests.Controllers.AppointmentsHistory_Tests
{
    [TestClass]
    public class Index
    {
        public AppointmentsHistoryController Controller_Test { get; set; }

        private readonly AppointmentServiceDependecy _appointmentService = new AppointmentServiceDependecy();
        private readonly TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new AppointmentsHistoryController(_appointmentService, _timeZoneService);
        }

        [TestMethod]
        public void IsView()
        {
            IActionResult result = Controller_Test.Index();
            Assert.IsTrue(result is ViewResult);
        }
    }
}

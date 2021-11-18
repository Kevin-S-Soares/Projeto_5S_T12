using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UnitTests.Controllers.AppointmentsController_Tests.HttpSetupClasses;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Controllers.AppointmentsController_Tests
{
    [TestClass]
    public class GetTimes
    {
        public AppointmentsController Controller_Test { get; set; }

        private readonly IAppointmentService _appointmentService = new AppointmentServiceDependecy();
        private readonly IDentistService _dentistService = new DentistServiceDependecy();
        private readonly TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();


        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new AppointmentsController(_appointmentService, _dentistService, _timeZoneService);
            AddContext(false);
        }

        private void AddContext(bool toAddAppointment)
        {
            Controller_Test.TempData = new TempDataDictionary(new UnitTest_HttpContext(null),
                new UnitTest_TempDataProvider(toAddAppointment));
        }

        [TestMethod]
        public async Task NullValues_CorrectResult()
        {
            string result = await Controller_Test.GetTimes(null, null, null);
            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public async Task OnlyDentistIdValue_CorrectResult()
        {
            string result = await Controller_Test.GetTimes(1, null, null);
            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public async Task OnlyDateTimeValue_CorrectResult()
        {
            string result = await Controller_Test.GetTimes(null, _timeZoneService.GetTodayOnly(), null);
            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public async Task OnlyDurationInMinutesValue_CorrectResult()
        {
            string result = await Controller_Test.GetTimes(null, null, 60);
            Assert.AreEqual("[]", result);
        }


        [TestMethod]
        public async Task OnlyDentistIdNull_CorrectResult()
        {
            string result = await Controller_Test.GetTimes(null, _timeZoneService.GetTodayOnly(), 60);
            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public async Task OnlyDateTimeNull_CorrectResult()
        {
            string result = await Controller_Test.GetTimes(1, null, 60);
            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public async Task OnlyDurationInMinutesNull_CorrectResult()
        {
            string result = await Controller_Test.GetTimes(1, _timeZoneService.GetTodayOnly(), null);
            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public async Task CorrectValues_CorrectResult()
        {
            string result = await Controller_Test.GetTimes(1, _timeZoneService.GetTodayOnly(), 60);
            Assert.AreEqual("[]", result);
        }
    }
}

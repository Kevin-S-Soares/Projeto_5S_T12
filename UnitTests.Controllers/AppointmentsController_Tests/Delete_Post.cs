using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Controllers.AppointmentsController_Tests
{
    [TestClass]
    public class Delete_Post
    {
        public AppointmentsController Controller_Test { get; set; }

        private readonly IAppointmentService _appointmentService = new AppointmentServiceDependecy();
        private readonly IDentistService _dentistService = new DentistServiceDependecy();
        private readonly TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();


        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new AppointmentsController(_appointmentService, _dentistService, _timeZoneService);
        }

        [TestMethod]
        public async Task Succeed_IsRedirecting()
        {
            int? id = 1;
            IActionResult result = await Controller_Test.DeleteById(id);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task Succeed_CorrectRedirect()
        {
            int? id = 1;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(id);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task Succeed_RemovingFromAppointmentService()
        {
            int? id = 1;
            await Controller_Test.DeleteById(id);
            Assert.IsTrue(await _appointmentService.FindByIdAsync(1) is null);
        }

        [TestMethod]
        public async Task NullId_IsRedirecting()
        {
            int? id = null;
            IActionResult result = await Controller_Test.DeleteById(id);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NullId_CorrectRedirect()
        {
            int? id = null;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(id);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NullId_CorrectModel()
        {
            int? id = null;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(id);
            Assert.AreEqual("Id não provido!", result.RouteValues["message"]);
        }

        [TestMethod]
        public async Task NonExistingAppointment_IsRedirecting()
        {
            int? id = -1;
            IActionResult result = await Controller_Test.DeleteById(id);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NonExistingAppointment_CorrectRedirect()
        {
            int? id = -1;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(id);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NonExistingAppointment_CorrectModel()
        {
            int? id = -1;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(id);
            Assert.AreEqual("Id não encontrado!", result.RouteValues["message"]);
        }
    }
}

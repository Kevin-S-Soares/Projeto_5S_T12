using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Controllers.AppointmentsController_Tests.HttpSetupClasses;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Models.ViewModels;

namespace UnitTests.Controllers.AppointmentsController_Tests
{
    [TestClass]
    public class Edit_Post
    {
        public AppointmentsController Controller_Test { get; set; }

        private readonly IAppointmentService _appointmentService = new AppointmentServiceDependecy();
        private readonly IDentistService _dentistService = new DentistServiceDependecy();
        private readonly TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();


        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new AppointmentsController(_appointmentService, _dentistService, _timeZoneService);
            AddContext(true);
        }

        private void AddContext(bool toAddAppointment)
        {
            Controller_Test.TempData = new TempDataDictionary(new UnitTest_HttpContext(null),
                new UnitTest_TempDataProvider(toAddAppointment));
        }

        [TestMethod]
        public async Task Succeed_IsRedirectToAction()
        {
            Appointment appointment = await _appointmentService.FindByIdAsync(1);
            IActionResult result = await Controller_Test.Edit(appointment);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task Succeed_CorrectRedirect()
        {
            Appointment appointment = await _appointmentService.FindByIdAsync(1);
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(appointment);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task Succeed_IsUpdatingInAppointmentService()
        {
            Appointment appointment = await GetNewAppointment();
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(appointment);
            Appointment beingTested = await _appointmentService.FindByIdAsync(1);
            Assert.AreEqual("Test123", beingTested.Patient);
        }

        [TestMethod]
        public async Task IncorrectModel_IsView()
        {
            Controller_Test.ModelState.AddModelError("error", "error");
            Appointment appointment = null;
            IActionResult result = await Controller_Test.Edit(appointment);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task IncorrectModel_CorrectModel()
        {
            Controller_Test.ModelState.AddModelError("error", "error");
            ViewResult result = (ViewResult)await Controller_Test.Edit(await GetNewAppointment());
            AppointmentFormViewModel viewModel = (AppointmentFormViewModel)result.Model;
            Assert.AreEqual(await GetNewAppointment(), viewModel.Appointment);
        }

        [TestMethod]
        public async Task IncorrectModel_CorrectDentistList()
        {
            Controller_Test.ModelState.AddModelError("error", "error");
            ViewResult result = (ViewResult)await Controller_Test.Create(await GetNewAppointment());
            AppointmentFormViewModel viewModel = (AppointmentFormViewModel)result.Model;
            CollectionAssert.AreEqual(await _dentistService.FindAllAsync(), (List<Dentist>)viewModel.Dentists);
        }

        [TestMethod]
        public async Task NonExistingAppointment_IsRedirecting()
        {
            Appointment appointment = new Appointment() { Id = -1 };
            IActionResult result = await Controller_Test.Edit(appointment);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NonExistingAppointment_CorrectRedirect()
        {
            Appointment appointment = new Appointment() { Id = -1 };
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(appointment);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NonExistingAppointment_CorrectModel()
        {
            Appointment appointment = new Appointment() { Id = -1 };
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(appointment);
            Assert.AreEqual("Id não encontrado!", result.RouteValues["message"]);
        }

        private async Task<Appointment> GetNewAppointment()
        {
            Appointment result = await _appointmentService.FindByIdAsync(1);
            result.Patient = "Test123";
            return result;
        }
    }
}

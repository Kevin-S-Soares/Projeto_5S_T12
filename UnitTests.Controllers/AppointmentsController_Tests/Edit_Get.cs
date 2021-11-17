using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
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
    public class Edit_Get
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
        public async Task Succeed_IsView()
        {
            IActionResult result = await Controller_Test.Edit(1);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task Succeed_CorrectModel()
        {
            ViewResult result = (ViewResult) await Controller_Test.Edit(1);
            AppointmentFormViewModel viewModel = (AppointmentFormViewModel) result.Model;
            Assert.AreEqual(await _appointmentService.FindByIdAsync(1), viewModel.Appointment);
        }

        [TestMethod]
        public async Task Succeed_IdenticalDentistList()
        {
            ViewResult result = (ViewResult)await Controller_Test.Edit(1);
            AppointmentFormViewModel viewModel = (AppointmentFormViewModel)result.Model;

            List<Dentist> correctList = await _dentistService.FindAllAsync();
            List<Dentist> beingTested = (List<Dentist>)viewModel.Dentists;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task Succeed_TempDataContainsKey()
        {
            await Controller_Test.Edit(1);

            Assert.IsTrue(Controller_Test.TempData.ContainsKey("appointment"));
        }

        [TestMethod]
        public async Task Succeed_TempDataValueIsCorrect()
        {
            await Controller_Test.Edit(1);
            Appointment appointment = await _appointmentService.FindByIdAsync(1);
            Assert.AreEqual(appointment.Serialize(), Controller_Test.TempData["appointment"]);
        }

        [TestMethod]
        public async Task NullId_IsRedirecting()
        {
            int? id = null;
            IActionResult result = await Controller_Test.Edit(id);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NullId_CorrectRedirect()
        {
            int? id = null;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(id);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NullId_CorrectModel()
        {
            int? id = null;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(id);
            Assert.AreEqual("Id não provido!", result.RouteValues["message"]);
        }

        [TestMethod]
        public async Task NonExistingAppointment_IsRedirecting()
        {
            int? id = -1;
            IActionResult result = await Controller_Test.Edit(id);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NonExistingAppointment_CorrectRedirect()
        {
            int? id = -1;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(id);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NonExistingAppointment_CorrectModel()
        {
            int? id = -1;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(id);
            Assert.AreEqual("Id não encontrado!", result.RouteValues["message"]);
        }


    }
}

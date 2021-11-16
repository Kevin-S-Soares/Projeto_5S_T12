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
    public class Create_Get
    {
        public AppointmentsController Controller_Test { get; set; }

        private readonly IAppointmentService _appointmentService = new AppointmentServiceDependecy();
        private readonly DentistServiceDependecy _dentistService = new DentistServiceDependecy();
        private readonly TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();


        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new AppointmentsController(_appointmentService, _dentistService, _timeZoneService);
            
        }

        [TestMethod]
        public async Task Successful_IsView()
        {
            AddContext(true);
            IActionResult result = await Controller_Test.Create();
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task Successful_ReturningCorrectModel()
        {
            AddContext(true);
            ViewResult result = (ViewResult) await Controller_Test.Create();
            Assert.IsTrue(result.Model is AppointmentFormViewModel);
        }

        [TestMethod]
        public async Task Successful_IdenticalList()
        {
            AddContext(true);
            ViewResult result = (ViewResult)await Controller_Test.Create();
            AppointmentFormViewModel viewModel = (AppointmentFormViewModel)result.Model;
            
            List<Dentist> correctList = await _dentistService.FindAllAsync();
            List<Dentist> beingTested = (List<Dentist>)viewModel.Dentists;


            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task WhenNoAvailableDentists_IsRedirect()
        {
            AddContext(true);
            _dentistService.DeleteAll();
            IActionResult result = await Controller_Test.Create();
            Assert.IsTrue(result is RedirectResult);
        }

        [TestMethod]
        public async Task WhenNoAvailableDentists_CorrectRedirect()
        {
            AddContext(true);
            _dentistService.DeleteAll();
            RedirectResult result = (RedirectResult) await Controller_Test.Create();
            Assert.AreEqual("/Dentists/Create?returnAppointment=1", result.Url);
        }

        [TestMethod]
        public async Task IsRemovingTempData()
        {
            AddContext(true);
            await Controller_Test.Create();
            Assert.IsFalse(Controller_Test.TempData.ContainsKey("appointment"));
        }


        private void AddContext(bool toAddAppointment)
        {
            Controller_Test.TempData = new TempDataDictionary(new UnitTest_HttpContext(null), 
                new UnitTest_TempDataProvider(toAddAppointment));
        }
    }
}

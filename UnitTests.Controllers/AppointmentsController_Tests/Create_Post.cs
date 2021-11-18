using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Models.ViewModels;

namespace UnitTests.Controllers.AppointmentsController_Tests
{
    [TestClass]
    public class Create_Post
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
        public async Task Successful_IsRedirectToActionResult()
        {
            IActionResult result = await Controller_Test.Create(GetNewAppointment());
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task Successful_CorrectRedirect()
        {
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Create(GetNewAppointment());
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task Succesful_IsAddingToAppointmentService()
        {
            await Controller_Test.Create(GetNewAppointment());
            Assert.AreEqual(GetNewAppointment(), await _appointmentService.FindByIdAsync(2047));
        }

        [TestMethod]
        public async Task IncorrectModel_IsView()
        {
            Controller_Test.ModelState.AddModelError("error", "error");
            IActionResult result = await Controller_Test.Create(null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task IncorrectModel_CorrectModel()
        {
            Controller_Test.ModelState.AddModelError("error", "error");
            ViewResult result = (ViewResult)await Controller_Test.Create(GetNewAppointment());
            AppointmentFormViewModel viewModel = (AppointmentFormViewModel)result.Model;
            Assert.AreEqual(GetNewAppointment(), viewModel.Appointment);
        }

        [TestMethod]
        public async Task IncorrectModel_CorrectDentistList()
        {
            Controller_Test.ModelState.AddModelError("error", "error");
            ViewResult result = (ViewResult)await Controller_Test.Create(GetNewAppointment());
            AppointmentFormViewModel viewModel = (AppointmentFormViewModel)result.Model;
            CollectionAssert.AreEqual(await _dentistService.FindAllAsync(), (List<Dentist>)viewModel.Dentists);
        }



        private Appointment GetNewAppointment()
        {
            return new Appointment()
            {
                Id = 2047,
                DurationInMinutes = 60,
                DentistId = 1,
                Time = new TimeSpan(9, 0, 0),
                Date = _timeZoneService.GetTodayOnly().AddDays(14)
            };
        }

    }
}

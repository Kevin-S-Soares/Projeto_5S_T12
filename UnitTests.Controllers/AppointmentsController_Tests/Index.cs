using Microsoft.AspNetCore.Mvc;
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
    public class Index
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
        public async Task NullShow_IsView()
        {
            AddContext(null);
            IActionResult result = await Controller_Test.Index(null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task NullShow_IdenticalList()
        {
            AddContext(null);
            ViewResult result = (ViewResult)await Controller_Test.Index(null);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => true);
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }


        [TestMethod]
        public async Task LessThanExpectedShow_IsView()
        {
            AddContext(null);
            IActionResult result = await Controller_Test.Index(-1);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task LessThanExpectedShow_IdenticalList()
        {
            AddContext(null);
            ViewResult result = (ViewResult)await Controller_Test.Index(-1);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => true);
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task LessThanExpectedCookie_IdenticalList()
        {
            AddContext(-1);
            ViewResult result = (ViewResult)await Controller_Test.Index(null);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => true);
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task MoreThanExpectedShow_IsView()
        {
            AddContext(null);
            IActionResult result = await Controller_Test.Index(4);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task MoreThanExpectedShow_IdenticalList()
        {
            AddContext(null);
            ViewResult result = (ViewResult)await Controller_Test.Index(4);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => true);
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task MoreThanExpectedCookie_IdenticalList()
        {
            AddContext(4);
            ViewResult result = (ViewResult)await Controller_Test.Index(null);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => true);
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }


        [TestMethod]
        public async Task ShowToday_IsView()
        {
            AddContext(null);
            IActionResult result = await Controller_Test.Index(0);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task ShowToday_IdenticalList()
        {
            AddContext(null);
            ViewResult result = (ViewResult)await Controller_Test.Index(0);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj =>
            obj.DateAndTime() >= _timeZoneService.GetDate()
            && obj.Date == _timeZoneService.GetTodayOnly());
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task ShowTodayCookie_IdenticalList()
        {
            AddContext(0);
            ViewResult result = (ViewResult)await Controller_Test.Index(null);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj =>
            obj.DateAndTime() >= _timeZoneService.GetDate()
            && obj.Date == _timeZoneService.GetTodayOnly());
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }


        [TestMethod]
        public async Task ShowThisWeek_IsView()
        {
            AddContext(null);
            IActionResult result = await Controller_Test.Index(1);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task ShowThisWeek_IdenticalList()
        {
            AddContext(null);
            ViewResult result = (ViewResult)await Controller_Test.Index(1);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj =>
            obj.DateAndTime() >= _timeZoneService.GetDate()
            && obj.Date <= _timeZoneService.GetTodayOnly().AddDays(7));
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task ShowThisWeekCookie_IdenticalList()
        {
            AddContext(1);
            ViewResult result = (ViewResult)await Controller_Test.Index(null);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj =>
            obj.DateAndTime() >= _timeZoneService.GetDate()
            && obj.Date <= _timeZoneService.GetTodayOnly().AddDays(7));
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }


        [TestMethod]
        public async Task ShowThisMonth_IsView()
        {
            AddContext(null);
            IActionResult result = await Controller_Test.Index(2);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task ShowThisMonth_IdenticalList()
        {
            AddContext(null);
            ViewResult result = (ViewResult)await Controller_Test.Index(2);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj =>
            obj.DateAndTime() >= _timeZoneService.GetDate()
            && obj.Date <= _timeZoneService.GetTodayOnly().AddDays(30));
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task ShowThisMonthCookie_IdenticalList()
        {
            AddContext(2);
            ViewResult result = (ViewResult)await Controller_Test.Index(null);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj =>
            obj.DateAndTime() >= _timeZoneService.GetDate()
            && obj.Date <= _timeZoneService.GetTodayOnly().AddDays(30));
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }


        [TestMethod]
        public async Task ShowAll_IsView()
        {
            AddContext(null);
            IActionResult result = await Controller_Test.Index(3);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task ShowAll_IdenticalList()
        {
            AddContext(null);
            ViewResult result = (ViewResult)await Controller_Test.Index(3);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => true);
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task ShowAllCookie_IdenticalList()
        {
            AddContext(3);
            ViewResult result = (ViewResult)await Controller_Test.Index(null);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => true);
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        [TestMethod]
        public async Task PastTime_IdenticalList()
        {
            _timeZoneService.ChangeToNineteen();

            AddContext(null);
            ViewResult result = (ViewResult)await Controller_Test.Index(0);
            IndexAppointmentFormViewModel viewModel = (IndexAppointmentFormViewModel)result.Model;

            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj =>
            obj.DateAndTime() >= _timeZoneService.GetDate()
            && obj.Date == _timeZoneService.GetTodayOnly());
            List<Appointment> beingTested = (List<Appointment>)viewModel.Appointments;

            CollectionAssert.AreEqual(correctList, beingTested);
        }

        private void AddContext(int? requestValue)
        {
            Controller_Test.ControllerContext.HttpContext = new UnitTest_HttpContext(requestValue);
        }
    }
}

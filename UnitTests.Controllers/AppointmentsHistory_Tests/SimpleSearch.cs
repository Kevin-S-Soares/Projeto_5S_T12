using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;

namespace UnitTests.Controllers.AppointmentsHistory_Tests
{
    [TestClass]
    public class SimpleSearch
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
        public async Task NullDates_IsView()
        {
            IActionResult result = await Controller_Test.SimpleSearch(null, null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task NullDates_CorrectModel()
        {
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(null, null);
            Assert.IsTrue(result.Model is List<Appointment>);
        }

        [TestMethod]
        public async Task NullDates_CorrectViewData()
        {
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(null, null);
            Assert.AreEqual(GetMinDateString(), result.ViewData["minDate"]);
            Assert.AreEqual(GetMaxDateString(), result.ViewData["maxDate"]);
        }

        [TestMethod]
        public async Task NullDates_IdenticalList()
        {
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(null, null);
            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => true);
            CollectionAssert.AreEqual(correctList, (List<Appointment>)result.Model);
        }

        [TestMethod]
        public async Task MinDateAndNull_IsView()
        {
            IActionResult result = await Controller_Test.SimpleSearch(_timeZoneService.GetTomorrowOnly(), null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task MinDateAndNull_CorrectModel()
        {
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(_timeZoneService.GetTomorrowOnly(), null);
            Assert.IsTrue(result.Model is List<Appointment>);
        }

        [TestMethod]
        public async Task MinDateAndNull_CorrectViewData()
        {
            DateTime minDate = _timeZoneService.GetTomorrowOnly();
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(minDate, null);
            Assert.AreEqual(minDate.ToString("yyyy-MM-dd"), result.ViewData["minDate"]);
            Assert.AreEqual(GetMaxDateString(), result.ViewData["maxDate"]);
        }

        [TestMethod]
        public async Task MinDateAndNull_IdenticalList()
        {
            DateTime minDate = _timeZoneService.GetTomorrowOnly();
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(minDate, null);
            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => obj.Date >= minDate);
            CollectionAssert.AreEqual(correctList, (List<Appointment>)result.Model);
        }

        [TestMethod]
        public async Task NullAndMaxDate_IsView()
        {
            IActionResult result = await Controller_Test.SimpleSearch(null, _timeZoneService.GetTodayOnly());
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task NullAndMaxDate_CorrectModel()
        {
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(null, _timeZoneService.GetTodayOnly());
            Assert.IsTrue(result.Model is List<Appointment>);
        }

        [TestMethod]
        public async Task NullAndMaxDate_CorrectViewData()
        {
            DateTime maxDate = _timeZoneService.GetTodayOnly();
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(null, maxDate);
            Assert.AreEqual(GetMinDateString(), result.ViewData["minDate"]);
            Assert.AreEqual(maxDate.ToString("yyyy-MM-dd"), result.ViewData["maxDate"]);
        }

        [TestMethod]
        public async Task NullAndMaxDate_IdenticalList()
        {
            DateTime maxDate = _timeZoneService.GetTodayOnly();
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(null, maxDate);
            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => obj.Date <= maxDate);
            CollectionAssert.AreEqual(correctList, (List<Appointment>)result.Model);
        }

        [TestMethod]
        public async Task MinDateAndMaxDate_IsView()
        {

            IActionResult result = await Controller_Test.SimpleSearch(_timeZoneService.GetTodayOnly(), _timeZoneService.GetTomorrowOnly());
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task MinDateAndMaxDate_CorrectModel()
        {
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(_timeZoneService.GetTodayOnly(), _timeZoneService.GetTomorrowOnly());
            Assert.IsTrue(result.Model is List<Appointment>);
        }

        [TestMethod]
        public async Task MinDateAndMaxDate_CorrectViewData()
        {
            DateTime minDate = _timeZoneService.GetTodayOnly();
            DateTime maxDate = _timeZoneService.GetTomorrowOnly();
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(minDate, maxDate);
            Assert.AreEqual(minDate.ToString("yyyy-MM-dd"), result.ViewData["minDate"]);
            Assert.AreEqual(maxDate.ToString("yyyy-MM-dd"), result.ViewData["maxDate"]);
        }

        [TestMethod]
        public async Task MinDateAndMaxDate_IdenticalList()
        {
            DateTime minDate = _timeZoneService.GetTodayOnly();
            DateTime maxDate = _timeZoneService.GetTomorrowOnly();
            ViewResult result = (ViewResult)await Controller_Test.SimpleSearch(minDate, maxDate);
            List<Appointment> correctList = await _appointmentService.FindAllAsync(obj => obj.Date >= minDate && obj.Date <= maxDate);
            CollectionAssert.AreEqual(correctList, (List<Appointment>)result.Model);
        }

        private string GetMinDateString()
        {
            return new DateTime(
                _timeZoneService.GetTodayOnly().Year,
                1, 1).ToString("yyyy-MM-dd");
        }
        private string GetMaxDateString()
        {
            return new DateTime(
                _timeZoneService.GetTodayOnly().Year,
                12, 31).ToString("yyyy-MM-dd");
        }
    }
}

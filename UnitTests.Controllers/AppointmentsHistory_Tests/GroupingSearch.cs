using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;

namespace UnitTests.Controllers.AppointmentsHistory_Tests
{
    [TestClass]
    public class GroupingSearch
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
            IActionResult result = await Controller_Test.GroupingSearch(null, null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task NullDates_CorrectModel()
        {
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(null, null);
            Assert.IsTrue(result.Model is List<IGrouping<Dentist, Appointment>>);
        }

        [TestMethod]
        public async Task NullDates_CorrectViewData()
        {
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(null, null);
            Assert.AreEqual(GetMinDateString(), result.ViewData["minDate"]);
            Assert.AreEqual(GetMaxDateString(), result.ViewData["maxDate"]);
        }

        [TestMethod]
        public async Task NullDates_IdenticalList()
        {
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(null, null);
            List<IGrouping<Dentist, Appointment>> correctList = await _appointmentService.FindByDateGroupingAsync(GetMinDate(), GetMaxDate());
            List<IGrouping<Dentist, Appointment>> beingTested = (List<IGrouping<Dentist, Appointment>>)result.Model;


            List<Appointment> correctListOfAppointments = correctList.SelectMany(obj => obj).ToList();
            List<Appointment> beingTestedListOfAppointments = beingTested.SelectMany(obj => obj).ToList();

            CollectionAssert.AreEqual(correctListOfAppointments, beingTestedListOfAppointments);
        }

        [TestMethod]
        public async Task MinDateAndNull_IsView()
        {
            IActionResult result = await Controller_Test.GroupingSearch(_timeZoneService.GetTomorrowOnly(), null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task MinDateAndNull_CorrectModel()
        {
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(_timeZoneService.GetTomorrowOnly(), null);
            Assert.IsTrue(result.Model is List<IGrouping<Dentist, Appointment>>);
        }

        [TestMethod]
        public async Task MinDateAndNull_CorrectViewData()
        {
            DateTime minDate = _timeZoneService.GetTomorrowOnly();
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(minDate, null);
            Assert.AreEqual(minDate.ToString("yyyy-MM-dd"), result.ViewData["minDate"]);
            Assert.AreEqual(GetMaxDateString(), result.ViewData["maxDate"]);
        }

        [TestMethod]
        public async Task MinDateAndNull_IdenticalList()
        {
            DateTime minDate = _timeZoneService.GetTomorrowOnly();
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(minDate, null);
            List<IGrouping<Dentist, Appointment>> correctList = await _appointmentService.FindByDateGroupingAsync(minDate, GetMaxDate());
            List<IGrouping<Dentist, Appointment>> beingTested = (List<IGrouping<Dentist, Appointment>>)result.Model;

            List<Appointment> correctListOfAppointments = correctList.SelectMany(obj => obj).ToList();
            List<Appointment> beingTestedListOfAppointments = beingTested.SelectMany(obj => obj).ToList();

            CollectionAssert.AreEqual(correctListOfAppointments, beingTestedListOfAppointments);
        }

        [TestMethod]
        public async Task NullAndMaxDate_IsView()
        {
            IActionResult result = await Controller_Test.GroupingSearch(null, _timeZoneService.GetTodayOnly());
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task NullAndMaxDate_CorrectModel()
        {
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(null, _timeZoneService.GetTodayOnly());
            Assert.IsTrue(result.Model is List<IGrouping<Dentist, Appointment>>);
        }

        [TestMethod]
        public async Task NullAndMaxDate_CorrectViewData()
        {
            DateTime maxDate = _timeZoneService.GetTodayOnly();
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(null, maxDate);
            Assert.AreEqual(GetMinDateString(), result.ViewData["minDate"]);
            Assert.AreEqual(maxDate.ToString("yyyy-MM-dd"), result.ViewData["maxDate"]);
        }

        [TestMethod]
        public async Task NullAndMaxDate_IdenticalList()
        {
            DateTime maxDate = _timeZoneService.GetTodayOnly();
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(null, maxDate);

            List<IGrouping<Dentist, Appointment>> correctList = await _appointmentService.FindByDateGroupingAsync(GetMinDate(), maxDate);
            List<IGrouping<Dentist, Appointment>> beingTested = (List<IGrouping<Dentist, Appointment>>)result.Model;

            List<Appointment> correctListOfAppointments = correctList.SelectMany(obj => obj).ToList();
            List<Appointment> beingTestedListOfAppointments = beingTested.SelectMany(obj => obj).ToList();

            CollectionAssert.AreEqual(correctListOfAppointments, beingTestedListOfAppointments);
        }

        [TestMethod]
        public async Task MinDateAndMaxDate_IsView()
        {

            IActionResult result = await Controller_Test.GroupingSearch(_timeZoneService.GetTodayOnly(), _timeZoneService.GetTomorrowOnly());
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task MinDateAndMaxDate_CorrectModel()
        {
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(_timeZoneService.GetTodayOnly(), _timeZoneService.GetTomorrowOnly());
            Assert.IsTrue(result.Model is List<IGrouping<Dentist, Appointment>>);
        }

        [TestMethod]
        public async Task MinDateAndMaxDate_CorrectViewData()
        {
            DateTime minDate = _timeZoneService.GetTodayOnly();
            DateTime maxDate = _timeZoneService.GetTomorrowOnly();
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(minDate, maxDate);
            Assert.AreEqual(minDate.ToString("yyyy-MM-dd"), result.ViewData["minDate"]);
            Assert.AreEqual(maxDate.ToString("yyyy-MM-dd"), result.ViewData["maxDate"]);
        }

        [TestMethod]
        public async Task MinDateAndMaxDate_IdenticalList()
        {
            DateTime minDate = _timeZoneService.GetTodayOnly();
            DateTime maxDate = _timeZoneService.GetTomorrowOnly();
            ViewResult result = (ViewResult)await Controller_Test.GroupingSearch(minDate, maxDate);
            List<IGrouping<Dentist, Appointment>> correctList = await _appointmentService.FindByDateGroupingAsync(minDate, maxDate);
            List<IGrouping<Dentist, Appointment>> beingTested = (List<IGrouping<Dentist, Appointment>>)result.Model;

            List<Appointment> correctListOfAppointments = correctList.SelectMany(obj => obj).ToList();
            List<Appointment> beingTestedListOfAppointments = beingTested.SelectMany(obj => obj).ToList();

            CollectionAssert.AreEqual(correctListOfAppointments, beingTestedListOfAppointments);
        }


        private DateTime GetMinDate()
        {
            return new DateTime(
                _timeZoneService.GetTodayOnly().Year,
                1, 1);
        }
        private string GetMinDateString()
        {
            return GetMinDate().ToString("yyyy-MM-dd");
        }
        private DateTime GetMaxDate()
        {
            return new DateTime(
                _timeZoneService.GetTodayOnly().Year,
                12, 31);
        }
        private string GetMaxDateString()
        {
            return GetMaxDate().ToString("yyyy-MM-dd");
        }

    }
}

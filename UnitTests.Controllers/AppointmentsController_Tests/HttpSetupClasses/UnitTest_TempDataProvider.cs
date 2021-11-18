using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Models;

namespace UnitTests.Controllers.AppointmentsController_Tests.HttpSetupClasses
{
    public class UnitTest_TempDataProvider : ITempDataProvider
    {
        public Dictionary<string, object> TempData = new Dictionary<string, object>(1);
        public UnitTest_TempDataProvider(bool toAddAppointment)
        {
            if (toAddAppointment)
            {
                TempData.Add("appointment", GetAppointment().Serialize());
            }
        }

        private Appointment GetAppointment()
        {
            return new Appointment()
            {
                Id = 1,
                Date = new TimeZoneServiceDependecy().GetTodayOnly(),
                DentistId = 1,
                DurationInMinutes = 15,
                Time = new TimeSpan(9, 0, 0),
            };
        }

        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            return TempData;
        }

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
            throw new NotImplementedException();
        }
    }
}

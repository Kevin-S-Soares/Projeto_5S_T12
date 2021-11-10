using System;
using System.Collections.Generic;
using System.Text;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.AppointmentBookDependecies
{
    public class TimeZoneServiceDependecy : ITimeZoneService
    {
        private DateTime _date = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                8, 0, 0);

        public DateTime CurrentTime()
        {
            return _date;
        }
        public DateTime ChangeToBrazilianTime()
        {
            TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZone);
        }
        public void ChangeToEight()
        {
            _date = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                8, 0, 0);
        }
        public void ChangeToFifteen()
        {
            _date = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                15, 0, 0);
        }
        public void ChangeToNineteen()
        {
            _date = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                19, 0, 0);
        }
        public DateTime GetTodayOnly()
        {
            return new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day
                );
        }
        public DateTime GetTomorrowOnly()
        {
            return GetTodayOnly().AddDays(1);
        }
        public DateTime GetYesterdayOnly()
        {
            return GetTodayOnly().AddDays(-1);
        }
    }
}

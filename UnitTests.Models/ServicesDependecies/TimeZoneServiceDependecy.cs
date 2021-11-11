using System;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.ServicesDependecies
{
    public class TimeZoneServiceDependecy : ITimeZoneService
    {
        private DateTime _date = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                8, 0, 0);

        public DateTime GetDate()
        {
            return _date;
        }
        public DateTime GetDateOnly()
        {
            DateTime result = new DateTime(
                GetDate().Year,
                GetDate().Month,
                GetDate().Day
                );
            return result;
        }
        public TimeSpan GetTimeOnly()
        {
            TimeSpan result = new TimeSpan(
                GetDate().Hour,
                GetDate().Minute,
                0);
            return result;
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

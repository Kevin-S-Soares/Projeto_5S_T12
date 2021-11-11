using System;
using WebOdontologista.Models.Interfaces;

namespace WebOdontologista.Services
{
    public class BrazilianTimeZoneService : ITimeZoneService
    {
        public readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        public DateTime GetDate()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZone);
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
    }
}

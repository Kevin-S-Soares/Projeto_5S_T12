using System;
using WebOdontologista.Models.Interfaces;

namespace WebOdontologista.Services
{
    public class BrazilianTimeZoneService : ITimeZoneService
    {
        public readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        public DateTime CurrentTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZone);
        }
    }
}

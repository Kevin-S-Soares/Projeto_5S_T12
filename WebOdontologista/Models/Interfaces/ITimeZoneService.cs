using System;

namespace WebOdontologista.Models.Interfaces
{
    public interface ITimeZoneService
    {
        DateTime GetDate();
        DateTime GetDateOnly();
        TimeSpan GetTimeOnly();
    }
}

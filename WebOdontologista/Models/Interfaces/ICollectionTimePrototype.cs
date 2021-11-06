using System;
using System.Collections.Generic;

namespace WebOdontologista.Models.Interfaces
{
    public interface ICollectionTimePrototype
    {
        void SetSchedule(Dentist dentist);
        ICollectionTimePrototype Clone();
        void MakeAppointment(Appointment appointment);
        void CancelAppointment(Appointment appointment);
        List<TimeSpan> GetAvailableTimes(Appointment appointment);
    }
}

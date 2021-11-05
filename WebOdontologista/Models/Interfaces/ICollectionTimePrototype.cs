using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOdontologista.Models.Interfaces
{
    public interface ICollectionTimePrototype
    {
        void InstantiateMembers(Dentist dentist);
        void SetSchedule(Dentist dentist);
        ICollectionTimePrototype Clone();
        void MakeAppointment(Appointment appointment);
        void CancelAppointment(Appointment appointment);
        List<TimeSpan> GetAvailableTimes(Appointment appointment);

    }
}

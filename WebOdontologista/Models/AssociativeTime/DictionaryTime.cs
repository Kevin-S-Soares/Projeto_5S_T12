using System;
using System.Collections.Generic;
using WebOdontologista.Models.Interfaces;

namespace WebOdontologista.Models.AssociativeTime
{
    public class DictionaryTime : IAssociativeTimePrototype
    {
        private Dictionary<TimeSpan, bool> _dictionary = 
            new Dictionary<TimeSpan, bool>();
        public void Set(Dentist dentist)
        {

        }
        public IAssociativeTimePrototype Clone()
        {
            throw new NotImplementedException();
        }
        public void MakeAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }
        public void CancelAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }
        public List<TimeSpan> AvailableTime(Appointment appointment)
        {
            throw new NotImplementedException();
        }
        public List<TimeSpan> EmptyList(Appointment appointment)
        {
            throw new NotImplementedException();
        }

    }
}

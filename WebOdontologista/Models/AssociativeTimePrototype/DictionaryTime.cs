using System;
using System.Collections.Generic;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models.Interfaces.Implementations.AssociativeTimePrototype
{
    public class DictionaryTime : IAssociativeTimePrototype
    {
        public void Set(Dentist dentist)
        {
            throw new NotImplementedException();
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

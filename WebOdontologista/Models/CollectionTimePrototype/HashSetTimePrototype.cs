using System;
using System.Collections.Generic;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models.CollectionTimePrototype
{
    /*
    * Será implementada uma funcionalidade em uma versão futura.
    * Por enquanto esta classe nada faz.
    */
    public class HashSetTimePrototype : ICollectionTimePrototype
    {
        public void InstantiateMembers(Dentist dentist)
        {
            throw new NotImplementedException();
        }
        public void SetSchedule(Dentist dentist)
        {
            throw new NotImplementedException();
        }
        public ICollectionTimePrototype Clone()
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
        public List<TimeSpan> GetAvailableTimes(Appointment appointment)
        {
            throw new NotImplementedException();
        }
    }
}

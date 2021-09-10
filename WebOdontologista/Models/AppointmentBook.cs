using System;
using System.Collections.Generic;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models
{
    public class AppointmentBook
    {
        public Dictionary<int, Dictionary<DateTime, AppointmentList>> Dentists { get; private set; } = new Dictionary<int, Dictionary<DateTime, AppointmentList>>();
        public AppointmentBook() { }
        public void AddDentist(int id)
        {
            Dentists.Add(id, new Dictionary<DateTime, AppointmentList>());
        }
        public void AddAppointment(Appointment appointment)
        {
            if(!Dentists.ContainsKey(appointment.DentistId))
            {
                AddDentist(appointment.DentistId);
            }
            if (Dentists[appointment.DentistId].ContainsKey(appointment.Date))
            {
                Dentists[appointment.DentistId][appointment.Date].MakeAppointment(appointment);
            }
            else
            {
                Dentists[appointment.DentistId].Add(appointment.Date, new AppointmentList(appointment));
            }
        }
        public void RemoveAppointment(Appointment appointment)
        {
            if (Dentists[appointment.DentistId].ContainsKey(appointment.Date))
            {
                Dentists[appointment.DentistId][appointment.Date].CancelAppointment(appointment);
            }
            else
            {
                throw new DomainException("Consulta não encontrada!");
            }
        }
        public List<TimeSpan> FindAvailableTime(Appointment appointment)
        {
            List<TimeSpan> result;
            if (Dentists.ContainsKey(appointment.DentistId))
            {
                if (Dentists[appointment.DentistId].ContainsKey(appointment.Date))
                {
                    result = Dentists[appointment.DentistId][appointment.Date].AvailableTime(appointment);
                }
                else
                {
                    result = AppointmentList.EmptyList(appointment);
                }
            }
            else
            {
                result = AppointmentList.EmptyList(appointment);
            }
            return result;
            /*
            List<TimeSpan> result;
            if (Dentists[appointment.DentistId].ContainsKey(appointment.Date))
            {
                result = Dentists[appointment.DentistId][appointment.Date].AvailableTime(appointment);
            }
            else
            {
                result = AppointmentList.EmptyList(appointment);
            }
            return result;
            */
        }
    }
}

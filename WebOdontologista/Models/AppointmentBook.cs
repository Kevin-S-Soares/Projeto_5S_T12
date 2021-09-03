using System;
using System.Collections.Generic;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models
{
    public class AppointmentBook
    {
        public Dictionary<int, Dictionary<DateTime, AppointmentList>> Book { get; private set; } = new Dictionary<int, Dictionary<DateTime, AppointmentList>>();
        public AppointmentBook() { }
        public void AddDentist(int id)
        {
            Book.Add(id, new Dictionary<DateTime, AppointmentList>());
        }
        public void AddAppointment(Appointment appointment)
        {
            if (Book[appointment.DentistId].ContainsKey(appointment.Date))
            {
                try
                {
                    Book[appointment.DentistId][appointment.Date].MakeAppointment(appointment);
                }
                catch (ApplicationException e)
                {
                    Console.WriteLine("Erro: " + e.Message);
                }

            }
            else
            {
                Book[appointment.DentistId].Add(appointment.Date, new AppointmentList(appointment));
            }
        }
        public void RemoveAppointment(Appointment appointment)
        {
            if (Book[appointment.DentistId].ContainsKey(appointment.Date))
            {
                try
                {
                    Book[appointment.DentistId][appointment.Date].CancelAppointment(appointment);
                }
                catch (ApplicationException e)
                {
                    Console.WriteLine("Erro: " + e.Message);
                }
            }
            else
            {
                throw new DomainException("Consulta não encontrada!");
            }
        }
        public List<TimeSpan> FindAvailableTime(Appointment appointment)
        {
            List<TimeSpan> result;
            if (Book[appointment.DentistId].ContainsKey(appointment.Date))
            {
                result = Book[appointment.DentistId][appointment.Date].AvailableTime(appointment);
            }
            else
            {
                result = AppointmentList.EmptyList(appointment);
            }
            return result;
        }

    }
}

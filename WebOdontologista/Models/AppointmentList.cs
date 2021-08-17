using System;
using System.Collections.Generic;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models
{
    public class AppointmentList
    {
        public DateTime Date { get; set; }
        public Dictionary<TimeSpan, Appointment> Availability { get; private set; } = new Dictionary<TimeSpan, Appointment>();
        public AppointmentList()
        {
            for (int i = 0; i < 40; i++)
            {
                TimeSpan workTime = new TimeSpan(9, 0 * i, 0);
                Availability.Add(workTime, null);
            }
        }
        public AppointmentList(DateTime date, int duration, Appointment appointment) : base()
        {
            Date = date;
            MakeAppointment(date.TimeOfDay, duration, appointment);
        }
        public void MakeAppointment(TimeSpan time, int duration, Appointment appointment)
        {
            if(!Available(time, duration))
            {
                throw new DomainException("Cannot make an appointment!");
            }
            int n = duration / 15;
            for (int i = 0; i < n; i++)
            {
                time = new TimeSpan(time.Hours, time.Minutes + i * 15, time.Seconds);
                Availability[time] = appointment;
            }
        }
        public void CancelAppointment(TimeSpan time, int duration)
        {
            int n = duration / 15;
            for (int i = 0; i < n; i++)
            {
                time = new TimeSpan(time.Hours, time.Minutes + i * 15, time.Seconds);
                Availability[time] = null;
            }
        }
        public bool Available(TimeSpan time, int duration)
        {
            int n = duration / 15;
            for (int i = 0; i < n; i++)
            {
                time = new TimeSpan(time.Hours, time.Minutes + i * 15, time.Seconds);
                if(Availability[time] != null)
                {
                    return false;
                }
            }
            return true;
        }
        public bool SameDay(DateTime date)
        {
            if(Date.Day == date.Day && Date.Month == date.Month && Date.Year == date.Year)
            {
                return true;
            }
            return false;
        }
        public bool DayBefore(DateTime date)
        {
            if(Date.Year < date.Year || Date.Month < date.Month || Date.Day < date.Day)
            {
                return true;
            }
            return false;
        }
    }
}

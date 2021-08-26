using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models
{
    public class AppointmentList
    {
        /*
         * 0 = Disponível.
         * 1 = Indisponível.
         * Usei operadores bitwise, pois permitem que certos metodos que tomariam tempo O(N*M), tomem O(N).
         */


        public DateTime Date { get; set; }
        public ulong Availability = 61440UL; // bits 12, 13, 14, 15 = 1, i.e, horário de almoço.
        public AppointmentList(Appointment appointment)
        {
            MakeAppointment(appointment);
            Debug.WriteLine(ToString());
        }
        public void MakeAppointment(Appointment appointment)
        {
            if(!Available(appointment))
            {
                throw new DomainException("Não foi possivel adicionar a consulta!");
            }
            int n = appointment.DurationInMinutes / 15;
            int initialBit = (appointment.Date.Hour - 9) * 4 + appointment.Date.Minute / 15;
            int finalBit = initialBit + n;
            for (int i = initialBit; i < finalBit; i++)
            {
                Availability |= 1UL << i;
            }
        }
        public void CancelAppointment(Appointment appointment)
        {
            int n = appointment.DurationInMinutes / 15;
            int initialBit = (appointment.Date.Hour - 9) * 4 + appointment.Date.Minute / 15;
            int finalBit = initialBit + n;
            for (int i = initialBit; i < finalBit; i++)
            {
                Availability ^= 1UL << i;
            }
        }
        public bool Available(Appointment appointment)
        {
            int n = appointment.DurationInMinutes / 15;
            int initialBit = (appointment.Date.Hour - 9) * 4 + appointment.Date.Minute / 15;
            int finalBit = initialBit + n;
            for (int i = initialBit; i < finalBit; i++)
            {
                if((1UL << i & Availability) > 0)
                {
                    return false;
                }
            }
            return true;
        }
        /*
        public List<TimeSpan> AvailableTime()
        {
            List<TimeSpan> result = new List<TimeSpan>();
            for(int i = 0; i < 36; i++)
            {
                if((1UL << i & Availability) == 0)
                {
                    result.Add(new TimeSpan(9, 15 * i, 0));
                }
            }
            return result;
        }
        */
        public List<TimeSpan> AvailableTime(Appointment appointment)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong mask = 0UL;
            for(int i = 0; i < numberOfBits; i++)
            {
                mask |= 1UL << i;
            }
            for(int i = 0; i < 36 - numberOfBits; i++)
            {
                if((mask & Availability) == 0)
                {
                    result.Add(new TimeSpan(9, 15 * i, 0));
                }
                mask <<= 1;
            }
            return result;
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
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < 36; i++)
            {
                if((1UL << i & Availability) == 0)
                {
                    sb.AppendLine(new TimeSpan(9, 15 * i, 0).ToString() + " - " + "Disponível");
                }
                else
                {
                    sb.AppendLine(new TimeSpan(9, 15 * i, 0).ToString() + " - " + "Indisponível");
                }
            }
            return sb.ToString();
        }
        public static List<TimeSpan> EmptyList(Appointment appointment)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            ulong availability = 61440UL;
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong mask = 0UL;
            for (int i = 0; i < numberOfBits; i++)
            {
                mask |= 1UL << i;
            }
            for (int i = 0; i < 36 - numberOfBits; i++)
            {
                if ((mask & availability) == 0)
                {
                    result.Add(new TimeSpan(9, 15 * i, 0));
                }
                mask <<= 1;
            }
            return result;
        }
    }
}

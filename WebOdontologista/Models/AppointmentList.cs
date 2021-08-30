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
         */
        public DateTime Date { get; set; }
        public ulong Availability = 61440UL; // bits 12, 13, 14, 15 = 1, i.e, horário de almoço.
        public AppointmentList(Appointment appointment)
        {
            Date = appointment.Date;
            MakeAppointment(appointment);
        }
        public void MakeAppointment(Appointment appointment)
        {
            byte[] binaryNumber = new byte[] { 0, 1, 3, 7, 15 };
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong duration = binaryNumber[numberOfBits];
            int initialBit = (appointment.Time.Hours - 9) * 4 + appointment.Time.Minutes / 15;
            duration <<= initialBit;
            if ((Availability & duration) > 0UL)
            {
                throw new DomainException("Não foi possivel adicionar a consulta!");
            }
            Availability |= duration;
            /*
            if(!Available(appointment))
            {
                throw new DomainException("Não foi possivel adicionar a consulta!");
            }
            int n = appointment.DurationInMinutes / 15;
            int initialBit = (appointment.Time.Hours - 9) * 4 + appointment.Time.Minutes / 15;
            int finalBit = initialBit + n;
            for (int i = initialBit; i < finalBit; i++)
            {
                Availability |= 1UL << i;
            }
            */
        }
        public void CancelAppointment(Appointment appointment)
        {
            byte[] binaryNumber = new byte[] { 0, 1, 3, 7, 15 };
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong duration = binaryNumber[numberOfBits];
            int initialBit = (appointment.Time.Hours - 9) * 4 + appointment.Time.Minutes / 15;
            duration <<= initialBit;
            if ((Availability & duration) != duration)
            {
                throw new DomainException("Cancelamento de consulta proíbido!");
            }
            Availability ^= duration;
            /*
            int n = appointment.DurationInMinutes / 15;
            int initialBit = (appointment.Time.Hours - 9) * 4 + appointment.Time.Minutes / 15;
            int finalBit = initialBit + n;
            for (int i = initialBit; i < finalBit; i++)
            {
                Availability ^= 1UL << i;
            }
            */
        }
        /*
        public bool Available(Appointment appointment)
        {
            byte[] num = new byte[5] { 0, 1, 3, 7, 15 };
            int n = appointment.DurationInMinutes / 15;
            ulong duration = num[n];
            int initialBit = (appointment.Time.Hours - 9) * 4 + appointment.Time.Minutes / 15;
            duration <<= initialBit;
            if((Availability & duration) > 0UL)
            {
                return false;
            }
            return true;
            /*
            int n = appointment.DurationInMinutes / 15;
            int initialBit = (appointment.Time.Hours - 9) * 4 + appointment.Time.Minutes / 15;
            int finalBit = initialBit + n;
            for (int i = initialBit; i < finalBit; i++)
            {
                if(((1UL << i) & Availability) > 0)
                {
                    return false;
                }
            }
            return true;
            
        }
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
            byte[] binaryNumber = new byte[] { 0, 1, 3, 7, 15 };
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong mask = binaryNumber[numberOfBits];
            for (int i = 0; i < 36 - numberOfBits; i++)
            {
                if ((mask & Availability) == 0)
                {
                    result.Add(new TimeSpan(9, 15 * i, 0));
                }
                mask <<= 1;
            }
            return result;
            /*
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
            */
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
            if(Date.Ticks < date.Ticks)
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
            byte[] binaryNumber = new byte[] { 0, 1, 3, 7, 15 };
            ulong availability = 61440UL;
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong mask = binaryNumber[numberOfBits];
            for (int i = 0; i < 36 - numberOfBits; i++)
            {
                if ((mask & availability) == 0)
                {
                    result.Add(new TimeSpan(9, 15 * i, 0));
                }
                mask <<= 1;
            }
            return result;
            /*
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
            */
        }
    }
}

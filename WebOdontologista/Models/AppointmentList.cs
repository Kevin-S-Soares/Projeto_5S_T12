using System;
using System.Collections.Generic;
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
        private ulong _availability = 61440UL; // bits 12, 13, 14, 15 = 1, i.e, horário de almoço.
        public AppointmentList(Appointment appointment)
        {
            MakeAppointment(appointment);
        }
        public void MakeAppointment(Appointment appointment)
        {
            byte[] durationInBinary = new byte[] { 0, 1, 3, 7, 15 };
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong duration = durationInBinary[numberOfBits];
            int initialBit = (appointment.Time.Hours - 9) * 4 + appointment.Time.Minutes / 15;
            duration <<= initialBit;
            if ((_availability & duration) > 0UL)
            {
                throw new DomainException("Não foi possivel adicionar a consulta!");
            }
            _availability |= duration;
        }
        public void CancelAppointment(Appointment appointment)
        {
            byte[] durationInBinary = new byte[] { 0, 1, 3, 7, 15 };
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong duration = durationInBinary[numberOfBits];
            int initialBit = (appointment.Time.Hours - 9) * 4 + appointment.Time.Minutes / 15;
            duration <<= initialBit;
            if ((_availability & duration) != duration)
            {
                throw new DomainException("Cancelamento de consulta proíbido!");
            }
            _availability ^= duration;
        }
        public List<TimeSpan> AvailableTime(Appointment appointment)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            byte[] durationInBinary = new byte[] { 0, 1, 3, 7, 15 };
            int numberOfBits = appointment.DurationInMinutes / 15;
            ulong mask = durationInBinary[numberOfBits];
            for (int i = 0; i < 36 - numberOfBits; i++)
            {
                if ((mask & _availability) == 0)
                {
                    result.Add(new TimeSpan(9, 15 * i, 0));
                }
                mask <<= 1;
            }
            return result;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 36; i++)
            {
                if ((1UL << i & _availability) == 0)
                {
                    sb.AppendLine(new TimeSpan(9, 15 * i, 0).ToString("HH:mm") + " - " + "Disponível");
                }
                else
                {
                    sb.AppendLine(new TimeSpan(9, 15 * i, 0).ToString("HH:mm") + " - " + "Indisponível");
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
        }
    }
}

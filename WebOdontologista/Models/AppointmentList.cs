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
        private static readonly int _initialHour = 9;
        private static readonly int _finalHour = 18;
        private static readonly int _appointmentsPerHour = 4;
        private static readonly int _hoursDividedByAppointments = 60 / _appointmentsPerHour;
        private static readonly int _totalBits = (_finalHour - _initialHour) * _appointmentsPerHour;
        public AppointmentList(Appointment appointment)
        {
            MakeAppointment(appointment);
        }
        public void MakeAppointment(Appointment appointment)
        {
            ulong mask = Mask(NumberOfBits(appointment));
            mask <<= InitialBitPosition(appointment);
            if ((_availability & mask) > 0UL)
            {
                throw new DomainException("Não foi possivel adicionar a consulta!");
            }
            _availability |= mask;
        }
        public void CancelAppointment(Appointment appointment)
        {
            ulong mask = Mask(NumberOfBits(appointment));
            mask <<= InitialBitPosition(appointment);
            if ((_availability & mask) != mask)
            {
                throw new DomainException("Cancelamento de consulta proíbido!");
            }
            _availability ^= mask;
        }
        public List<TimeSpan> AvailableTime(Appointment appointment)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            int numberOfBits = NumberOfBits(appointment);
            ulong mask = Mask(numberOfBits);
            int length = _totalBits - numberOfBits;
            for (int i = 0; i <= length; i++)
            {
                if ((mask & _availability) == 0)
                {
                    result.Add(new TimeSpan(_initialHour, _hoursDividedByAppointments * i, 0));
                }
                mask <<= 1;
            }
            return result;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _totalBits; i++)
            {
                if ((1UL << i & _availability) == 0)
                {
                    sb.AppendLine(new TimeSpan(_initialHour, _hoursDividedByAppointments * i, 0).ToString("HH:mm") + " - " + "Disponível");
                }
                else
                {
                    sb.AppendLine(new TimeSpan(_initialHour, _hoursDividedByAppointments * i, 0).ToString("HH:mm") + " - " + "Indisponível");
                }
            }
            return sb.ToString();
        }
        public static List<TimeSpan> EmptyList(Appointment appointment)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            ulong availability = 61440UL;
            int numberOfBits = NumberOfBits(appointment);
            ulong mask = Mask(numberOfBits);
            int length = _totalBits - numberOfBits;
            for (int i = 0; i <= length; i++)
            {
                if ((mask & availability) == 0)
                {
                    result.Add(new TimeSpan(_initialHour, _hoursDividedByAppointments * i, 0));
                }
                mask <<= 1;
            }
            return result;
        }
        private static int InitialBitPosition(Appointment appointment)
        {
            return (appointment.Time.Hours - _initialHour) * _appointmentsPerHour + appointment.Time.Minutes / _hoursDividedByAppointments;
        }
        private static int NumberOfBits(Appointment appointment)
        {
            return appointment.DurationInMinutes / _hoursDividedByAppointments;
        }
        private static ulong Mask(int numberOfBits)
        {
            return (1UL << numberOfBits) - 1UL;
        }
    }
}

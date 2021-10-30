using System;
using System.Collections.Generic;
using System.Text;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models.CollectionTimePrototype
{
    public class BitMaskTimePrototype : ICollectionTimePrototype
    {
        /*
         * 0 = Disponível.
         * 1 = Indisponível.
         */

        private int _hourOffSet;
        private int _minuteOffSet;
        private int _appointmentsPerHour;
        private int _hourDividedByAppointmentsPerHour;
        private int _totalBits;
        private ulong _availability;

        public BitMaskTimePrototype() { }
        public void Set(Dentist dentist)
        {
            /*
             * Insira lógica aqui de como os atributos serão instanciados.
            */
            TimeSpan startingTime = new TimeSpan(9, 0, 0);
            TimeSpan endingTime = new TimeSpan(18, 0, 0);
            TimeSpan lunchTime = new TimeSpan(12, 0, 0);
            int durationLunchInMinutes = 60;
            _hourOffSet = startingTime.Hours;
            _minuteOffSet = startingTime.Minutes;
            _appointmentsPerHour = 4;
            _hourDividedByAppointmentsPerHour = 60 / _appointmentsPerHour;
            _totalBits = (int) (endingTime.Subtract(startingTime).TotalMinutes / _hourDividedByAppointmentsPerHour);
            ulong mask = Mask(NumberOfBits(durationLunchInMinutes));
            mask <<= InitialBitPosition(lunchTime);
            _availability |= mask;

        }
        public ICollectionTimePrototype Clone()
        {
            BitMaskTimePrototype result = new BitMaskTimePrototype()
            {
                _hourOffSet = this._hourOffSet,
                _minuteOffSet = this._minuteOffSet,
                _appointmentsPerHour = this._appointmentsPerHour,
                _hourDividedByAppointmentsPerHour = this._hourDividedByAppointmentsPerHour,
                _totalBits = this._totalBits,
                _availability = this._availability,
            };
            return result;
        }
        public void MakeAppointment(Appointment appointment)
        {
            ulong mask = Mask(NumberOfBits(appointment.DurationInMinutes));
            mask <<= InitialBitPosition(appointment.Time);
            if ((_availability & mask) > 0UL)
            {
                throw new DomainException("Não foi possivel adicionar a consulta!");
            }
            _availability |= mask;
        }
        public void CancelAppointment(Appointment appointment)
        {
            ulong mask = Mask(NumberOfBits(appointment.DurationInMinutes)) ;
            mask <<= InitialBitPosition(appointment.Time);
            if ((_availability & mask) != mask)
            {
                throw new DomainException("Cancelamento de consulta proíbido!");
            }
            _availability ^= mask;
        }
        public List<TimeSpan> AvailableTime(Appointment appointment)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            int numberOfBits = NumberOfBits(appointment.DurationInMinutes);
            ulong mask = Mask(numberOfBits);
            int length = _totalBits - numberOfBits;
            for (int i = 0; i <= length; i++)
            {
                if ((mask & _availability) == 0)
                {
                    result.Add(new TimeSpan(_hourOffSet, _hourDividedByAppointmentsPerHour * i + _minuteOffSet, 0));
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
                    sb.AppendLine(new TimeSpan(_hourOffSet, _hourDividedByAppointmentsPerHour * i + _minuteOffSet, 0).ToString("HH:mm") + " - " + "Disponível");
                }
                else
                {
                    sb.AppendLine(new TimeSpan(_hourOffSet, _hourDividedByAppointmentsPerHour * i + _minuteOffSet, 0).ToString("HH:mm") + " - " + "Indisponível");
                }
            }
            return sb.ToString();
        }
        public List<TimeSpan> EmptyList(Appointment appointment)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            ulong availability = 61440UL;
            int numberOfBits = NumberOfBits(appointment.DurationInMinutes);
            ulong mask = Mask(numberOfBits);
            int length = _totalBits - numberOfBits;
            for (int i = 0; i <= length; i++)
            {
                if ((mask & availability) == 0)
                {
                    result.Add(new TimeSpan(_hourOffSet, _hourDividedByAppointmentsPerHour * i + _minuteOffSet, 0));
                }
                mask <<= 1;
            }
            return result;
        }
        private int InitialBitPosition(TimeSpan time)
        {
            return (time.Hours - _hourOffSet) * _appointmentsPerHour + (time.Minutes - _minuteOffSet) / _hourDividedByAppointmentsPerHour;
        }
        private int NumberOfBits(int durationInMinutes)
        {
            return durationInMinutes / _hourDividedByAppointmentsPerHour;
        }
        private ulong Mask(int numberOfBits)
        {
            return (1UL << numberOfBits) - 1UL;
            /*  
             * seja numberOfBits = 4.
             * (1 << 4) = 16 = 0001 0000b.
             * 16 - 1 = 15 = 0000 1111b.
             */
        }
    }
}

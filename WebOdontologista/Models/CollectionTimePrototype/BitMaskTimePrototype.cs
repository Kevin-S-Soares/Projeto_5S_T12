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
        private int _minutesInAnHourDividedByAppointmentsPerHour;
        private int _totalAmountOfBits;
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
            int minutesInAnHour = 60;

            _hourOffSet = startingTime.Hours;
            _minuteOffSet = startingTime.Minutes;
            _appointmentsPerHour = 4;
            _minutesInAnHourDividedByAppointmentsPerHour = minutesInAnHour / _appointmentsPerHour;
            _totalAmountOfBits = (int)endingTime.Subtract(startingTime).TotalMinutes / _minutesInAnHourDividedByAppointmentsPerHour;

            ulong mask = GetPosiotionedMask(durationLunchInMinutes, lunchTime);
            _availability |= mask;
        }
        public ICollectionTimePrototype Clone()
        {
            return new BitMaskTimePrototype()
            {
                _hourOffSet = this._hourOffSet,
                _minuteOffSet = this._minuteOffSet,
                _appointmentsPerHour = this._appointmentsPerHour,
                _minutesInAnHourDividedByAppointmentsPerHour = this._minutesInAnHourDividedByAppointmentsPerHour,
                _totalAmountOfBits = this._totalAmountOfBits,
                _availability = this._availability
            };
        }
        public void MakeAppointment(Appointment appointment)
        {
            ulong mask = GetPosiotionedMask(appointment.DurationInMinutes, appointment.Time);
            if ((_availability & mask) > 0UL)
            {
                throw new DomainException("Não foi possivel adicionar a consulta!");
            }
            _availability |= mask;
        }
        public void CancelAppointment(Appointment appointment)
        {
            ulong mask = GetPosiotionedMask(appointment.DurationInMinutes, appointment.Time);
            if ((_availability & mask) != mask)
            {
                throw new DomainException("Cancelamento de consulta proíbido!");
            }
            _availability ^= mask;
        }
        public List<TimeSpan> GetAvailableTimes(Appointment appointment)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            int amountOfBits = GetAmountOfBits(appointment.DurationInMinutes);
            ulong mask = GetMask(amountOfBits);
            int length = _totalAmountOfBits - amountOfBits;
            for (int i = 0; i <= length; i++)
            {
                if ((mask & _availability) == 0)
                {
                    TimeSpan time = new TimeSpan(_hourOffSet, _minutesInAnHourDividedByAppointmentsPerHour * i + _minuteOffSet, 0);
                    result.Add(time);
                }
                mask <<= 1;
            }
            return result;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _totalAmountOfBits; i++)
            {
                TimeSpan time = new TimeSpan(_hourOffSet, _minutesInAnHourDividedByAppointmentsPerHour * i + _minuteOffSet, 0); ;
                string text = time.ToString("HH:mm") + " - ";
                if ((1UL << i & _availability) == 0)
                {
                    text += "Disponível";
                    sb.AppendLine(text);
                }
                else
                {
                    text += "Indisponível";
                    sb.AppendLine(text);
                }
            }
            return sb.ToString();
        }
        private ulong GetPosiotionedMask(int durationInMinutes, TimeSpan time)
        {
            int amountOfBits = GetAmountOfBits(durationInMinutes);
            ulong mask = GetMask(amountOfBits);
            int bitPosition = GetBitPosition(time);
            mask <<= bitPosition; // posiciona a mascara no lugar correto.
            return mask;
        }
        private int GetBitPosition(TimeSpan time)
        {
            return (time.Hours - _hourOffSet) * _appointmentsPerHour + (time.Minutes - _minuteOffSet) / _minutesInAnHourDividedByAppointmentsPerHour;
        }
        private int GetAmountOfBits(int durationInMinutes)
        {
            return durationInMinutes / _minutesInAnHourDividedByAppointmentsPerHour;
        }
        private ulong GetMask(int amountOfBits)
        {
            return (1UL << amountOfBits) - 1UL;
            /*  
             * seja amountOfBits = 4.
             * (1 << 4) = 16 = 0001 0000b.
             * 16 - 1 = 15 = 0000 1111b.
             */
        }
    }
}

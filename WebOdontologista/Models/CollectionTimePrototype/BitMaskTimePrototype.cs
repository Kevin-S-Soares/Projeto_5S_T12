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

        public BitMaskTimePrototype(Dentist dentist)
        {
            TimeSpan startingTime = new TimeSpan(9, 0, 0);
            TimeSpan endingTime = new TimeSpan(18, 0, 0);

            int minutesInAnHour = 60;

            _hourOffSet = startingTime.Hours;
            _minuteOffSet = startingTime.Minutes;
            _appointmentsPerHour = 4;
            _minutesInAnHourDividedByAppointmentsPerHour = minutesInAnHour / _appointmentsPerHour;
            _totalAmountOfBits = (int)endingTime.Subtract(startingTime).TotalMinutes / _minutesInAnHourDividedByAppointmentsPerHour;
        }
        private BitMaskTimePrototype() { }

        public void SetSchedule(Dentist dentist)
        {
            int lunchDurationInMinutes = 60;
            TimeSpan lunchTime = new TimeSpan(12, 0, 0);
            SetLunchTime(lunchDurationInMinutes, lunchTime);
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
            int amountOfBitsInAMask = GetAmountOfBits(appointment.DurationInMinutes);
            ulong mask = GetMask(amountOfBitsInAMask);
            int length = _totalAmountOfBits - amountOfBitsInAMask;
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
        private void SetLunchTime(int durationInMinutes, TimeSpan lunchTime)
        {
            ulong mask = GetPosiotionedMask(durationInMinutes, lunchTime);
            _availability |= mask;
        }
        private ulong GetPosiotionedMask(int durationInMinutes, TimeSpan time)
        {
            int amountOfBits = GetAmountOfBits(durationInMinutes);
            ulong mask = GetMask(amountOfBits);
            int bitPosition = GetBitPosition(time);
            mask <<= bitPosition;
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
        }
    }
}

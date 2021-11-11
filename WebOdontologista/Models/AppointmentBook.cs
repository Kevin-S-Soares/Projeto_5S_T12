using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Models.Exceptions;
using WebOdontologista.Models.CollectionTimePrototype;

namespace WebOdontologista.Models
{
    public class AppointmentBook
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDentistService _dentistService;
        private readonly ITimeZoneService _timeZoneService;

        private readonly Dictionary<Dentist, Dictionary<DateTime, ICollectionTimePrototype>> _booking =
            new Dictionary<Dentist, Dictionary<DateTime, ICollectionTimePrototype>>();

        private readonly Dictionary<Dentist, ICollectionTimePrototype> _prototypeDictionary =
            new Dictionary<Dentist, ICollectionTimePrototype>();

        public AppointmentBook(IAppointmentService appointmentService, IDentistService dentistService, ITimeZoneService timeZoneService)
        {
            _appointmentService = appointmentService;
            _dentistService = dentistService;
            _timeZoneService = timeZoneService;
        }
        public async Task AddAppointment(Appointment appointment)
        {
            AppointmentIsNotNull(appointment);
            IsValidDate(appointment.DateAndTime());
            await LoadAppointmentDependecies(appointment);
            _booking[appointment.Dentist][appointment.Date].MakeAppointment(appointment);
        }
        public async Task RemoveAppointment(int id)
        {
            Appointment appointment = await _appointmentService.FindByIdAsync(id);
            AppointmentIsNotNull(appointment);
            await LoadAppointmentDependecies(appointment);
            _booking[appointment.Dentist][appointment.Date].CancelAppointment(appointment);
        }
        public async Task RemoveAppointment(Appointment appointment)
        {
            AppointmentIsNotNull(appointment);
            await LoadAppointmentDependecies(appointment);
            _booking[appointment.Dentist][appointment.Date].CancelAppointment(appointment);
        }
        public async Task EditingAppointment(int id)
        {
            await RemoveAppointment(id);
        }
        public async Task EditingAppointment(Appointment appointment)
        {
            await RemoveAppointment(appointment);
        }
        public async Task EditAppointment(Appointment oldAppointment, Appointment newAppointment)
        {
            AppointmentIsNotNull(newAppointment);
            AppointmentIsNotNull(oldAppointment);
            IsValidDate(newAppointment.DateAndTime());
            await RemoveAppointment(oldAppointment);
            await AddAppointment(newAppointment);
        }
        public async Task<List<TimeSpan>> FindAvailableTime(Appointment appointment)
        {
            AppointmentIsNotNull(appointment);
            await LoadAppointmentDependecies(appointment);
            List<TimeSpan> result = _booking[appointment.Dentist][appointment.Date].GetAvailableTimes(appointment);
            if (appointment.Date == GetTodayDateOnly())
            {
                RemovePastTimes(result);
            }
            return result;
        }
        private void IsValidDate(DateTime date)
        {
            if (date < _timeZoneService.CurrentTime())
            {
                throw new DomainException("Data inválida!");
            }
        }
        private void AppointmentIsNotNull(Appointment appointment)
        {
            if (appointment is null)
            {
                throw new DomainException("Consulta não fornecida!");
            }
        }
        private async Task LoadAppointmentDependecies(Appointment appointment)
        {
            await SetupDentist(appointment);
            await SetupAppointments(appointment);
        }
        private async Task SetupDentist(Appointment appointment)
        {
            await AddDentistToAppointmentIfNull(appointment);
            DentistIsNotNull(appointment.Dentist);
            if (!_booking.ContainsKey(appointment.Dentist))
            {
                AddDentist(appointment.Dentist);
                AssociateDentistToPrototype(appointment.Dentist);
                SetDentistPrototype(appointment.Dentist);
            }
        }
        private async Task AddDentistToAppointmentIfNull(Appointment appointment)
        {
            if (appointment.Dentist is null)
            {
                appointment.Dentist = await _dentistService.FindByIdAsync(appointment.DentistId);
            }
        }
        private void DentistIsNotNull(Dentist dentist)
        {
            if (dentist is null)
            {
                throw new DomainException("Dentista inexistente!");
            }
        }
        private void AddDentist(Dentist dentist)
        {
            _booking.Add(dentist, new Dictionary<DateTime, ICollectionTimePrototype>());
        }
        private void AssociateDentistToPrototype(Dentist dentist)
        {
            ICollectionTimePrototype prototype = CollectionTimePrototypeFabric(dentist);
            _prototypeDictionary.Add(dentist, prototype);
        }
        private ICollectionTimePrototype CollectionTimePrototypeFabric(Dentist dentist)
        {
            if (dentist.AppointmentsPerDay() <= 64)
            {
                return new BitMaskTimePrototype(dentist);
            }
            return new HashSetTimePrototype(dentist);
        }
        private void SetDentistPrototype(Dentist dentist)
        {
            _prototypeDictionary[dentist].SetSchedule(dentist);
        }
        private async Task SetupAppointments(Appointment appointment)
        {
            if (!_booking[appointment.Dentist].ContainsKey(appointment.Date))
            {
                List<Appointment> list = await _appointmentService.FindAllAsync(
                    obj => obj.DentistId == appointment.DentistId
                    && obj.Date == appointment.Date);
                AssociateDateToClone(appointment);
                MakeAppointments(list);
            }
        }
        private void AssociateDateToClone(Appointment appointment)
        {
            ICollectionTimePrototype clone = _prototypeDictionary[appointment.Dentist].Clone();
            _booking[appointment.Dentist].Add(appointment.Date, clone);
        }
        private void MakeAppointments(List<Appointment> list)
        {
            foreach (Appointment obj in list)
            {
                _booking[obj.Dentist][obj.Date].MakeAppointment(obj);
            }
        }
        private DateTime GetTodayDateOnly()
        {
            return new DateTime(
                _timeZoneService.CurrentTime().Year,
                _timeZoneService.CurrentTime().Month,
                _timeZoneService.CurrentTime().Day
                );
        }
        private void RemovePastTimes(List<TimeSpan> list)
        {
            TimeSpan timeOfTheDay = _timeZoneService.CurrentTime().TimeOfDay;
            while (list.Count > 0 && list[0] < timeOfTheDay)
            {
                list.RemoveAt(0);
            }
        }
    }
}
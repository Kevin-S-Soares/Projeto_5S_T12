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
        private readonly ICurrentTimeZoneService _currentTimeZoneService;

        private readonly Dictionary<Dentist, Dictionary<DateTime, ICollectionTimePrototype>> _booking =
            new Dictionary<Dentist, Dictionary<DateTime, ICollectionTimePrototype>>();

        private readonly Dictionary<Dentist, ICollectionTimePrototype> _prototypeDictionary =
            new Dictionary<Dentist, ICollectionTimePrototype>();

        public AppointmentBook(IAppointmentService appointmentService, IDentistService dentistService, ICurrentTimeZoneService currentTimeZoneService)
        {
            _appointmentService = appointmentService;
            _dentistService = dentistService;
            _currentTimeZoneService = currentTimeZoneService;
        }
        public async Task AddAppointment(Appointment appointment)
        {
            IsValidDate(appointment.Date);
            await LoadAppointmentDependecies(appointment);
            _booking[appointment.Dentist][appointment.Date].MakeAppointment(appointment);
        }
        public async Task RemoveAppointment(int id)
        {
            Appointment appointment = await _appointmentService.FindByIdAsync(id);
            await LoadAppointmentDependecies(appointment);
            _booking[appointment.Dentist][appointment.Date].CancelAppointment(appointment);
        }
        public async Task RemoveAppointment(Appointment appointment)
        {
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
            IsValidDate(newAppointment.Date);
            await RemoveAppointment(oldAppointment);
            await AddAppointment(newAppointment);
        }
        public async Task<List<TimeSpan>> FindAvailableTime(Appointment appointment)
        {
            await LoadAppointmentDependecies(appointment);
            List<TimeSpan> result = GetListOfTimesFromPrototypeOrClone(appointment);
            if(appointment.Date == GetTodayDateOnly())
            {
                RemovePastTimes(result);
            }
            return result;
        }
        private void IsValidDate(DateTime date)
        {
            if (date < GetTodayDateOnly())
            {
                throw new DomainException("Data inválida!");
            }
        }
        private DateTime GetTodayDateOnly()
        {
            return new DateTime(
                _currentTimeZoneService.CurrentTime().Year,
                _currentTimeZoneService.CurrentTime().Month,
                _currentTimeZoneService.CurrentTime().Day
                );
        }
        public async Task LoadAppointmentDependecies(Appointment appointment)
        {
            await SetupDentist(appointment);
            await SetupAppointments(appointment);
        }
        public async Task SetupDentist(Appointment appointment)
        {
            if (appointment.Dentist == null)
            {
                appointment.Dentist = await _dentistService.FindByIdAsync(appointment.DentistId);
            }
            if (!_booking.ContainsKey(appointment.Dentist))
            {
                AddDentist(appointment.Dentist);
                AddDentistPrototype(appointment.Dentist);
            }
        }
        private void AddDentist(Dentist dentist)
        {
            _booking.Add(dentist, new Dictionary<DateTime, ICollectionTimePrototype>());
        }
        private void AddDentistPrototype(Dentist dentist)
        {
            ICollectionTimePrototype prototype;
            if (dentist.AppointmentsPerDay() <= 64)
            {
                prototype = new BitMaskTimePrototype();
            }
            else
            {
                prototype = new HashSetTimePrototype();
            }
            prototype.InstantiateMembers(dentist);
            prototype.SetSchedule(dentist);
            _prototypeDictionary.Add(dentist, prototype);
        }
        private async Task SetupAppointments(Appointment appointment)
        {
            if (!_booking[appointment.Dentist].ContainsKey(appointment.Date))
            {
                var list = await _appointmentService.FindAllAsync(obj => obj.DentistId == appointment.DentistId && obj.Date == appointment.Date);
                ClonePrototypeInADate(appointment);
                MakeAppointments(list);
            }
        }

        private void ClonePrototypeInADate(Appointment appointment)
        {
            ICollectionTimePrototype clone = _prototypeDictionary[appointment.Dentist].Clone();
            _booking[appointment.Dentist].Add(appointment.Date, clone);
        }
        private void MakeAppointments(List<Appointment> list)
        {
            try
            {
                foreach (Appointment obj in list)
                {
                    _booking[obj.Dentist][obj.Date].MakeAppointment(obj);
                }
            }
            catch (DomainException)
            {
                throw new DomainException("Erro ao carregar os dados!");
            }
        }
        private List<TimeSpan> GetListOfTimesFromPrototypeOrClone(Appointment appointment)
        {
            if (_booking.ContainsKey(appointment.Dentist) && _booking[appointment.Dentist].ContainsKey(appointment.Date))
            {
                return _booking[appointment.Dentist][appointment.Date].GetAvailableTimes(appointment);
            }
            return _prototypeDictionary[appointment.Dentist].GetAvailableTimes(appointment);
        }
        private void RemovePastTimes(List<TimeSpan> list)
        {
            TimeSpan timeOfTheDay = _currentTimeZoneService.CurrentTime().TimeOfDay;
            while(list[0] < timeOfTheDay)
            {
                list.RemoveAt(0);
            }
        }
    }
}
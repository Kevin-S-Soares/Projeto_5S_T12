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

        private readonly Dictionary<Dentist, Dictionary<DateTime, ICollectionTimePrototype>> _dentists =
            new Dictionary<Dentist, Dictionary<DateTime, ICollectionTimePrototype>>();

        private readonly Dictionary<Dentist, ICollectionTimePrototype> _prototypeDictionary =
            new Dictionary<Dentist, ICollectionTimePrototype>();

        private readonly Dictionary<Dentist, HashSet<DateTime>> _loadedDentists =
            new Dictionary<Dentist, HashSet<DateTime>>();

        public AppointmentBook(IAppointmentService appointmentService, IDentistService dentistService, ICurrentTimeZoneService currentTimeZoneService)
        {
            _appointmentService = appointmentService;
            _dentistService = dentistService;
            _currentTimeZoneService = currentTimeZoneService;
        }
        public async Task AddAppointment(Appointment appointment)
        {
            Dentist dentist = await Setup(appointment);
            if (appointment.DateAndTime() < _currentTimeZoneService.CurrentTime())
            {
                throw new DomainException("Data inválida!");
            }
            if (_dentists[dentist].ContainsKey(appointment.Date))
            {
                _dentists[dentist][appointment.Date].MakeAppointment(appointment);
            }
            else
            {
                ICollectionTimePrototype clone = _prototypeDictionary[dentist].Clone();
                clone.MakeAppointment(appointment);
                _dentists[dentist].Add(appointment.Date, clone);
            }
        }
        public async Task RemoveAppointment(int id)
        {
            Appointment appointment = await _appointmentService.FindByIdAsync(id);
            Dentist dentist = await Setup(appointment);
            if (_dentists[dentist].ContainsKey(appointment.Date))
            {
                _dentists[dentist][appointment.Date].CancelAppointment(appointment);
            }
            else
            {
                throw new DomainException("Consulta não encontrada!");
            }
        }
        public async Task EditingAppointment(int id)
        {
            await RemoveAppointment(id);
        }
        public async Task EditingAppointment(Appointment appointment)
        {
            Dentist dentist = await Setup(appointment);
            if (_dentists[dentist].ContainsKey(appointment.Date))
            {
                _dentists[dentist][appointment.Date].CancelAppointment(appointment);
            }
            else
            {
                throw new DomainException("Consulta não encontrada!");
            }
        }
        public async Task EditAppointment(Appointment oldAppointment, Appointment newAppointment)
        {
            if (newAppointment.DateAndTime() < _currentTimeZoneService.CurrentTime())
            {
                throw new DomainException("Data inválida!");
            }
            Dentist oldDentist = await Setup(oldAppointment);
            Dentist newDentist;
            if (newAppointment.DentistId != oldAppointment.DentistId)
            {
                newDentist = await Setup(newAppointment);
            }
            else
            {
                newDentist = oldDentist;
            }
            if (_dentists[oldDentist].ContainsKey(oldAppointment.Date))
            {
                _dentists[oldDentist][oldAppointment.Date].CancelAppointment(oldAppointment);
            }
            else
            {
                throw new DomainException("Consulta não encontrada!");
            }
            if (_dentists[newDentist].ContainsKey(newAppointment.Date))
            {
                _dentists[newDentist][newAppointment.Date].MakeAppointment(newAppointment);
            }
            else
            {
                ICollectionTimePrototype clone = _prototypeDictionary[newDentist].Clone();
                clone.MakeAppointment(newAppointment);
                _dentists[newDentist].Add(newAppointment.Date, clone);
            }
        }
        public async Task<List<TimeSpan>> FindAvailableTime(Appointment appointment)
        {
            Dentist dentist = await Setup(appointment);
            List<TimeSpan> result;
            if (_dentists.ContainsKey(dentist) && _dentists[dentist].ContainsKey(appointment.Date))
            {
                result = _dentists[dentist][appointment.Date].AvailableTime(appointment);
            }
            else
            {
                result = _prototypeDictionary[dentist].EmptyList(appointment);
            }
            RemovePastTime(result, appointment);
            return result;

        }
        private async Task<Dentist> Setup(Appointment appointment)
        {
            Dentist dentist = await AddDentist(appointment);
            await AddAppointmentsByDentist(dentist, appointment.Date);
            return dentist;
        }
        private async Task<Dentist> AddDentist(Appointment appointment)
        {
            Dentist dentist = await _dentistService.FindByIdAsync(appointment.DentistId);
            if (dentist != null)
            {
                if (!_dentists.ContainsKey(dentist))
                {
                    _dentists.Add(dentist, new Dictionary<DateTime, ICollectionTimePrototype>());
                    ICollectionTimePrototype prototype;
                    if (dentist.AppointmentsPerDay() <= 64)
                    {
                        prototype = new BitMaskTimePrototype();
                    }
                    else
                    {
                        prototype = null;
                    }
                    prototype.Set(dentist);
                    _prototypeDictionary.Add(dentist, prototype);
                }
            }
            else
            {
                throw new DomainException("Odontologista não existe!");
            }
            return dentist;
        }
        private async Task AddAppointmentsByDentist(Dentist dentist, DateTime date)
        {
            if (!_loadedDentists.ContainsKey(dentist) || !_loadedDentists[dentist].Contains(date))
            {
                List<Appointment> result = await _appointmentService.FindAllAsync(obj => obj.DentistId == dentist.Id && obj.Date == date);
                try
                {
                    foreach (Appointment obj in result)
                    {
                        if (_dentists[dentist].ContainsKey(obj.Date))
                        {
                            _dentists[dentist][obj.Date].MakeAppointment(obj);
                        }
                        else
                        {
                            ICollectionTimePrototype clone = _prototypeDictionary[dentist].Clone();
                            clone.MakeAppointment(obj);
                            _dentists[dentist].Add(obj.Date, clone);
                        }
                    }
                }
                catch (DomainException)
                {
                    throw new DomainException("Erro ao carregar os dados!");
                }
                if (!_loadedDentists.ContainsKey(dentist))
                {
                    _loadedDentists.Add(dentist, new HashSet<DateTime>());
                }
                if (!_loadedDentists[dentist].Contains(date))
                {
                    _loadedDentists[dentist].Add(date);
                }

            }
        }
        private void RemovePastTime(List<TimeSpan> list, Appointment appointment)
        {
            DateTime today = new DateTime(
                _currentTimeZoneService.CurrentTime().Year,
                _currentTimeZoneService.CurrentTime().Month,
                _currentTimeZoneService.CurrentTime().Day
                                        );

            if (appointment.Date == today)
            {
                TimeSpan timeNow = _currentTimeZoneService.CurrentTime().TimeOfDay;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] < timeNow)
                    {
                        list.Remove(list[0]);
                        i--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}

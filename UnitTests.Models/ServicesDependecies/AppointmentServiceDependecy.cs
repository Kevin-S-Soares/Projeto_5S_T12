using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.ServicesDependecies
{
    public class AppointmentServiceDependecy : IAppointmentService
    {
        private readonly List<Appointment> _list = new List<Appointment>();
        private readonly List<Dentist> _dentists = new DentistServiceDependecy().FindAllDentists();
        private readonly TimeZoneServiceDependecy _timeZoneService = new TimeZoneServiceDependecy();

        public AppointmentServiceDependecy()
        {
            GenerateAppointments();
        }

        public async Task<List<Appointment>> FindAllAsync(Expression<Func<Appointment, bool>> expression)
        {
            await Task.Delay(0);
            return _list
                .AsQueryable()
                .Where(expression)
                .OrderBy(obj => obj.DateAndTime())
                .ToList();
        }

        public async Task<List<Appointment>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _list select obj;
            if (minDate.HasValue)
            {
                result = result.Where(obj => obj.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(obj => obj.Date <= maxDate.Value);
            }
            await Task.Delay(0);
            return result.OrderBy(obj => obj.Date).ToList();
        }

        public async Task<List<IGrouping<Dentist, Appointment>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _list select obj;
            if (minDate.HasValue)
            {
                result = result.Where(obj => obj.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(obj => obj.Date <= maxDate.Value);
            }
            await Task.Delay(0);
            return result.OrderBy(obj => obj.Date).GroupBy(obj => obj.Dentist).ToList();
        }

        public async Task<Appointment> FindByIdAsync(int id)
        {
            await Task.Delay(0);
            return _list.FirstOrDefault(obj => obj.Id == id);
        }

        public async Task InsertAsync(Appointment appointment)
        {
            await Task.Delay(0);
            _list.Add(appointment);
        }

        public async Task RemoveByIdAsync(int id)
        {
            await Task.Delay(0);
            _list.RemoveAll(obj => obj.Id == id);
        }

        public Task UpdateAsync(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        private void GenerateAppointments()
        {
            _list.AddRange(new Appointment[]
            {
                new Appointment()
                {
                    Id = 1,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 15,
                    Time = new TimeSpan(9, 0, 0),
                },
                new Appointment()
                {
                    Id = 2,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 15,
                    Time = new TimeSpan(9, 15, 0)
                },
                new Appointment()
                {
                    Id = 3,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 15,
                    Time = new TimeSpan(9, 30, 0)
                },
                new Appointment()
                {
                    Id = 4,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 15,
                    Time = new TimeSpan(9, 45, 0)
                },
                new Appointment()
                {
                    Id = 5,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 60,
                    Time = new TimeSpan(10, 0, 0)
                },
                new Appointment()
                {
                    Id = 6,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 30,
                    Time = new TimeSpan(11, 0, 0)
                },
                new Appointment()
                {
                    Id = 7,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 30,
                    Time = new TimeSpan(11, 30, 0)
                },
                new Appointment()
                {
                    Id = 8,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 15,
                    Time = new TimeSpan(13, 0, 0)
                },
                new Appointment()
                {
                    Id = 9,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 45,
                    Time = new TimeSpan(13, 15, 0)
                },
                new Appointment()
                {
                    Id = 10,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 60,
                    Time = new TimeSpan(14, 0, 0)
                },
                new Appointment()
                {
                    Id = 11,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 45,
                    Time = new TimeSpan(15, 0, 0)
                },
                new Appointment()
                {
                    Id = 12,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 15,
                    Time = new TimeSpan(15, 45, 0)
                },
                new Appointment()
                {
                    Id = 13,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 15,
                    Time = new TimeSpan(16, 0, 0)
                },
                new Appointment()
                {
                    Id = 14,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 15,
                    Time = new TimeSpan(16, 15, 0)
                },
                new Appointment()
                {
                    Id = 15,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 30,
                    Time = new TimeSpan(16, 30, 0)
                },
                new Appointment()
                {
                    Id = 16,
                    Date = _timeZoneService.GetTodayOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 60,
                    Time = new TimeSpan(17, 0, 0)
                },
                new Appointment()
                {
                    Id = 17,
                    Date = _timeZoneService.GetTomorrowOnly(),
                    DentistId = 1,
                    Dentist = _dentists.First(obj => obj.Id == 1),
                    DurationInMinutes = 60,
                    Time = new TimeSpan(9, 0, 0)
                },
                new Appointment()
                {
                    Id = 18,
                    Date = _timeZoneService.GetTomorrowOnly(),
                    DentistId = 2,
                    Dentist = _dentists.First(obj => obj.Id == 2),
                    DurationInMinutes = 60,
                    Time = new TimeSpan(9, 0, 0)
                },
                new Appointment()
                {
                    Id = 19,
                    Date = _timeZoneService.GetTomorrowOnly().AddDays(7),
                    DentistId = 2,
                    Dentist = _dentists.First(obj => obj.Id == 2),
                    DurationInMinutes = 60,
                    Time = new TimeSpan(9, 0, 0)
                },
                new Appointment()
                {
                    Id = 20,
                    Date = _timeZoneService.GetTomorrowOnly().AddDays(30),
                    DentistId = 2,
                    Dentist = _dentists.First(obj => obj.Id == 2),
                    DurationInMinutes = 60,
                    Time = new TimeSpan(9, 0, 0)
                },
                new Appointment()
                {
                    Id = 20,
                    Date = _timeZoneService.GetTomorrowOnly().AddDays(35),
                    DentistId = 2,
                    Dentist = _dentists.First(obj => obj.Id == 2),
                    DurationInMinutes = 60,
                    Time = new TimeSpan(9, 0, 0)
                },
            });
        }
    }
}

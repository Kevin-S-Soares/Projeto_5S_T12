using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Data;
using WebOdontologista.Models;
using WebOdontologista.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using WebOdontologista.Services.Exceptions;
using System.Diagnostics;
using System.Linq.Expressions;

namespace WebOdontologista.Services
{
    public class AppointmentService
    {
        public AppointmentBook Book = new AppointmentBook();
        public readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        public DateTime Now
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZone);
            }
            private set { }
        }
        private readonly ApplicationDbContext _context;
        private readonly DentistService _dentistService;
        public AppointmentService(ApplicationDbContext context, DentistService dentistService)
        {
            _context = context;
            _dentistService = dentistService;
            BookingService();
        }
        public List<Appointment> FindAll()
        {
            DateTime sameDay = new DateTime(Now.Year, Now.Month, Now.Day);
            List<Appointment> result =
                _context.Appointment.Include(obj => obj.Dentist)
                .Where(obj => obj.DateAndTime() >= sameDay)
                .OrderBy(obj => obj.DateAndTime())
                .ToList();
            return result;
        }
        public async Task<List<Appointment>> FindAllAsync()
        {
            return await FindGenericAsync(obj => obj.DateAndTime() >= Now);
        }
        public async Task<List<Appointment>> FindDailyAsync()
        {
            DateTime sameDay = new DateTime(Now.Year, Now.Month, Now.Day);
            return await FindGenericAsync(obj => obj.DateAndTime() >= Now && obj.Date == sameDay);
        }
        public async Task<List<Appointment>> FindWeeklyAsync()
        {
            DateTime sameWeek = new DateTime(Now.Year, Now.Month, Now.Day).AddDays(7);
            return await FindGenericAsync(obj => obj.DateAndTime() >= Now && obj.Date <= sameWeek);
        }
        public async Task<List<Appointment>> FindMonthlyAsync()
        {
            DateTime sameMonth = new DateTime(Now.Year, Now.Month, Now.Day).AddDays(30);
            return await FindGenericAsync(obj => obj.DateAndTime() >= Now && obj.Date <= sameMonth);
        }
        public async Task<AppointmentFormViewModel> ViewModel()
        {
            List<Dentist> dentists = await _dentistService.FindAllAsync();
            AppointmentFormViewModel viewModel = new AppointmentFormViewModel() { Dentists = dentists };
            return viewModel;
        }
        public async Task InsertAsync(Appointment appointment)
        {
            _context.Add(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task<Appointment> FindByIdAsync(int id)
        {
            Appointment appointment = await
                _context.Appointment.Include(obj => obj.Dentist)
                .FirstOrDefaultAsync(obj => obj.Id == id);
            return appointment;
        }
        public async Task RemoveAsync(int id)
        {
            Appointment appointment = await _context.Appointment.FindAsync(id);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Appointment appointment)
        {

            bool hasAny = await _context.Appointment.AnyAsync(obj => obj.Id == appointment.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado!");
            }
            try
            {
                Appointment entry = await _context.Appointment.FirstAsync(obj => obj.Id == appointment.Id);
                _context.Entry(entry).CurrentValues.SetValues(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
        public async Task<List<Appointment>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.Appointment select obj;
            if (minDate.HasValue)
            {
                result = result.Where(obj => obj.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(obj => obj.Date <= maxDate.Value);
            }
            return await result.Include(obj => obj.Dentist).OrderBy(obj => obj.Date).ToListAsync();
        }
        public async Task<List<IGrouping<Dentist, Appointment>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.Appointment select obj;
            if (minDate.HasValue)
            {
                result = result.Where(obj => obj.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(obj => obj.Date <= maxDate.Value);
            }
            return await result.Include(obj => obj.Dentist).OrderBy(obj => obj.Date).GroupBy(obj => obj.Dentist).ToListAsync();
        }
        private void BookingService()
        {
            List<Appointment> appointments = FindAll();
            foreach (Appointment obj in appointments)
            {
                if (Book.Dentists.ContainsKey(obj.DentistId))
                {
                    Book.AddAppointment(obj);
                }
                else
                {
                    Book.AddDentist(obj.DentistId);
                    Book.AddAppointment(obj);
                }
            }
        }
        private async Task<List<Appointment>> FindGenericAsync(Expression<Func<Appointment, bool>> predicate)
        {
            List<Appointment> result = await
             _context.Appointment.Include(obj => obj.Dentist)
            .Where(predicate)
            .OrderBy(obj => obj.DateAndTime())
            .ToListAsync();
            return result;
        }
    }
}

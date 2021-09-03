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

namespace WebOdontologista.Services
{
    public class AppointmentService
    {
        private readonly WebOdontologistaContext _context;
        private readonly DentistService _dentistService;
        public AppointmentBook Book = new AppointmentBook();
        public AppointmentService(WebOdontologistaContext context, DentistService dentistService)
        {
            _context = context;
            _dentistService = dentistService;
            List<Dentist> dentists = _dentistService.FindAll();
            foreach(Dentist obj in dentists)
            {
               Book.AddDentist(obj.Id);
            }
            List<Appointment> appointments = FindAll();
            foreach(Appointment obj in appointments)
            {
                Book.AddAppointment(obj);
            }
        }
        public List<Appointment> FindAll()
        {
            List<Appointment> listOfAppointments = _context.Appointment.Include(obj => obj.Dentist).OrderBy(obj => obj.DateAndTime()).ToList(); //.FindAll(obj => DateAndTime() > DateTime.Now); Produto final
            return listOfAppointments;
        }
        public async Task<List<Appointment>> FindAllAsync()
        {
            List<Appointment> listOfAppointments = await _context.Appointment.Include(obj => obj.Dentist).OrderBy(obj => obj.DateAndTime()).ToListAsync(); //.FindAll(obj => DateAndTime() > DateTime.Now); Produto final
            return listOfAppointments;
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
        public Appointment FindById(int id)
        {
            return _context.Appointment.AsNoTracking().Include(obj => obj.Dentist).FirstOrDefault(obj => obj.Id == id);
        }
        public async Task<Appointment> FindByIdAsync(int id)
        {
            Appointment appointment = await _context.Appointment.AsNoTracking().Include(obj => obj.Dentist).FirstOrDefaultAsync(obj => obj.Id == id);
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
                var entry = await _context.Appointment.FirstAsync(obj => obj.Id == appointment.Id);
                _context.Entry(entry).CurrentValues.SetValues(appointment);
                await _context.SaveChangesAsync();
            } catch(DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
        public async Task<List<Appointment>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.Appointment select obj;
            if(minDate.HasValue)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Data;
using WebOdontologista.Models;
using WebOdontologista.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using WebOdontologista.Services.Exceptions;

namespace WebOdontologista.Services
{
    public class AppointmentService
    {
        private readonly WebOdontologistaContext _context;
        private readonly DentistService _dentistService;
        public AppointmentService(WebOdontologistaContext context, DentistService dentistService)
        {
            _context = context;
            _dentistService = dentistService;
        }
        public List<Appointment> FindAll()
        {
            List<Appointment> listOfAppointments = _context.Appointment.Include(obj => obj.Dentist).OrderBy(x => x.Date).ToList(); //.FindAll(x => x.Date > DateTime.Now); Produto final
            return listOfAppointments;
        }
        public AppointmentFormViewModel ViewModel()
        {
            List<Dentist> dentists = _dentistService.FindAll();
            AppointmentFormViewModel viewModel = new AppointmentFormViewModel() { Dentists = dentists };
            return viewModel;
        }
        public void Insert(Appointment appointment)
        {
            _context.Add(appointment);
            _context.SaveChanges();
        }
        public Appointment FindById(int id)
        {
            return _context.Appointment.Include(obj => obj.Dentist).FirstOrDefault(obj => obj.Id == id);
        }
        public void Remove(int id)
        {
            Appointment obj = _context.Appointment.Find(id);
            _context.Appointment.Remove(obj);
            _context.SaveChanges();
        }
        public void Update(Appointment appointment)
        {
            if(!_context.Appointment.Any(x => x.Id == appointment.Id))
            {
                throw new NotFoundException("Id não encontrado!");
            }
            try
            {
                _context.Update(appointment);
                _context.SaveChanges();
            } catch(DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}

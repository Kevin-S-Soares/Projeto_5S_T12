using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Data;
using WebOdontologista.Models;
using WebOdontologista.Models.ViewModels;

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
            List<Appointment> listOfAppointments = _context.Appointment.ToList(); //.FindAll(x => x.Date > DateTime.Now); Produto final
            Dictionary<int, Dentist> primaryKeyDentist = _dentistService.PrimaryKey();
            foreach (Appointment appointment in listOfAppointments)
            {
                appointment.Dentist = primaryKeyDentist[appointment.DentistId];
            }
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
            appointment.DentistId = 1;
            _context.Add(appointment);
            _context.SaveChanges();
        }
    }
}

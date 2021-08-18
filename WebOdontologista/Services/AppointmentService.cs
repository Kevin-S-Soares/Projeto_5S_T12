using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Data;
using WebOdontologista.Models;

namespace WebOdontologista.Services
{
    public class AppointmentService
    {
        private readonly WebOdontologistaContext _context;
        private readonly DentistService _dentistService;
        public AppointmentService(WebOdontologistaContext context, DentistService dentistservice)
        {
            _context = context;
            _dentistService = dentistservice;
        }
        public List<Appointment> FindAll()
        {
            List<Appointment> listOfAppointments = _context.Appointment.ToList(); //.FindAll(x => x.Date > DateTime.Now); Para testes
            List<Dentist> listOfDentists = _dentistService.FindAll();
            Dictionary<int, Dentist> primaryKeyDentist = new Dictionary<int, Dentist>();
            foreach (Dentist dentist in listOfDentists)
            {
                if (!primaryKeyDentist.ContainsKey(dentist.Id))
                {
                    primaryKeyDentist.Add(dentist.Id, dentist);
                }
            }
            foreach (Appointment appointment in listOfAppointments)
            {
                appointment.Dentist = primaryKeyDentist[appointment.DentistId];
            }
            return listOfAppointments;
        }
    }
}

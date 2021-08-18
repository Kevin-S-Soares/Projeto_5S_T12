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
        public AppointmentService(WebOdontologistaContext context)
        {
            _context = context;
        }
        public List<Appointment> FindAll()
        {
            List<Appointment> listOfAppointments = _context.Appointment.ToList(); //.FindAll(x => x.Date > DateTime.Now); Produto final
            Dictionary<int, Dentist> primaryKeyDentist = _context.Dentist.ToDictionary<Dentist, int>(x => x.Id);
            foreach (Appointment appointment in listOfAppointments)
            {
                appointment.Dentist = primaryKeyDentist[appointment.DentistId];
            }
            return listOfAppointments;
        }
        public void Insert(Appointment appointment)
        {
            appointment.DentistId = 1;
            _context.Add(appointment);
            _context.SaveChanges();
        }
    }
}

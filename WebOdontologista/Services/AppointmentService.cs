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
        public AppointmentService(WebOdontologistaContext  context)
        {
            _context = context;
        }
        public List<Appointment> FindAll()
        {
            return _context.Appointment.ToList();
        }
    }
}

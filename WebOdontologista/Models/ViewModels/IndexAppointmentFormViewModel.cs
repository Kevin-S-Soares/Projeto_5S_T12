using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOdontologista.Models.ViewModels
{
    public class IndexAppointmentFormViewModel
    {
        public ICollection<Appointment> Appointments { get; set; }
        public int Show { get; set; }
        public Appointment Appointment { get; set; }
    }
}

using System.Collections.Generic;

namespace WebOdontologista.Models.ViewModels
{
    public class IndexAppointmentFormViewModel
    {
        public ICollection<Appointment> Appointments { get; set; }
        public int Show { get; set; }
        public Appointment Appointment { get; set; }
    }
}

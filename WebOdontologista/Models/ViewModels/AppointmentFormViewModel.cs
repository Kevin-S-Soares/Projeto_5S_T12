using System;
using System.Collections.Generic;

namespace WebOdontologista.Models.ViewModels
{
    public class AppointmentFormViewModel
    {
        public Appointment Appointment { get; set; }
        public bool Visible { get; set; }
        public ICollection<TimeSpan> AvailableTime { get; set; }
        public ICollection<Dentist> Dentists { get; set; }
    }
}

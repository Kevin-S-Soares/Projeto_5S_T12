using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebOdontologista.Models.Interfaces;

namespace WebOdontologista.Models.ViewModels
{
    public class AppointmentFormViewModel
    {
        public Appointment Appointment { get; set; }
        public ICollection<TimeSpan> AvailableTime { get; set; }
        public ICollection<Dentist> Dentists { get; set; }

        private AppointmentFormViewModel() { }

        public async static Task<AppointmentFormViewModel> CreateFormViewModel(
            IDentistService dentistService)
        {
            return new AppointmentFormViewModel()
            {
                Dentists = await dentistService.FindAllAsync(),
            };
        }
    }
}

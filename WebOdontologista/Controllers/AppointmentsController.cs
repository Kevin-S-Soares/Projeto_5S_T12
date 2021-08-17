using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Services;

namespace WebOdontologista.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly DentistService _dentistService;

        public AppointmentsController(AppointmentService appointmentService, DentistService dentistService)
        {
            _appointmentService = appointmentService;
            _dentistService = dentistService;
        }
        public IActionResult Index()
        {
            var list = _appointmentService.FindAll();
            var listOfDentists = _dentistService.FindAll();

            foreach(Appointment app in list)
            {
                app.Dentist = listOfDentists.Find(x => app.DentistId == x.Id);
            }
            return View(list);
        }
    }
}

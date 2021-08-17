using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Services;

namespace WebOdontologista.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentsController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        public IActionResult Index()
        {
            var list = _appointmentService.FindAll();
            return View(list);
        }
    }
}

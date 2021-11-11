using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace WebOdontologista.Controllers
{
    [Authorize]
    public class AppointmentsHistoryController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ITimeZoneService _timeZoneService;
        public AppointmentsHistoryController(IAppointmentService appointmentService,
            ITimeZoneService currentTimeZoneService)
        {
            _appointmentService = appointmentService;
            _timeZoneService = currentTimeZoneService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SimpleSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(_timeZoneService.GetDate().Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = new DateTime(_timeZoneService.GetDate().Year, 12, 31);
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            List<Appointment> result = await _appointmentService.FindByDateAsync(minDate, maxDate);
            return View(result);
        }
        public async Task<IActionResult> GroupingSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(_timeZoneService.GetDate().Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = new DateTime(_timeZoneService.GetDate().Year, 12, 31);
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            List<IGrouping<Dentist, Appointment>> result = await _appointmentService.FindByDateGroupingAsync(minDate, maxDate);
            return View(result);
        }
        public IActionResult Error(string message)
        {
            ErrorViewModel error = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(error);
        }
    }
}

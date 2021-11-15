using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            AddMinDateIfNull(ref minDate);
            AddMaxDateIfNull(ref maxDate);
            CreateViewData(minDate, maxDate);
            List<Appointment> result = await _appointmentService.FindByDateAsync(minDate, maxDate);
            return View(result);
        }

        public async Task<IActionResult> GroupingSearch(DateTime? minDate, DateTime? maxDate)
        {
            AddMinDateIfNull(ref minDate);
            AddMaxDateIfNull(ref maxDate);
            CreateViewData(minDate, maxDate);
            List<IGrouping<Dentist, Appointment>> result = await _appointmentService.FindByDateGroupingAsync(minDate, maxDate);
            return View(result);
        }

        private void AddMinDateIfNull(ref DateTime? minDate)
        {
            if (minDate is null)
            {
                minDate = new DateTime(_timeZoneService.GetDate().Year, 1, 1);
            }
        }
        private void AddMaxDateIfNull(ref DateTime? maxDate)
        {
            if (maxDate is null)
            {
                maxDate = new DateTime(_timeZoneService.GetDate().Year, 12, 31);
            }
        }
        private void CreateViewData(DateTime? minDate, DateTime? maxDate)
        {
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
        }
    }
}

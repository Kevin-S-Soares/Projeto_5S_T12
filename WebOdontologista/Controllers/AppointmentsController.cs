using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Exceptions;
using WebOdontologista.Models.ViewModels;
using WebOdontologista.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebOdontologista.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly CookieOptions _cookieOptions;
        private readonly TimeZoneInfo _timeZoneInfo;
        private DateTime _now
        {
            get
            {
                return _appointmentService.Now;
            }
        }
        public AppointmentsController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            _cookieOptions = new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = _now.AddDays(30)
            };
            _timeZoneInfo = _appointmentService.TimeZone;
        }
        public async Task<IActionResult> Index(int? show)
        {
            IActionResult result;
            IndexAppointmentFormViewModel viewModel = new IndexAppointmentFormViewModel();
            if (show.HasValue)
            {
                viewModel.Appointments = await ShowType(show.Value);
                if (show.Value > 0 && show.Value < 5)
                {
                    viewModel.Show = show.Value;
                    Response.Cookies.Append("Show", show.Value.ToString(), _cookieOptions);
                    result = View(viewModel);
                }
                else
                {
                    result = Redirect("?show=4");
                }
            }
            else
            {
                if (Request.Cookies.ContainsKey("Show"))
                {
                    int value = int.Parse(Request.Cookies["Show"]);
                    viewModel.Appointments = await ShowType(value);
                    viewModel.Show = value;
                }
                else
                {
                    viewModel.Appointments = await _appointmentService.FindAllAsync();
                    viewModel.Show = 4;
                }
                result = View(viewModel);
            }
            return result;
        }
        public async Task<IActionResult> Create()
        {
            if (TempData.ContainsKey("appointment"))
            {
                TempData.Remove("appointment");
            }
            return View(await _appointmentService.ViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                AppointmentFormViewModel viewModel = await _appointmentService.ViewModel();
                viewModel.Appointment = appointment;
                return View(viewModel);
            }
            else
            {
                if (appointment.DateAndTime() < _now)
                {
                    return RedirectToAction(nameof(Error), new { message = "Data inválida!" });
                }
                try
                {
                    _appointmentService.Book.AddAppointment(appointment);
                }
                catch (DomainException e)
                {
                    return RedirectToAction(nameof(Error), new { message = e.Message });
                }
                await _appointmentService.InsertAsync(appointment);
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido" });
            }
            Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
            if (appointment == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            return View(appointment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não provido!" });
            }
            Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
            if (appointment == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Consulta inexistente!" });
            }
            AppointmentFormViewModel viewModel = await _appointmentService.ViewModel();
            viewModel.Appointment = appointment;
            try
            {
                _appointmentService.Book.RemoveAppointment(appointment);
            }
            catch (DomainException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            TempData["appointment"] = appointment.Serialize();
            viewModel.AvailableTime = RemovePastTime(_appointmentService.Book.FindAvailableTime(appointment), appointment);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Ids são distintos!" });
            }
            if (appointment.DateAndTime() < _now)
            {
                return RedirectToAction(nameof(Error), new { message = "Data inválida!" });
            }
            try
            {
                Appointment oldAppointment = Appointment.Deserialize(TempData["appointment"] as string);
                _appointmentService.Book.RemoveAppointment(oldAppointment);
                _appointmentService.Book.AddAppointment(appointment);
                await _appointmentService.UpdateAsync(appointment);
            }
            catch (DomainException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Search()
        {
            return View();
        }
        public async Task<IActionResult> SimpleSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(_now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = new DateTime(_now.Year, 12, 31);
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            var result = await _appointmentService.FindByDateAsync(minDate, maxDate);
            return View(result);
        }
        public async Task<IActionResult> GroupingSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(_now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = new DateTime(_now.Year, 12, 31);
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            var result = await _appointmentService.FindByDateGroupingAsync(minDate, maxDate);
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
        [HttpGet]
        public string GetTimes(int? dentistId, DateTime? date, int? durationInMinutes)
        {
            string result;
            if (dentistId.HasValue && date.HasValue && durationInMinutes.HasValue)
            {
                Appointment appointment = new Appointment()
                {
                    DentistId = dentistId.Value,
                    Date = date.Value,
                    DurationInMinutes = durationInMinutes.Value
                };
                if (TempData.ContainsKey("appointment"))
                {
                    Appointment oldAppointment = Appointment.Deserialize(TempData["appointment"] as string);
                    TempData["appointment"] = oldAppointment.Serialize();
                    try
                    {
                        _appointmentService.Book.RemoveAppointment(oldAppointment);
                    }
                    catch (DomainException)
                    {
                        return "[]";
                    }
                }
                ICollection<TimeSpan> list = RemovePastTime(_appointmentService.Book.FindAvailableTime(appointment), appointment);

                result = JsonConvert.SerializeObject(list);
            }
            else
            {
                result = "[]";
            }
            return result;
        }
        private async Task<List<Appointment>> ShowType(int type)
        {
            List<Appointment> result;
            switch (type)
            {
                case 1:
                    result = await _appointmentService.FindDailyAsync();
                    break;
                case 2:
                    result = await _appointmentService.FindWeeklyAsync();
                    break;
                case 3:
                    result = await _appointmentService.FindMonthlyAsync();
                    break;
                case 4:
                    result = await _appointmentService.FindAllAsync();
                    break;
                default:
                    result = await _appointmentService.FindAllAsync();
                    break;
            }
            return result;
        }
        private ICollection<TimeSpan> RemovePastTime(ICollection<TimeSpan> list, Appointment appointment)
        {
            DateTime today = new DateTime(_now.Year, _now.Month, _now.Day);
            if (appointment.Date == today)
            {
                TimeSpan timeNow = _now.TimeOfDay;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list.ElementAt(i) < timeNow)
                    {
                        list.Remove(list.ElementAt(0));
                        i--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return list;
        }
    }

}

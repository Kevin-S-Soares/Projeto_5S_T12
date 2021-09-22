using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Exceptions;
using WebOdontologista.Models.ViewModels;
using WebOdontologista.Services;
using WebOdontologista.Services.Exceptions;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;


namespace WebOdontologista.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly CookieOptions _cookieOptions;
        public AppointmentsController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            _cookieOptions = new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddDays(30)
            };
        }
        [HttpGet]
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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if(TempData.ContainsKey("appointment"))
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
                if (appointment.DateAndTime() < DateTime.Now)
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
        [HttpGet]
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
            viewModel.AvailableTime = _appointmentService.Book.FindAvailableTime(appointment);
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
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = new DateTime(DateTime.Now.Year, 12, 31);
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
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = new DateTime(DateTime.Now.Year, 12, 31);
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
                List<TimeSpan> list = _appointmentService.Book.FindAvailableTime(appointment);
                DateTime now = DateTime.Now;
                DateTime today = new DateTime(now.Year, now.Month, now.Day);
                if (appointment.Date == today)
                {
                    RemovePastTime(list);
                }
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
        private static string ReturnUrl(Appointment appointment)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Create?");
            sb.Append("Patient=" + HttpUtility.UrlEncode(appointment.Patient));
            sb.Append("&TelephoneNumber=" + HttpUtility.UrlEncode(appointment.TelephoneNumber));
            sb.Append("&DentistId=" + HttpUtility.UrlEncode(appointment.DentistId.ToString()));
            sb.Append("&AppointmentType=" + HttpUtility.UrlEncode(appointment.AppointmentType));
            sb.Append("&DurationInMinutes=" + HttpUtility.UrlEncode(appointment.DurationInMinutes.ToString()));
            sb.Append("&Date=" + HttpUtility.UrlEncode(appointment.Date.ToString("yyyy-MM-dd")));
            sb.Append("&prefill=1");

            return sb.ToString();
        }
        private static void RemovePastTime(ICollection<TimeSpan> list)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            for (int i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i) < now)
                {
                    list.Remove(list.ElementAt(0));
                    i--;
                }
            }

        }
    }

}

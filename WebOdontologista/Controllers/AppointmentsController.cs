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

namespace WebOdontologista.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly DentistService _dentistService;
        private static Appointment _oldAppointment = null;
        private readonly CookieOptions _cookieOptions;
        public AppointmentsController(AppointmentService appointmentService, DentistService dentistService)
        {
            _appointmentService = appointmentService;
            _dentistService = dentistService;
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
        public async Task<IActionResult> Create(Appointment appointment, int? prefill)
        {
            IActionResult result;
            if (!ModelState.IsValid)
            {
                ViewData["error"] = 0;
                ViewData["step"] = 1;
                AppointmentFormViewModel viewModel = await _appointmentService.ViewModel();
                viewModel.Appointment = appointment;
                if (viewModel.Dentists.Count == 0)
                {
                    result = Redirect("/Dentists/Create?ReturnAppointment=true");
                }
                else
                {
                    result = View(viewModel);
                }
            }
            else
            {
                if (prefill.HasValue)
                {
                    if(prefill.Value == 1)
                    {
                        ViewData["error"] = 0;
                        ViewData["step"] = 1;
                        AppointmentFormViewModel viewModel = await _appointmentService.ViewModel();
                        viewModel.Appointment = appointment;
                        result = View(viewModel);
                    }
                    else
                    {
                        result = Redirect(ReturnUrl(appointment));
                    }
                }
                else
                {
                    AppointmentFormViewModel viewModel = await _appointmentService.ViewModel();
                    viewModel.Appointment = appointment;
                    viewModel.Appointment.Dentist = await _dentistService.FindByIdAsync(viewModel.Appointment.DentistId);
                    viewModel.AvailableTime = _appointmentService.Book.FindAvailableTime(appointment);
                    DateTime now = DateTime.Now;
                    DateTime today = new DateTime(now.Year, now.Month, now.Day);
                    if(appointment.Date == today)
                    {
                        RemovePastTime(viewModel.AvailableTime);
                    }
                    if (viewModel.AvailableTime.Count == 0 || appointment.Date < today)
                    {
                        ViewData["step"] = 1;
                        if (viewModel.AvailableTime.Count == 0)
                        {
                            ViewData["error"] = 1;
                            result = View(viewModel);
                        }
                        else
                        {
                            ViewData["error"] = 2;
                            result = View(viewModel);
                        }
                    }
                    else
                    {
                        ViewData["error"] = 0;
                        ViewData["step"] = 2;
                        result = View(viewModel);
                    }
                }
            }
            return result;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment, bool? toReturn)
        {
            IActionResult result;
            if (toReturn.HasValue && toReturn.Value)
            {
                result = Redirect(ReturnUrl(appointment));
            }
            else
            {
                if(!ModelState.IsValid)
                {
                    result = Redirect(ReturnUrl(appointment));
                }
                else
                {
                    try
                    {
                        _appointmentService.Book.AddAppointment(appointment);
                    }
                    catch (DomainException e)
                    {
                        return RedirectToAction(nameof(Error), new { message = e.Message });
                    }
                    await _appointmentService.InsertAsync(appointment);
                    result = RedirectToAction(nameof(Index));
                }
            }
            return result;
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
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido" });
            }
            Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
            if (appointment == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            ViewData["step"] = 1;
            AppointmentFormViewModel obj = await _appointmentService.ViewModel();
            obj.Appointment = new Appointment(appointment);
            _oldAppointment = appointment;
            ViewData["id"] = appointment.Id;
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment, int? step, bool? toReturn)
        {
            if (step.HasValue && step.Value == 1)
            {
                Debug.WriteLine(id);
                Debug.WriteLine(appointment.Id);
                ViewData["step"] = 2;
                ViewData["id"] = id;
                AppointmentFormViewModel formViewModel = await _appointmentService.ViewModel();
                appointment.Dentist = await _dentistService.FindByIdAsync(appointment.DentistId);
                formViewModel.Appointment = appointment;
                _appointmentService.Book.RemoveAppointment(_oldAppointment);
                formViewModel.AvailableTime = _appointmentService.Book.FindAvailableTime(appointment);
                return View(formViewModel);
            }
            if (!ModelState.IsValid)
            {
                AppointmentFormViewModel formViewModel = await _appointmentService.ViewModel();
                formViewModel.Appointment = appointment;
                return View(formViewModel);
            }
            if (toReturn.HasValue && toReturn.Value)
            {
                ViewData["step"] = 1;
                AppointmentFormViewModel formViewModel = await _appointmentService.ViewModel();
                formViewModel.Appointment = appointment;
                return View(formViewModel);
            }
            if (appointment.DateAndTime().Ticks < DateTime.Now.Ticks)
            {
                return RedirectToAction(nameof(Error), new { message = "Data inválida!" });
            }
            Debug.WriteLine(id);
            Debug.WriteLine(appointment.Id);

            if (id != appointment.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Ids são diferentes" });
            }
            try
            {
                await _appointmentService.UpdateAsync(appointment);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            /*
            await _appointmentService.RemoveAsync(id);
            appointment.Id = 0;
            await _appointmentService.InsertAsync(appointment);
            return RedirectToAction(nameof(Index));
            */
            /*
            try
            {

                await _appointmentService.RemoveAsync(id); 
                await _appointmentService.InsertAsync(appointment);
                return RedirectToAction(nameof(Index));
            }
            catch(ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            */
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
            for(int i = 0; i < list.Count; i++)
            {
                if(list.ElementAt(i) < now)
                {
                    list.Remove(list.ElementAt(0));
                    i--;
                }
            }
            
        }
    }
}

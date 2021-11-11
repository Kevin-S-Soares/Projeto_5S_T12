using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Exceptions;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Models.ViewModels;
using WebOdontologista.Services;

namespace WebOdontologista.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly IDentistService _dentistService;
        private readonly ITimeZoneService _currentTime;

        private readonly AppointmentBook _book;

        public AppointmentsController(AppointmentService appointmentService,
            IDentistService dentistService, ITimeZoneService currentTimeZoneService)
        {
            _appointmentService = appointmentService;
            _dentistService = dentistService;
            _currentTime = currentTimeZoneService;
            _book = new AppointmentBook(_appointmentService, _dentistService, currentTimeZoneService);
        }
        public async Task<IActionResult> Index(int? show)
        {
            IActionResult result;
            CookieOptions cookieOptions = new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = _currentTime.GetDate().AddDays(30)
            };
            DateTime sameDay = new DateTime(_currentTime.GetDate().Year, _currentTime.GetDate().Month, _currentTime.GetDate().Day);
            Expression<Func<Appointment, bool>>[] predicate = new Expression<Func<Appointment, bool>>[4]
            {
                    obj => obj.DateAndTime() >= _currentTime.GetDate() && obj.Date == sameDay,
                    obj => obj.DateAndTime() >= _currentTime.GetDate() && obj.Date <= sameDay.AddDays(7),
                    obj => obj.DateAndTime() >= _currentTime.GetDate() && obj.Date <= sameDay.AddDays(30),
                    obj => obj.DateAndTime() >= _currentTime.GetDate()
            };
            IndexAppointmentFormViewModel viewModel = new IndexAppointmentFormViewModel();
            if (show.HasValue)
            {
                viewModel.Appointments = await _appointmentService.FindAllAsync(predicate[show.Value]);
                if (show.Value > -1 && show.Value < 4)
                {
                    viewModel.Show = show.Value;
                    Response.Cookies.Append("Show", show.Value.ToString(), cookieOptions);
                    result = View(viewModel);
                }
                else
                {
                    result = Redirect("?show=3");
                }
            }
            else
            {
                if (Request.Cookies.ContainsKey("Show"))
                {
                    int value = int.Parse(Request.Cookies["Show"]);
                    if (value < 0 || value > 3)
                    {
                        value = 3;
                    }
                    viewModel.Appointments = await _appointmentService.FindAllAsync(predicate[value]);
                    viewModel.Show = value;
                }
                else
                {
                    viewModel.Appointments = await _appointmentService.FindAllAsync(obj => obj.DateAndTime() > _currentTime.GetDate());
                    viewModel.Show = 3;
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
            AppointmentFormViewModel viewModel = await _appointmentService.ViewModel();
            if (viewModel.Dentists.Count == 0)
            {
                return Redirect("/Dentists/Create?returnAppointment=1");
            }
            return View(viewModel);
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
                try
                {
                    await _book.AddAppointment(appointment);
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
            try
            {
                await _book.RemoveAppointment(id);
            }
            catch (DomainException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
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
                await _book.EditingAppointment(appointment.Id);
            }
            catch (DomainException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            TempData["appointment"] = appointment.Serialize();
            viewModel.AvailableTime = await _book.FindAvailableTime(appointment);
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
            if (!ModelState.IsValid)
            {
                AppointmentFormViewModel viewModel = await _appointmentService.ViewModel();
                viewModel.Appointment = appointment;
                return View(viewModel);
            }
            try
            {
                Appointment oldAppointment = Appointment.Deserialize(TempData["appointment"] as string);
                await _book.EditAppointment(oldAppointment, appointment);
                await _appointmentService.UpdateAsync(appointment);
            }
            catch (DomainException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return RedirectToAction(nameof(Index));
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
        public async Task<string> GetTimes(int? dentistId, DateTime? date, int? durationInMinutes)
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
                        await _book.EditingAppointment(oldAppointment);
                    }
                    catch (DomainException)
                    {
                        return "[]";
                    }
                }
                try
                {
                    List<TimeSpan> list = await _book.FindAvailableTime(appointment);
                    result = JsonConvert.SerializeObject(list);
                }
                catch (DomainException)
                {
                    result = "[]";
                }
            }
            else
            {
                result = "[]";
            }
            return result;
        }
    }

}

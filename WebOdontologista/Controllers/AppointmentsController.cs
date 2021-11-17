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

namespace WebOdontologista.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDentistService _dentistService;
        private readonly ITimeZoneService _timeZoneService;

        private readonly AppointmentBook _book;

        public AppointmentsController(IAppointmentService appointmentService,
            IDentistService dentistService, ITimeZoneService currentTimeZoneService)
        {
            _appointmentService = appointmentService;
            _dentistService = dentistService;
            _timeZoneService = currentTimeZoneService;
            _book = new AppointmentBook(_appointmentService, _dentistService, _timeZoneService);
        }
        public async Task<IActionResult> Index(int? show)
        {
            IActionResult result;
            CookieOptions cookieOptions = new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = _timeZoneService.GetDate().AddDays(30)
            };
            DateTime sameDay = new DateTime(_timeZoneService.GetDate().Year, _timeZoneService.GetDate().Month, _timeZoneService.GetDate().Day);
            Expression<Func<Appointment, bool>>[] expression = new Expression<Func<Appointment, bool>>[4]
            {
                    obj => obj.DateAndTime() >= _timeZoneService.GetDate() && obj.Date == sameDay,
                    obj => obj.DateAndTime() >= _timeZoneService.GetDate() && obj.Date <= sameDay.AddDays(7),
                    obj => obj.DateAndTime() >= _timeZoneService.GetDate() && obj.Date <= sameDay.AddDays(30),
                    obj => obj.DateAndTime() >= _timeZoneService.GetDate()
            };
            IndexAppointmentFormViewModel viewModel = new IndexAppointmentFormViewModel();
            if (show.HasValue)
            {
                
                if (show.Value < 0 || show.Value > 3)
                {
                    show = 3;
                }
                viewModel.Appointments = await _appointmentService.FindAllAsync(expression[show.Value]);
                viewModel.Show = show.Value;
                Response.Cookies.Append("Show", show.Value.ToString(), cookieOptions);
                result = View(viewModel);
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
                    viewModel.Appointments = await _appointmentService.FindAllAsync(expression[value]);
                    viewModel.Show = value;
                }
                else
                {
                    viewModel.Appointments = await _appointmentService.FindAllAsync(expression[3]);
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
            AppointmentFormViewModel viewModel = await AppointmentFormViewModel.CreateFormViewModel(_dentistService);
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
                AppointmentFormViewModel viewModel = await AppointmentFormViewModel.CreateFormViewModel(_dentistService);
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
                return RedirectToAction(nameof(Error), new { message = "Id não provido!" });
            }
            Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
            if (appointment == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado!" });
            }
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteById(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido!" });
            }
            Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
            if (appointment == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado!" });
            }
            try
            {
                await _book.RemoveAppointment(id.Value);
            }
            catch (DomainException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            await _appointmentService.RemoveByIdAsync(id.Value);
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
                return RedirectToAction(nameof(Error), new { Message = "Id não encontrado!" });
            }
            AppointmentFormViewModel viewModel = await AppointmentFormViewModel.CreateFormViewModel(_dentistService);
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
                AppointmentFormViewModel viewModel = await AppointmentFormViewModel.CreateFormViewModel(_dentistService);
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebOdontologista.Controllers.Exceptions;
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

        private AppointmentFormViewModel _viewModel;

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
            if (show.HasValue)
            {
                IsBetweenExpectedValue(ref show);
                Response.Cookies.Append("Show", show.Value.ToString(), GetCookieOptions());
            }
            else
            {
                if (Request.Cookies.ContainsKey("Show"))
                {
                    show = int.Parse(Request.Cookies["Show"]);
                    IsBetweenExpectedValue(ref show);
                }
                else
                {
                    show = 3;
                }
            }
            Expression<Func<Appointment, bool>>[] expression = GetExpressions();
            List<Appointment> list = await _appointmentService.FindAllAsync(expression[show.Value]);

            return View(CreateIndexViewModel(list, show.Value));
        }
        private CookieOptions GetCookieOptions()
        {
            return new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = _timeZoneService.GetDate().AddDays(30)
            };
        }
        private Expression<Func<Appointment, bool>>[] GetExpressions()
        {
            return new Expression<Func<Appointment, bool>>[]
            {
                    obj => obj.DateAndTime() >= _timeZoneService.GetDate() && obj.Date == _timeZoneService.GetDateOnly(),
                    obj => obj.DateAndTime() >= _timeZoneService.GetDate() && obj.Date <= _timeZoneService.GetDateOnly().AddDays(7),
                    obj => obj.DateAndTime() >= _timeZoneService.GetDate() && obj.Date <= _timeZoneService.GetDateOnly().AddDays(30),
                    obj => obj.DateAndTime() >= _timeZoneService.GetDate()
            };
        }
        private void IsBetweenExpectedValue(ref int? number)
        {
            if (!number.HasValue || number.Value < 0 || number.Value > 3)
            {
                number = 3;
            }
        }
        private IndexAppointmentFormViewModel CreateIndexViewModel(List<Appointment> list, int show)
        {
            return new IndexAppointmentFormViewModel()
            {
                Appointments = list,
                Show = show
            };
        }

        public async Task<IActionResult> Create()
        {
            RemoveTempData();
            await CreateAppointmentFormViewModel(null);
            return RedirectOrViewResult();
        }
        private void RemoveTempData()
        {
            if (TempData.ContainsKey("appointment"))
            {
                TempData.Remove("appointment");
            }
        }
        private IActionResult RedirectOrViewResult()
        {
            if (_viewModel.Dentists.Count == 0)
            {
                return Redirect("/Dentists/Create?returnAppointment=1");
            }
            return View(_viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            IActionResult result;
            try
            {
                ModelIsValid();
                await _book.AddAppointment(appointment);
                await _appointmentService.InsertAsync(appointment);
                result = RedirectToAction(nameof(Index));
            }
            catch (ModelStateException)
            {
                await CreateAppointmentFormViewModel(appointment);
                result = View(_viewModel);
            }
            catch (DomainException e)
            {
                result = RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return result;
        }

        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult result;
            try
            {
                IntIsNotNull(id);
                Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
                AppointmentIsNotNull(appointment);
                result = View(appointment);
            }
            catch (DomainException e)
            {
                result = RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteById(int? id)
        {
            IActionResult result;
            try
            {
                IntIsNotNull(id);
                Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
                AppointmentIsNotNull(appointment);
                await _book.RemoveAppointment(appointment);
                await _appointmentService.RemoveByIdAsync(appointment.Id);
                result = RedirectToAction(nameof(Index));
            }
            catch (DomainException e)
            {
                result = RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return result;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult result;
            try
            {
                IntIsNotNull(id);
                Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
                AppointmentIsNotNull(appointment);
                await _book.EditingAppointment(appointment);
                await CreateAppointmentFormViewModel(appointment);
                AddTempData(appointment);
                result = View(_viewModel);
            }
            catch (DomainException e)
            {
                result = RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Appointment appointment)
        {
            IActionResult result;
            try
            {
                ModelIsValid();
                Appointment oldAppointment = await _appointmentService.FindByIdAsync(appointment.Id);
                AppointmentIsNotNull(oldAppointment);
                await _book.EditAppointment(oldAppointment, appointment);
                await _appointmentService.UpdateAsync(appointment);
                result = RedirectToAction(nameof(Index));
            }
            catch (ModelStateException)
            {
                await CreateAppointmentFormViewModel(appointment);
                result = View(_viewModel);
            }
            catch (DomainException e)
            {
                result = RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return result;
        }

        [HttpGet]
        public async Task<string> GetTimes(int? dentistId, DateTime? date, int? durationInMinutes)
        {
            string result = "[]";
            if (dentistId.HasValue && date.HasValue && durationInMinutes.HasValue)
            {
                Appointment appointment = GetAppointment(dentistId, date, durationInMinutes);
                try
                {
                    if (TempData.ContainsKey("appointment"))
                    {
                        await EditingAppointment();
                    }
                    List<TimeSpan> list = await _book.FindAvailableTime(appointment);
                    result = JsonConvert.SerializeObject(list);
                }
                catch (DomainException)
                {
                    return "[]";
                }
            }
            return result;
        }

        private Appointment GetAppointment(int? dentistId, DateTime? date, int? durationInMinutes)
        {
            return new Appointment()
            {
                DentistId = dentistId.Value,
                Date = date.Value,
                DurationInMinutes = durationInMinutes.Value
            };
        }

        private async Task EditingAppointment()
        {
            Appointment appointment = Appointment.Deserialize(TempData["appointment"] as string);
            AppointmentIsNotNull(appointment);
            AddTempData(appointment);
            await _book.EditingAppointment(appointment);
        }

        private async Task CreateAppointmentFormViewModel(Appointment appointment)
        {
            _viewModel = await AppointmentFormViewModel.CreateFormViewModel(_dentistService);
            _viewModel.Appointment = appointment;
            if (appointment != null)
            {
                _viewModel.AvailableTime = await _book.FindAvailableTime(appointment);
            }
        }

        private void AddTempData(Appointment appointment)
        {
            TempData["appointment"] = appointment.Serialize();
        }

        private void ModelIsValid()
        {
            if (!ModelState.IsValid)
            {
                throw new ModelStateException("Erro no modelo!");
            }
        }

        private void IntIsNotNull(int? id)
        {
            if (id is null)
            {
                throw new DomainException("Id não provido!");
            }
        }

        private void AppointmentIsNotNull(Appointment appointment)
        {
            if (appointment is null)
            {
                throw new DomainException("Id não encontrado!");
            }
        }

        private IActionResult Error(string message)
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

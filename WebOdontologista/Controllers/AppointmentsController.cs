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

namespace WebOdontologista.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly DentistService _dentistService;
        private static Appointment _oldAppointment = null;
        private static Appointment _toUpdateAppointment = null;
        public AppointmentsController(AppointmentService appointmentService, DentistService dentistService)
        {
            _appointmentService = appointmentService;
            _dentistService = dentistService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _appointmentService.FindAllAsync());
        }
        public async Task<IActionResult> Create()
        {
            ViewData["step"] = 1;
            return View(await _appointmentService.ViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment, int? step, bool? toReturn)
        {
            if (step.HasValue && step.Value == 1)
            {
                ViewData["step"] = 2;
                AppointmentFormViewModel formViewModel = await _appointmentService.ViewModel();
                formViewModel.Appointment = appointment;
                formViewModel.Appointment.Dentist = await _dentistService.FindByIdAsync(formViewModel.Appointment.DentistId);
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
            _toUpdateAppointment = await _appointmentService.FindByIdAsync(id.Value);
            if (_toUpdateAppointment == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            ViewData["step"] = 1;
            AppointmentFormViewModel obj = await _appointmentService.ViewModel();
            obj.Appointment = _oldAppointment = new Appointment(_toUpdateAppointment);
            ViewData["id"] = _toUpdateAppointment.Id;
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment, int? step, bool? toReturn)
        {
            if (step.HasValue && step.Value == 1)
            {
                ViewData["step"] = 2;
                ViewData["id"] = id;
                _toUpdateAppointment.Patient = appointment.Patient;
                _toUpdateAppointment.TelephoneNumber = appointment.TelephoneNumber;
                _toUpdateAppointment.DentistId = appointment.DentistId;
                _toUpdateAppointment.DurationInMinutes = appointment.DurationInMinutes;
                _toUpdateAppointment.Date = appointment.Date;
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
            if (id != _toUpdateAppointment.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Ids são diferentes" });
            }

            _toUpdateAppointment.Time = appointment.Time;
            try
            {
                await _appointmentService.UpdateAsync(_toUpdateAppointment);
                return RedirectToAction(nameof(Index));
            }
            catch(ApplicationException e)
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
    }
}

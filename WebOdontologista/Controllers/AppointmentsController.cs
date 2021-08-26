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
using WebOdontologista.Services.Exceptions;

namespace WebOdontologista.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentsController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _appointmentService.FindAllAsync());
        }
        public async Task<IActionResult> Create()
        {
            return View(await _appointmentService.ViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                AppointmentFormViewModel formViewModel = await _appointmentService.ViewModel();
                formViewModel.Appointment = appointment;
                return View(formViewModel);
            }
            if(appointment.Date.Ticks < DateTime.Now.Ticks)
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
            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido"});
            }
            Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
            if(appointment == null)
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
            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido" });
            }
            Appointment appointment = await _appointmentService.FindByIdAsync(id.Value);
            if(appointment == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            AppointmentFormViewModel obj = await _appointmentService.ViewModel();
            obj.Appointment = appointment;
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                AppointmentFormViewModel formViewModel = await _appointmentService.ViewModel();
                formViewModel.Appointment = appointment;
                return View(formViewModel);
            }
            if (id != appointment.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Ids são diferentes" });
            }
            try
            {
                await _appointmentService.Update(appointment);
                return RedirectToAction(nameof(Index));
            }
            catch(ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
                
            }
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

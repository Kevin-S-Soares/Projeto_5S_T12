using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Models;
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
        public IActionResult Index()
        {
            return View(_appointmentService.FindAll());
        }
        public IActionResult Create()
        {
            return View(_appointmentService.ViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment appointment)
        {
            _appointmentService.Insert(appointment);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido"});
            }
            Appointment appointment = _appointmentService.FindById(id.Value);
            if(appointment == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            return View(appointment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _appointmentService.Remove(id);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido" });
            }
            Appointment appointment = _appointmentService.FindById(id.Value);
            if(appointment == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            AppointmentFormViewModel obj = _appointmentService.ViewModel();
            obj.Appointment = appointment;
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Appointment appointment)
        {
            if(id != appointment.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Ids são diferentes" });
            }
            try
            {
                _appointmentService.Update(appointment);
                return RedirectToAction(nameof(Index));
            }
            catch(ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
                
            }
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

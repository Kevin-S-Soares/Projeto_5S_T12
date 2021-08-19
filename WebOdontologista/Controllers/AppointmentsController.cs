using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            List<Appointment> listOfAppointments = _appointmentService.FindAll();
            return View(listOfAppointments);
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
                return NotFound();
            }
            var obj = _appointmentService.FindById(id.Value);
            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);
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
                return NotFound();
            }
            Appointment appointment = _appointmentService.FindById(id.Value);
            if(appointment == null)
            {
                return NotFound();
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
                return BadRequest();
            }
            try
            {
                _appointmentService.Update(appointment);
            }
            catch(NotFoundException)
            {
                return NotFound();
            }
            catch(DbConcurrencyException)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebOdontologista.Data;
using WebOdontologista.Models;
using WebOdontologista.Services;
using WebOdontologista.Services.Exceptions;

namespace WebOdontologista.Controllers
{
    public class DentistsController : Controller
    {
        private readonly DentistService _dentistService;

        public DentistsController(DentistService dentistService)
        {
            _dentistService = dentistService;
        }
        public IActionResult Index()
        {
            return View(_dentistService.FindAll());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Dentist dentist)
        {
            _dentistService.Insert(dentist);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido" });
            }
            Dentist dentist = _dentistService.FindById(id.Value);
            if (dentist == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            return View(dentist);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _dentistService.Remove(id);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido" });
            }
            Dentist dentist = _dentistService.FindById(id.Value);
            if (dentist == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            return View(dentist);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Dentist dentist)
        {
            if (id != dentist.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Ids são diferentes" }); ;
            }
            try
            {
                _dentistService.Update(dentist);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message});
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

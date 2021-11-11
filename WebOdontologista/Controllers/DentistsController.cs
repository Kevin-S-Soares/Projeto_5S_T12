using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace WebOdontologista.Controllers
{
    [Authorize]
    public class DentistsController : Controller
    {
        private readonly IDentistService _dentistService;

        public DentistsController(IDentistService dentistService)
        {
            _dentistService = dentistService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dentistService.FindAllAsync());
        }
        [HttpGet]
        public IActionResult Create(int? returnAppointment)
        {
            if (returnAppointment.HasValue)
            {
                ViewData["ReturnAppointment"] = 1;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dentist dentist, int? returnAppointment)
        {
            if (!ModelState.IsValid)
            {
                return View(dentist);
            }
            await _dentistService.InsertAsync(dentist);
            if (returnAppointment.HasValue && returnAppointment.Value == 1)
            {
                return Redirect("/Appointments/Create/");
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido" });
            }
            Dentist dentist = await _dentistService.FindByIdAsync(id.Value);
            if (dentist == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            return View(dentist);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _dentistService.RemoveByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido" });
            }
            Dentist dentist = await _dentistService.FindByIdAsync(id.Value);
            if (dentist == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            return View(dentist);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dentist dentist)
        {
            if (!ModelState.IsValid)
            {
                return View(dentist);
            }
            if (id != dentist.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Ids são diferentes" }); ;
            }
            try
            {
                await _dentistService.UpdateAsync(dentist);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e)
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

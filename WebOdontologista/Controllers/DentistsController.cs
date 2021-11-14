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
            ReturnToCreateAppointment(returnAppointment);
            return View();
        }
        private void ReturnToCreateAppointment(int? returnAppointment)
        {
            if (returnAppointment.HasValue)
            {
                ViewData["ReturnAppointment"] = 1;
            }
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
            return RedirectToIndexOrCreateAppointment(returnAppointment);
        }
        private IActionResult RedirectToIndexOrCreateAppointment(int? returnAppointment)
        {
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
                return RedirectToAction(nameof(Error), new { message = "Id não provido!" });
            }
            Dentist dentist = await _dentistService.FindByIdAsync(id.Value);
            if (dentist == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado!" });
            }
            return View(dentist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteById(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido!" });
            }
            Dentist dentist = await _dentistService.FindByIdAsync(id.Value);
            if (dentist == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado!" });
            }
            await _dentistService.RemoveByIdAsync(id.Value);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não provido!" });
            }
            Dentist dentist = await _dentistService.FindByIdAsync(id.Value);
            if (dentist == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado!" });
            }
            return View(dentist);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Dentist dentist)
        {
            if (!ModelState.IsValid)
            {
                return View(dentist);
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

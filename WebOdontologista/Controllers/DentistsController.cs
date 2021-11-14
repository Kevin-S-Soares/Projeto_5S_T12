using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Exceptions;
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
            CreateViewData(returnAppointment);
            return View();
        }
        private void CreateViewData(int? returnAppointment)
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
            IActionResult result;
            try
            {
                ModelIsValid();
                await _dentistService.InsertAsync(dentist);
                result = RedirectToIndexOrCreateAppointment(returnAppointment);
            }
            catch(DomainException)
            {
                result = View(dentist);
            }
            return result;
        }
        private IActionResult RedirectToIndexOrCreateAppointment(int? returnAppointment)
        {
            if (returnAppointment.HasValue && returnAppointment.Value == 1)
            {
                return Redirect("/Appointments/Create/");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult result;
            try
            {
                IntIsNotNull(id);
                Dentist dentist = await _dentistService.FindByIdAsync(id.Value);
                DentistIsNotNull(dentist);
                result = View(dentist);
            }
            catch(DomainException e)
            {
                result = RedirectToAction(nameof(Error), new { message = e.Message});
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
                Dentist dentist = await _dentistService.FindByIdAsync(id.Value);
                DentistIsNotNull(dentist);
                await _dentistService.RemoveByIdAsync(dentist.Id);
                result = RedirectToAction(nameof(Index));
            }
            catch (DomainException e)
            {
                result = RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult result;
            try
            {
                IntIsNotNull(id);
                Dentist dentist = await _dentistService.FindByIdAsync(id.Value);
                DentistIsNotNull(dentist);
                result = View(dentist);
            }
            catch(DomainException e)
            {
                result = RedirectToAction(nameof(Error), new { message = e.Message });
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Dentist dentist)
        {
            IActionResult result;
            try
            {
                ModelIsValid();
                await _dentistService.UpdateAsync(dentist);
                result = RedirectToAction(nameof(Index));
            }
            catch(DomainException)
            {
                result = View(dentist);
            }
            return result;
        }

        [HttpGet]
        public IActionResult Error(string message)
        {
            ErrorViewModel error = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(error);
        }

        private void ModelIsValid()
        {
            if (!ModelState.IsValid)
            {
                throw new DomainException("Modelo inválido!");
            }
        }
        private void IntIsNotNull(int? id)
        {
            if (!id.HasValue)
            {
                throw new DomainException("Id não provido!");
            }
        }
        private void DentistIsNotNull(Dentist dentist)
        {
            if (dentist is null)
            {
                throw new DomainException("Id não encontrado!");
            }
        }
    }
}

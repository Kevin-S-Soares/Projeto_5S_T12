using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebOdontologista.Data;
using WebOdontologista.Models;
using WebOdontologista.Services;
using WebOdontologista.Services.Exceptions;

namespace WebOdontologista.Controllers
{
    [Authorize]
    public class DentistsController : Controller
    {
        private readonly DentistService _dentistService;

        public DentistsController(DentistService dentistService)
        {
            _dentistService = dentistService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dentistService.FindAllAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dentist dentist)
        {
            if (!ModelState.IsValid)
            {
                return View(dentist);
            }
            await _dentistService.InsertAsync(dentist);
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
            await _dentistService.RemoveAsync(id);
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

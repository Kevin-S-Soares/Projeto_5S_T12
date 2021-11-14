using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;

namespace UnitTests.Controllers.DentistController_Tests
{
    [TestClass]
    public class Create_Post
    {
        public DentistsController Controller_Test { get; set; }

        private readonly DentistServiceDependecy _dentistService = new DentistServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new DentistsController(_dentistService);
        }

        [TestMethod]
        public async Task CreateNewDentist_AddedToDentistService()
        {
            Dentist dentist = GetNewDentist();
            await Controller_Test.Create(dentist, null);
            Assert.IsTrue(await _dentistService.FindByIdAsync(dentist.Id) == dentist);
        }
        private Dentist GetNewDentist()
        {
            return new Dentist()
            {
                Id = 4,
                Name = "Kevin",
                Email = "kevin@gmail.com",
                TelephoneNumber = "(11) 12345-6789"
            };
        }

        [TestMethod]
        public async Task ReturnToIndex_IsRedirectToAction()
        {
            IActionResult result = await Controller_Test.Create(GetNewDentist(), null);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task ReturnToIndex_CorrectRedirect()
        {
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Create(GetNewDentist(), null);
            Assert.AreEqual(result.ActionName, "Index");
        }

        [TestMethod]
        public async Task ReturnToCreateAppointmentView_IsRedirectResult()
        {
            IActionResult result = await Controller_Test.Create(GetNewDentist(), 1);
            Assert.IsTrue(result is RedirectResult);
        }

        [TestMethod]
        public async Task ReturnToCreateAppointmentView_CorrectRedirect()
        {
            RedirectResult result = (RedirectResult)await Controller_Test.Create(GetNewDentist(), 1);
            Assert.AreEqual(result.Url, "/Appointments/Create/");
        }

        [TestMethod]
        public async Task IncorrectDentistModel_ReturnToView()
        {
            /* 
            * https://stackoverflow.com/questions/17346866/model-state-validation-in-unit-tests
            */

            Controller_Test.ModelState.AddModelError("error", "error");
            IActionResult result = await Controller_Test.Create(null, null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task IncorrectDentistModel_ModelAreEqual()
        {
            Dentist dentist = GetWrongDentist();
            Controller_Test.ModelState.AddModelError("error", "error");
            ViewResult result = (ViewResult)await Controller_Test.Create(dentist, null);
            Assert.AreEqual(result.Model, dentist);
        }
        private Dentist GetWrongDentist()
        {
            return new Dentist()
            {
                Id = -1,
            };
        }
    }
}

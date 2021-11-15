using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;

namespace UnitTests.Controllers.DentistController_Tests
{
    [TestClass]
    public class Edit_Get
    {
        public DentistsController Controller_Test { get; set; }

        private readonly DentistServiceDependecy _dentistService = new DentistServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new DentistsController(_dentistService);
        }

        [TestMethod]
        public async Task ExistingDentist_IsView()
        {
            IActionResult result = await Controller_Test.Edit(1);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task ExistingDentist_ModelAreEqual()
        {
            Dentist dentist = await _dentistService.FindByIdAsync(1);
            ViewResult result = (ViewResult)await Controller_Test.Edit(1);
            Assert.AreEqual(dentist, result.Model);
        }

        [TestMethod]
        public async Task NonExistingDentist_IsRedirectToAction()
        {
            IActionResult result = await Controller_Test.Edit(-1);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NonExistingDentist_CorrectRedirect()
        {
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(-1);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NonExistingDentist_CorrectMessage()
        {
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(-1);
            Assert.AreEqual("Id não encontrado!", result.RouteValues["message"]);
        }

        [TestMethod]
        public async Task NullDentist_IsRedirectToAction()
        {
            int? id = null;
            IActionResult result = await Controller_Test.Edit(id);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NullDentist_CorrectRedirect()
        {
            int? id = null;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(id);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NullDentist_CorrectMessage()
        {
            int? id = null;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Edit(id);
            Assert.AreEqual("Id não provido!", result.RouteValues["message"]);
        }
    }
}

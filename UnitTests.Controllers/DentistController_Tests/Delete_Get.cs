using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;

namespace UnitTests.Controllers.DentistController_Tests
{
    [TestClass]
    public class Delete_Get
    {
        public DentistsController Controller_Test { get; set; }

        private readonly DentistServiceDependecy _dentistService = new DentistServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new DentistsController(_dentistService);
        }

        [TestMethod]
        public async Task ExistingId_IsViewResult()
        {
            int? id = 1;
            IActionResult result = await Controller_Test.Delete(id);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task ExistingId_ModelAreEqual()
        {
            int? id = 1;
            ViewResult result = (ViewResult)await Controller_Test.Delete(id);
            Dentist correctDentist = await _dentistService.FindByIdAsync(id.Value);
            Assert.AreEqual(result.Model, correctDentist);
        }

        [TestMethod]
        public async Task NullId_IsRedirectToAction()
        {
            int? id = null;
            IActionResult result = await Controller_Test.Delete(id);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NullId_ModelAreEqual()
        {
            int? id = null;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Delete(id);
            Assert.AreEqual(result.RouteValues["message"], "Id não provido!");
        }

        [TestMethod]
        public async Task NonExistingId_IsRedirectToAction()
        {
            int? id = -1;
            IActionResult result = await Controller_Test.Delete(id);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NonExistingId_ModelAreEqual()
        {
            int? id = -1;
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.Delete(id);
            Assert.AreEqual(result.RouteValues["message"], "Id não encontrado!");
        }
    }
}

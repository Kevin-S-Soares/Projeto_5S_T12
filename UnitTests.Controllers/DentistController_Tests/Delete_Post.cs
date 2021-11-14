using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;

namespace UnitTests.Controllers.DentistController_Tests
{
    [TestClass]
    public class Delete_Post
    {
        public DentistsController Controller_Test { get; set; }

        private readonly DentistServiceDependecy _dentistService = new DentistServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new DentistsController(_dentistService);
        }

        [TestMethod]
        public async Task ExistingId_RemovedInDentistService()
        {
            await Controller_Test.DeleteById(1);
            Assert.IsTrue(await _dentistService.FindByIdAsync(1) is null);
        }

        [TestMethod]
        public async Task ExistingId_IsRedirectToAction()
        {
            IActionResult result = await Controller_Test.DeleteById(1);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task ExistingId_CorrectRedirect()
        {
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(1);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task NonExistingId_IsRedirectToAction()
        {
            IActionResult result = await Controller_Test.DeleteById(-1);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NonExistingId_CorrectRedirect()
        {
            RedirectToActionResult result = (RedirectToActionResult) await Controller_Test.DeleteById(-1);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NonExistingId_CorrectMessage()
        {
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(-1);
            Assert.AreEqual("Id não encontrado!", result.RouteValues["message"]);
        }

        [TestMethod]
        public async Task NullId_IsRedirectToAction()
        {
            IActionResult result = await Controller_Test.DeleteById(null);
            Assert.IsTrue(result is RedirectToActionResult);
        }

        [TestMethod]
        public async Task NullId_CorrectRedirect()
        {
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(null);
            Assert.AreEqual("Error", result.ActionName);
        }

        [TestMethod]
        public async Task NullId_CorrectMessage()
        {
            RedirectToActionResult result = (RedirectToActionResult)await Controller_Test.DeleteById(null);
            Assert.AreEqual("Id não provido!", result.RouteValues["message"]);
        }
    }
}

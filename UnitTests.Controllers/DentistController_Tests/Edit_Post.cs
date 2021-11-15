using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;

namespace UnitTests.Controllers.DentistController_Tests
{
    [TestClass]
    public class Edit_Post
    {
        public DentistsController Controller_Test { get; set; }

        private readonly DentistServiceDependecy _dentistService = new DentistServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new DentistsController(_dentistService);
        }

        [TestMethod]
        public async Task EditingExistingDentist_EditedInDentistService()
        {
            Dentist dentist = GetNewDentist();
            Dentist oldDentist = await _dentistService.FindByIdAsync(1);

            await Controller_Test.Edit(dentist);

            Assert.AreEqual(oldDentist.Name, dentist.Name);
        }

        private Dentist GetNewDentist()
        {
            return new Dentist()
            {
                Id = 1,
                Name = "Kevin",
            };
        }

        [TestMethod]
        public async Task IncorrectDentistModel_ReturnToView()
        {
            Dentist dentist = null;
            Controller_Test.ModelState.AddModelError("error", "error");
            IActionResult result = await Controller_Test.Edit(dentist);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task IncorrectDentistModel_ModelAreEqual()
        {
            Dentist dentist = GetWrongDentist();
            Controller_Test.ModelState.AddModelError("error", "error");
            ViewResult result = (ViewResult)await Controller_Test.Edit(dentist);
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

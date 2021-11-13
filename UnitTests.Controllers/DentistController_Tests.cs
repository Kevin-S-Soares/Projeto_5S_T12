using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;

namespace UnitTests.Controllers
{
    [TestClass]
    public class DentistController_Tests
    {

        public DentistsController Controller_Test { get; set; }

        private DentistServiceDependecy _dentistService = new DentistServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new DentistsController(_dentistService);
        }
        [TestMethod]
        public async Task Index_IsView_Succeed()
        {
            IActionResult result = await Controller_Test.Index();
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task Index_ListOfDentists_IdenticalList()
        {
            ViewResult result = (ViewResult) await Controller_Test.Index();
            List<Dentist> beingTested = (List<Dentist>) result.Model;
            CollectionAssert.AreEqual(beingTested, GetAllDentists());
        }
        private List<Dentist> GetAllDentists()
        {
            return _dentistService.FindAllDentists();
        }

        [TestMethod]
        public void Create_Get_IsView_WithoutParams_Succeed()
        {
            IActionResult result = Controller_Test.Create(null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public void Create_Get_IsView_WithParams_Succeed()
        {
            IActionResult result = Controller_Test.Create(0);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task Create_Post_CreateNewDentist_Succeed()
        {
            try
            {
                await Controller_Test.Create(GetNewDentist(), null);
            }
            catch(Exception)
            {
                Assert.Fail();
            }
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
        public async Task Create_Post_IncorrectModelDentist_ReturnToView()
        {
            try
            {
                Controller_Test.ModelState.AddModelError("error", "error");
                /* 
                 * https://stackoverflow.com/questions/17346866/model-state-validation-in-unit-tests
                 */
                IActionResult result = await Controller_Test.Create(null, null);
                Assert.IsTrue(result is ViewResult);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task Create_Post_ReturnToAppointment_ReturnToView()
        {
            try
            {
                IActionResult result = await Controller_Test.Create(null, 1);
                Assert.IsTrue(result is RedirectResult);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}

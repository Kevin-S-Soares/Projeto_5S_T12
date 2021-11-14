using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;

namespace UnitTests.Controllers.DentistController_Tests
{
    [TestClass]
    public class Create_Get
    {

        public DentistsController Controller_Test { get; set; }

        private readonly DentistServiceDependecy _dentistService = new DentistServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new DentistsController(_dentistService);
        }

        [TestMethod]
        public void IsView_WithoutParams_True()
        {
            IActionResult result = Controller_Test.Create(null);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public void IsView_WithParams_True()
        {
            IActionResult result = Controller_Test.Create(0);
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public void AreParamsInViewData_True()
        {
            ViewResult result = (ViewResult)Controller_Test.Create(1);
            Assert.IsTrue((int)result.ViewData["ReturnAppointment"] == 1);
        }
    }
}

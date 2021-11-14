using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;
using WebOdontologista.Models;

namespace UnitTests.Controllers.DentistController_Tests
{
    [TestClass]
    public class Index
    {
        public DentistsController Controller_Test { get; set; }

        private readonly DentistServiceDependecy _dentistService = new DentistServiceDependecy();

        [TestInitialize]
        public void Initialize()
        {
            Controller_Test = new DentistsController(_dentistService);
        }

        [TestMethod]
        public async Task IsView_True()
        {
            IActionResult result = await Controller_Test.Index();
            Assert.IsTrue(result is ViewResult);
        }

        [TestMethod]
        public async Task ListOfDentists_IdenticalList()
        {
            ViewResult result = (ViewResult)await Controller_Test.Index();
            List<Dentist> beingTested = (List<Dentist>)result.Model;
            CollectionAssert.AreEqual(beingTested, GetAllDentists());
        }
        private List<Dentist> GetAllDentists()
        {
            return _dentistService.FindAllDentists();
        }
    }
}

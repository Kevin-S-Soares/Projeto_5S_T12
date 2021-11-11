using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Models.ServicesDependecies;
using WebOdontologista.Controllers;

namespace UnitTests.Controllers
{
    [TestClass]
    public class DentistController_Tests
    {

        public DentistsController Controller { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Controller = new DentistsController(new DentistServiceDependecy());
        }

    }
}

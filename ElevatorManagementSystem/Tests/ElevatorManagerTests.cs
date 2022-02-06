using ElevatorManagementSystem.Base.Interfaces;
using ElevatorManagementSystem.Base.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorManagementSystem.Tests
{
    [TestClass]
    public class ElevatorManagerTests
    {
        private ElevatorManager ElevatorManager;

        [TestInitialize]
        public virtual void Setup()
        {
            ElevatorManager = new ElevatorManager();
        }

        [TestMethod]
        public void AssignsRequestCorrectly()
        {
            var firstRequestElevator = ElevatorManager.ProcessRequest(new ExternalRequest(2, Base.Enums.RequestDirection.Up));

            Assert.IsTrue(firstRequestElevator.Name == "Bottom elevator");
            Assert.IsTrue(firstRequestElevator.UpRequests.Any());

            var secondRequestElevator = ElevatorManager.ProcessRequest(new InternalRequest(2, 3));

            Assert.IsTrue(secondRequestElevator.Name == "Bottom elevator");
            Assert.IsTrue(secondRequestElevator.UpRequests.Count == 2);

            var thirdRequestElevator = ElevatorManager.ProcessRequest(new ExternalRequest(8, Base.Enums.RequestDirection.Up));

            Assert.IsTrue(thirdRequestElevator.Name == "Top elevator");
            Assert.IsTrue(thirdRequestElevator.UpRequests.Count == 1);
        }
    }
}

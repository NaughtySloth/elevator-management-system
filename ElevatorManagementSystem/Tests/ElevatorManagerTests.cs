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
            var assignedElevator = ElevatorManager.ProcessRequest(new ExternalRequest(2, Base.Enums.RequestDirection.Up));

            Assert.IsTrue(assignedElevator.Name == "Bottom elevator");
            Assert.IsTrue(assignedElevator.UpRequests.Any());
        }
    }
}

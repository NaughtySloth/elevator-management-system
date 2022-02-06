using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
    }
}

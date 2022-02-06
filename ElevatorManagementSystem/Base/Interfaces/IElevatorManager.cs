using ElevatorManagementSystem.Base.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElevatorManagementSystem.Base.Interfaces
{
    public interface IElevatorManager
    {
        Elevator ProcessRequest(Request request);
        void StartOperation();
    }
}

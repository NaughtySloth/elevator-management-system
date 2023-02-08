using ElevatorManagementSystem.Base.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorManagementSystem.Base.Interfaces
{
    public interface IBuildingElevatorsManager
    {
        Elevator ProcessRequest(Request request);
        Task StartOperation();
    }
}

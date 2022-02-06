using ElevatorManagementSystem.Base.Models;
using System;
using System.Threading.Tasks;

namespace ElevatorManagementSystem
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var elevatorManager = new ElevatorManager();

            elevatorManager.ProcessRequest(new InternalRequest(0, 10));
            elevatorManager.ProcessRequest(new ExternalRequest(2, Base.Enums.RequestDirection.Down));
            elevatorManager.ProcessRequest(new InternalRequest(2, 0));

            elevatorManager.StartOperation();
        }
    }
}

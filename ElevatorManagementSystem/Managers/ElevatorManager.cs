using ElevatorManagementSystem.Base.Enums;
using ElevatorManagementSystem.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorManagementSystem.Managers
{
    public class ElevatorManager
    {
        private readonly Elevator _elevator;

        public ElevatorManager(Elevator elevator)
        {
            _elevator = elevator;
        }

        public async Task Start()
        {
            while (_elevator.UpRequests.Any() || _elevator.DownRequests.Any())
            {
                Console.WriteLine($"{_elevator.Name} Processing requests...");

                await ProcessRequests();
            }

            Console.WriteLine($"{_elevator.Name} Processed all requests. Elevator Idle");

            _elevator.Status = ElevatorStatus.Idle;

            if (Elevator.IdleTimer == null)
            {
                _elevator.StartElevatorIdleTimer();
            }
        }

        private async Task ProcessRequests()
        {
            // a real-life elevator should first process requests from passengers that need to go up before going down
            if (_elevator.Status == ElevatorStatus.GoingUp || _elevator.Status == ElevatorStatus.Idle)
            {
                await ProcessUpRequest();
                await ProcessDownRequest();
            }
            else
            {
                await ProcessDownRequest();
                await ProcessUpRequest();
            }
        }

        private async Task ProcessUpRequest()
        {
            while (_elevator.UpRequests.Any())
            {
                Request upRequest = _elevator.UpRequests.Dequeue();

                _elevator.DestinationFloor = upRequest is InternalRequest internalRequest ? internalRequest.DestinationFloor : upRequest.OriginFloor;

                Console.WriteLine($"{_elevator.Name} going up to floor " + _elevator.DestinationFloor);

                _elevator.Status = ElevatorStatus.GoingUp;
                await Task.Delay(5000);

                Console.WriteLine($"{_elevator.Name} Processing up requests. Stopped at floor " + _elevator.CurrentFloor);
            }

            // after doing all the requests on the way up we set the elevator to do the Down Requests
            if (_elevator.DownRequests.Any())
            {
                _elevator.Status = ElevatorStatus.GoingDown;
            }
            else
            {
                // if there are no down requests we set it to idle
                _elevator.Status = ElevatorStatus.Idle;

                // we would call the idle timer here again unless it's already running
            }
        }

        private async Task ProcessDownRequest()
        {
            while (_elevator.DownRequests.Any())
            {
                Request downRequest = _elevator.DownRequests.Dequeue();

                _elevator.DestinationFloor = downRequest is InternalRequest internalRequest ? internalRequest.DestinationFloor : downRequest.OriginFloor;

                Console.WriteLine($"{_elevator.Name} going down to floor " + _elevator.DestinationFloor);

                _elevator.Status = ElevatorStatus.GoingDown;
                await Task.Delay(5000);

                Console.WriteLine($"{_elevator.Name} Processing down requests. Stopped at floor " + _elevator.CurrentFloor);
            }

            // same logic as in ProcessUpRequest just in reverse
            if (_elevator.UpRequests.Any())
            {
                _elevator.Status = ElevatorStatus.GoingUp;
            }
            else
            {
                _elevator.Status = ElevatorStatus.Idle;
                // we would call the idle timer here again unless it's already running
            }
        }
    }
}

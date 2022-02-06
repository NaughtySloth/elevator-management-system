using ElevatorManagementSystem.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ElevatorManagementSystem.Base.Models
{
    /// <summary>
    /// The Elevator class has no complex logic in it by design. All of that should be handled by the ElevatorManager.
    /// </summary>
    public class Elevator
    {
        private const int _IdleSeconds = 2;

        private static System.Timers.Timer _idleTimer;

        public Elevator(int startingFloor, string name)
        {
            CurrentFloor = startingFloor;
            Name = name;
            Status = ElevatorStatus.Idle;
        }

        public int CurrentFloor { get; set; }
        public int DestinationFloor { get; set; }
        public ElevatorStatus Status { get; set; }

        // in a real implementation these queues would be sorted, probably by using a min-heap
        // the Up Requests would be sorted from the lower floors to the higher ones and the reverse for Down Requests
        public Queue<Request> UpRequests { get; set; } = new Queue<Request>();
        public Queue<Request> DownRequests { get; set; } = new Queue<Request>();

        public string Name { get; set; }

        // this event should trigger after an elevator has been idle for too long
        // after the event is triggered, the ElevatorManager should send the elevator to one of the optimal default floors
        public EventHandler ElevatorIdleThreshold;

        public async Task Start()
        {
            while (UpRequests.Any() || DownRequests.Any())
            {
                Console.WriteLine($"{Name} Processing requests...");

                await ProcessRequests();
            }

            Console.WriteLine($"{Name} Processed all requests. Elevator Idle");

            Status = ElevatorStatus.Idle;

            if (_idleTimer == null)
            {
                ElevatorIdleTimer();
            }
        }

        private async Task ProcessRequests()
        {
            // a real-life elevator should first process requests from passengers that need to go up before going down
            if (Status == ElevatorStatus.GoingUp || Status == ElevatorStatus.Idle)
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
            while (UpRequests.Any())
            {
                Request upRequest = UpRequests.Dequeue();

                DestinationFloor = upRequest is InternalRequest internalRequest ? internalRequest.DestinationFloor : upRequest.OriginFloor;

                Console.WriteLine($"{Name} going up to floor " + DestinationFloor);

                Status = ElevatorStatus.GoingUp;
                await Task.Delay(5000);

                Console.WriteLine($"{Name} Processing up requests. Stopped at floor " + CurrentFloor);
            }

            // after doing all the requests on the way up we set the elevator to do the Down Requests
            if (DownRequests.Any())
            {
                Status = ElevatorStatus.GoingDown;
            }
            else
            {
                // if there are no down requests we set it to idle
                Status = ElevatorStatus.Idle;

                // we would call the idle timer here again unless it's already running
            }
        }

        private async Task ProcessDownRequest()
        {
            while (DownRequests.Any())
            {
                Request downRequest = DownRequests.Dequeue();

                DestinationFloor = downRequest is InternalRequest internalRequest ? internalRequest.DestinationFloor : downRequest.OriginFloor;

                Console.WriteLine($"{Name} going down to floor " + DestinationFloor);

                Status = ElevatorStatus.GoingDown;
                await Task.Delay(5000);

                Console.WriteLine($"{Name} Processing down requests. Stopped at floor " + CurrentFloor);
            }

            // same logic as in ProcessUpRequest just in reverse
            if (UpRequests.Any())
            {
                Status = ElevatorStatus.GoingUp;
            }
            else
            {
                Status = ElevatorStatus.Idle;
                // we would call the idle timer here again unless it's already running
            }
        }

        /// <summary>
        /// This is a simple mock implementation of an idle timer for an elevator
        /// The real implementation would be asynchronous and hooked up to changes to Elevator Status so that the timer is canceled
        /// once the elevator is no longer idle
        /// </summary>
        private void ElevatorIdleTimer()
        {
            _idleTimer = new System.Timers.Timer
            {
                // convert to milliseconds
                Interval = _IdleSeconds * 1000
            };

            _idleTimer.Elapsed += ElevatorTimerElapsed;

            _idleTimer.AutoReset = false;

            _idleTimer.Start();
        }

        private void ElevatorTimerElapsed(object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"{Name} Elevator has been idle for {_IdleSeconds} seconds");

            ElevatorIdleThreshold.Invoke(this, e);
        }
    }
}

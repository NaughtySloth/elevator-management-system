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
        public static System.Timers.Timer IdleTimer { get => _idleTimer; }

        // this event should trigger after an elevator has been idle for too long
        // after the event is triggered, the ElevatorManager should send the elevator to one of the optimal default floors
        public EventHandler ElevatorIdleThreshold;

        /// <summary>
        /// This is a simple mock implementation of an idle timer for an elevator
        /// The real implementation would be asynchronous and hooked up to changes to Elevator Status so that the timer is canceled
        /// once the elevator is no longer idle
        /// </summary>
        public void StartElevatorIdleTimer()
        {
            _idleTimer = new System.Timers.Timer
            {
                // convert to milliseconds
                Interval = _IdleSeconds * 1000
            };

            IdleTimer.Elapsed += ElevatorTimerElapsed;

            IdleTimer.AutoReset = false;

            IdleTimer.Start();
        }

        private void ElevatorTimerElapsed(object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"{Name} Elevator has been idle for {_IdleSeconds} seconds");

            ElevatorIdleThreshold.Invoke(this, e);
        }
    }
}

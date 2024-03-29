﻿using ElevatorManagementSystem.Base.Enums;
using ElevatorManagementSystem.Base.Exceptions;
using ElevatorManagementSystem.Base.Interfaces;
using ElevatorManagementSystem.Base.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorManagementSystem.Managers
{
    /// <summary>
    /// Class responsible for controlling all the elevators in a building, instantiating individual elevators and their managers
    /// </summary>
    public class BuildingElevatorsManager : IBuildingElevatorsManager
    {
        private const int _numberOfFloors = 10;
        private readonly (int, int) optimalIdleFloors = (0, 7);

        private readonly Elevator _topElevator;
        private readonly Elevator _bottomElevator;

        private readonly ElevatorManager _topElevatorManager;
        private readonly ElevatorManager _bottomElevatorManager;

        public BuildingElevatorsManager()
        {
            // we start off the elevators at optimal resting positions
            _topElevator = new Elevator(optimalIdleFloors.Item2, "Top elevator");
            _bottomElevator = new Elevator(optimalIdleFloors.Item1, "Bottom elevator");

            _topElevator.ElevatorIdleThreshold += HandleIdleElevatorTimer;
            _bottomElevator.ElevatorIdleThreshold += HandleIdleElevatorTimer;

            _topElevatorManager = new ElevatorManager(_topElevator);
            _bottomElevatorManager = new ElevatorManager(_bottomElevator);

        }

        /// <summary>
        /// Execute this after adding all desired requests via ProcessRequest method
        /// </summary>
        public async Task StartOperation()
        {
            Console.WriteLine("Starting Building Elevator Manager and all the Elevators");

            var topElevatorTask = Task.Run(() => _topElevatorManager.Start());
            var bottomElevatorTask = Task.Run(() => _bottomElevatorManager.Start());

            await Task.WhenAll(topElevatorTask, bottomElevatorTask);
        }

        /// <summary>
        /// This method compares the status of the elevators and passes either the destination floor if the elevator is on the move
        /// or the current floor if the elevator is idling
        /// Depending on those conditions we send different parameters to the SendIdleElevatorToOptimalFloor method
        /// </summary>
        /// <param name="request"></param>
        /// <returns> The Elevator to which the request was assigned to </returns>
        public Elevator ProcessRequest(Request request)
        {
            int topElevatorFloor = -1;
            int bottomElevatorFloor = -1;

            if (_topElevator.Status == ElevatorStatus.Idle && _bottomElevator.Status == ElevatorStatus.Idle)
            {
                // logic for when both elevators are idle

                topElevatorFloor = _topElevator.CurrentFloor;
                bottomElevatorFloor = _bottomElevator.CurrentFloor;
            }
            else if (_topElevator.Status != ElevatorStatus.Idle && _bottomElevator.Status == ElevatorStatus.Idle)
            {
                // logic for when the top elevator is moving and other is idle

                topElevatorFloor = _topElevator.DestinationFloor;
                bottomElevatorFloor = _bottomElevator.CurrentFloor;
            }
            else if (_topElevator.Status == ElevatorStatus.Idle && _bottomElevator.Status != ElevatorStatus.Idle)
            {
                // logic for when the bottom elevator is moving and other is idle

                topElevatorFloor = _topElevator.CurrentFloor;
                bottomElevatorFloor = _bottomElevator.DestinationFloor;
            }
            else if (_topElevator.Status != ElevatorStatus.Idle && _bottomElevator.Status != ElevatorStatus.Idle)
            {
                // logic for when both elevators are moving

                topElevatorFloor = _topElevator.DestinationFloor;
                bottomElevatorFloor = _bottomElevator.DestinationFloor;
            }

            return AssignRequestToOptimalElevator(topElevatorFloor, bottomElevatorFloor, request);
        }

        private Elevator AssignRequestToOptimalElevator(int topElevatorFloor, int bottomElevatorFloor, Request request)
        {
            var requestedFloor = request is InternalRequest internalRequest ? internalRequest.DestinationFloor : request.OriginFloor;

            if (requestedFloor > _numberOfFloors)
            {
                throw new InvalidRequestException("Building isn't tall enough for this request!!!");
            }

            var elevatorCloserToRequestedFloor = GetCloserElevator(topElevatorFloor, bottomElevatorFloor, requestedFloor);

            AssignRequest(elevatorCloserToRequestedFloor, request);

            // I would add here the logic to optimize the location of the resting elevator to ensure minimum wait time for any future passengers
            // to do this I would call the SendIdleElevatorToOptimalFloor with the idle elevator (if any is idle now)

            return elevatorCloserToRequestedFloor;
        }

        private Elevator GetCloserElevator(int topElevatorFloor, int bottomElevatorFloor, int requestedFloor)
        {
            // if the top elevator is further away from the requested floor
            if (Math.Abs(topElevatorFloor - requestedFloor) > Math.Abs(bottomElevatorFloor - requestedFloor))
            {
                return _bottomElevator;
            }
            // if the top elevator is closer to the requested floor or if they are equally apart from the requested floor
            else if (Math.Abs(topElevatorFloor - requestedFloor) <= Math.Abs(bottomElevatorFloor - requestedFloor))
            {
                // Note: we assign to the top one if they are equally apart because in real life it is more energy efficient to go down than up
                return _topElevator;
            }
            else
            {
                throw new Exception("Error occurred while assigning request to elevator");
            }
        }

        private void HandleIdleElevatorTimer(object sender, EventArgs e)
        {
            Console.WriteLine("Handling Idle Elevator after timer threshold reached.");

            SendIdleElevatorToOptimalFloor(sender as Elevator);
        }

        private void SendIdleElevatorToOptimalFloor(Elevator idleElevator)
        {
            // here I would put logic which would assign an elevator to one of the default floors to rest in order to ensure minimal wait time
            // to do this I would take the current floor of the idle elevator and the destination floor of the moving elevator
            // with these two floors I could calculate which of the two default floors (0,7) is better to rest at for the idle elevator

            // for the time being we will just send it to rest at ground floor

            Console.WriteLine("Sending idle elevator to ground floor.");

            AssignRequest(idleElevator, new InternalRequest(idleElevator.CurrentFloor, 0));
        }

        private void AssignRequest(Elevator elevator, Request request)
        {
            if (request.Direction == RequestDirection.Up)
            {
                elevator.UpRequests.Enqueue(request);
            }
            else
            {
                elevator.DownRequests.Enqueue(request);
            }
        }
    }
}

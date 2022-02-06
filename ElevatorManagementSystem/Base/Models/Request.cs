using ElevatorManagementSystem.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElevatorManagementSystem.Base.Models
{
    public abstract class Request
    {
        public int OriginFloor { get; set; }
        public RequestDirection Direction { get; set; }
    }
}

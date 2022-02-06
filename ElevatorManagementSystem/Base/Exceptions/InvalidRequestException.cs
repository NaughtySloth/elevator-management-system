using System;
using System.Collections.Generic;
using System.Text;

namespace ElevatorManagementSystem.Base.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message)
        {
        }
    }
}

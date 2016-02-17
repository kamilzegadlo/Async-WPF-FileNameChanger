using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNameChanger.Service
{
    public class InvalidInputValueException : Exception
    {
        public InvalidInputValueException(string message) : base(message) { }
    }
}

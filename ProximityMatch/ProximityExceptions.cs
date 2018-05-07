using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProximityMatch.Exceptions
{
    public class DimensionExceptions : System.Exception
    {
        public DimensionExceptions(string message)
            : base(message: message)
        {
        }
    }
    public class UniqueIdExceptions : System.Exception
    {
        public UniqueIdExceptions(string message)
            : base(message: message)
        {
        }
    }


}

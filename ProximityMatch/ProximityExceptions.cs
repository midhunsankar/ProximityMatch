using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctagonSquare.ProximityMatch.Exceptions
{
    public class DimensionException : System.Exception
    {
        public DimensionException(string message)
            : base(message: message)
        {
        }
    }

    public class UniqueIdException : System.Exception
    {
        public UniqueIdException(string message)
            : base(message: message)
        {
        }
    }

    public class CoordinateException : System.Exception
    {
        public CoordinateException(string message)
            : base(message: message)
        {
        }
    }



}

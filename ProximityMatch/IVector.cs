using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProximityMatch
{
    public interface IVector
    {
        double[] coordinate { get; set; }
        double _distance { get; set; }
    }
}

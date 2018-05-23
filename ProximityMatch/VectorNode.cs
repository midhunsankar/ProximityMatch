/*
 * Developer    : Midhun Sankar
 * Date         : 02/05/2018
 * Description  : This project is a demo to plot entities in a three dimentional vector space,
 *                and find vector points laying nearest to a point of reference. 
 * License      : GNU General Public License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctagonSquare.ProximityMatch
{
    public sealed class VectorNode
    {
        public readonly long UniqueId;
        public readonly double[] Angles;
        public readonly double DistanceFromOrgin;

        public VectorNode(long uniqueid, double[] angles, double distance)
        {
            Angles = angles;
            DistanceFromOrgin = distance;
            UniqueId = uniqueid;
        }
    }
}

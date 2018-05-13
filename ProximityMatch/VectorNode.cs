﻿/*
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

namespace ProximityMatch
{
    public sealed class VectorNode
    {
        public readonly long _uniqueID;
        public readonly double[] _anglePlain;
        public readonly double _distanceOrgin;

        public VectorNode(long uniqueid, double[] angle, double distance)
        {
            _anglePlain = angle;
            _distanceOrgin = distance;
            _uniqueID = uniqueid;
        }
    }
}

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
using System.Threading.Tasks;

namespace OctagonSquare.ProximityMatch
{
    public interface IVector
    {
        long UniqueId { get; set; }
        double?[] Coordinate { get; set; }
        double Distance { get; set; }
    }
}

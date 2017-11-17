/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parsers
{
    public class RailRoute
    {
        public RailRoute(int id, String des)
        {
            routeId = id;
            routeDes = des;
        }

        public RailRoute(string agency, List<string> record)
        {
            routeId = Int32.Parse(record[0]);
            routeDes = record[1];
            Agency = agency;
        }

        public bool AllFieldsEqual(RailRoute second)
        {
            bool result = false;
            result = routeId == second.routeId && routeDes == second.routeDes;
            return result;
        }        
        
        public int routeId { get; set; }
        public String routeDes { get; set; }
        public String Agency { get; set; }



    }
}

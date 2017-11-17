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
    public class BusRoute
    {
        public BusRoute(int id,String agency)
        {
            routeId = id;
            Agency = agency;
        }

        public BusRoute(int id, String des, String zs, String agency)
        {
            routeId = id;
            routeDes = des;
            String[] temp = zs.Split(',');
            zones = new int[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                try
                {
                    zones[i] = Convert.ToInt16(temp[i]);
                }
                catch
                {
                    zones[i] = -1;
                }
            }
            Agency = agency;
        }

        public BusRoute(string agency, List<string> record)
        {
            routeId = Int32.Parse(record[0]);
            routeDes = record[1];
            String[] temp = record[2].Split(',');
            zones = new int[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                try
                {
                    zones[i] = Convert.ToInt16(temp[i]);
                }
                catch (Exception)
                {
                    zones[i] = -1;
                }
                
            }
            Agency = agency;
        }

        public int routeId { get; set; }
        public String routeDes { get; set; }
        public int[] zones { get; set; }
        public String Agency { get; set; }

        public bool AllFieldsEqual(BusRoute second)
        {
            bool result = false;
            return result = routeId == second.routeId && Agency==second.Agency&& /* routeDes == second.routeDes &&*/ zones.Length == second.zones.Length;
           
        }

    }
}

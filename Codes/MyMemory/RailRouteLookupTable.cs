/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

/**
 * Updated by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Purpose: Add the Azure Lookup Table
 * Date: 06/09/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;

using OutputTypes;
using Parsers;
using Utils;

namespace MyMemory
{
    public abstract class LookupRailGPS : LookupTable<RailGPSOutputElement>
    {
        private static BootUp memory = BootUp.GetInstance();
        public override Object GetRecord(RailGPSOutputElement ev, string agency)
        {
            //cannot find the one in inventory
            RailRoute route = new RailRoute(0,"des"); //= memory.GetRailRouteInfo(ev.routeId);

            //if (route == null)
            //    return null;
            //cannot find it...

            var values = GetFields(route, ev);

            return values;
        }

        protected abstract Object GetFields(RailRoute br, RailGPSOutputElement ev);
    }


    public class LookupRailGPSDB : LookupRailGPS
    {
        protected override Object GetFields(RailRoute br, RailGPSOutputElement ev)
        {
            return new List<Object>
                                 {
                                     BootUp.maxRailConfigID, //ev.ConfigId,
                                    ev.StartTime.LocalDateTime.ToString("yyyyMMdd HH:mm:ss"),
                                    ev.trainId,
                                    ev.lineId,
                                    ev.routeId,
                                    ev.routeDes,
                                    ev.destination,
                                    ev.offRoute,
                                    ev.direction,
                                    ev.Latitude,
                                    ev.Longitude,
                                    ev.Location_time,
                                    ev.scheduled_dev,
                                    ev.arrival_nextTP,
                                    ev.next_location,
                                    ev.timepoint//,
                                    //br.Agency
                                 };
        }
    }

    public class LookUpRailGPSAzureTable : LookupRailGPS
    {
        protected override object GetFields(RailRoute br, RailGPSOutputElement ev)
        {
            return new RailGPSEntityTuple(br, ev);

        }
    }



}

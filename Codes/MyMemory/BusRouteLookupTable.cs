/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

/**
 * Updated by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Purpose: Add the Azure Lookup Table
 * Date: 06/08/2011
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
    public abstract class LookupBusGPS : LookupTable<BusGPSOutputElement>
    {
        private static BootUp memory = BootUp.GetInstance();
        public override Object GetRecord(BusGPSOutputElement ev, string agency)
        {
            BusRoute route = new BusRoute(ev.routeId, agency);
            //BusRoute route = memory.GetBusRouteInfo(ev.routeId);
            //if (route == null)
            //    return null;

            var values = GetFields(route, ev);

            return values;
        }

        protected abstract Object GetFields(BusRoute br, BusGPSOutputElement ev);
    }


    public class LookupBusGPSDB : LookupBusGPS
    {
        protected override Object GetFields(BusRoute br, BusGPSOutputElement ev)
        {
            return new List<Object>
                                 {
                                     BootUp.maxBusConfigID,//ev.ConfigId,
                                    ev.StartTime.LocalDateTime.ToString("yyyyMMdd HH:mm:ss"),
                                    ev.busId,
                                    ev.lineId,
                                    ev.runId,
                                    ev.routeId,
                                    ev.routeDes,
                                    ev.direction,
                                    ev.Latitude,
                                    ev.Longitude,
                                    ev.Location_time,
                                    ev.scheduled_dev,
                                    ev.arrival_nextTP,
                                    ev.next_location,
                                    ev.timepoint,
                                    ev.brtFlag
                                    //br.Agency
                                 };
        }
    }

    public class LookUpBusGPSAzureTable : LookupBusGPS
    {
        protected override object GetFields(BusRoute br, BusGPSOutputElement ev)
        {
            return new BusGPSEntityTuple(br, ev);

        }
    }


}

/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventTypes
{
    public class TrafficBusGPSReading
    {
         public TrafficBusGPSReading()
        {
        }
        /*  For each item
            •	Bus ID (Train ID)
            •	Line ID
            •	Run ID
            *   Route ID
            *   Route Description
            •	Bus Direction
            •	Bus Location - Lat/Long (2 attributes)
            *   Bus Location Time
            •	Schedule Deviation
            •	Arrival Time at Next Time Point
            •	Next Time Point Location (Street/Cross Street)
            •	Timepoint time
         */

        public TrafficBusGPSReading(int bID, int lID, int rID, int routeID, String rD, int dir, 
            String lat, String lon, String loc_time, int dev, int nTime, String nLocation, int tp, String flag)
        {
            busId = bID;
            lineId = lID;
            runId = rID;
            routeId = routeID;
            routeDes = rD;
            direction = dir;
            Longitude = lon;
            Latitude = lat;
            Location_time = loc_time;
            scheduled_dev = dev;
            arrival_nextTP = nTime;
            next_location = nLocation;
            timepoint = tp;
            brtFlag = flag;
        }

        public int busId { get; set; }
        public int lineId { get; set; }
        public int runId { get; set; }
        public int routeId { get; set; }
        public String routeDes { get; set; }
        public int direction { get; set; }
        public String Longitude { get; set; }
        public String Latitude { get; set; }
        public String Location_time { get; set; }
        public int scheduled_dev { get; set; }
        public int arrival_nextTP { get; set; }
        public String next_location { get; set; }
        public int timepoint { get; set; }
        public String brtFlag { get; set; }
    }

    
}

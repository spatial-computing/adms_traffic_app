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
    public class TrafficRailGPSReading
    {
        public TrafficRailGPSReading()
        { }

        /*  For each item
        •	Train ID
        •	Line ID
        *   Route ID
        *   Route Description
        *   Destination
        *   Off Route
        *   direction
        •	Train Location - dir/Lat/Long (3 attributes)
        *   Train Location Time
        •	Schedule Deviation
        •	Arrival Time at Next Time Point
        •	Next Time Point Location (Street/Cross Street)
        •	Timepoint time
     */

        public TrafficRailGPSReading(int tID, String lID, String routeID, String rD, String desti, String isOff, int dir, 
            String lat, String lon, String loc_time, int dev, int nTime, String nLocation, int tp)
        {
            trainId = tID;
            lineId = lID;
            routeId = routeID;
            routeDes = rD;
            destination = desti;
            offRoute = isOff;
            direction = dir;
            Latitude = lat;
            Longitude = lon;
            Location_time = loc_time;
            scheduled_dev = dev;
            arrival_nextTP = nTime;
            next_location = nLocation;
            timepoint = tp;
        }

        public int trainId { get; set; }
        public String lineId { get; set; }
        public String routeId { get; set; }
        public String routeDes { get; set; }
        public String destination { get; set; }
        public String offRoute { get; set; }
        public int direction { get; set; }
        public String Longitude { get; set; }
        public String Latitude { get; set; }
        public String Location_time { get; set; }
        public int scheduled_dev { get; set; }
        public int arrival_nextTP { get; set; }
        public String next_location { get; set; }
        public int timepoint { get; set; }
    }
}

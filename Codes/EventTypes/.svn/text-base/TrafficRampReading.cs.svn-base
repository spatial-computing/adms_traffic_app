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
    public class TrafficRampReading
    {
        public TrafficRampReading()
        {
        }
        /*  For each item
            •	Ramp ID 
            •	MS ID
            •	Device Status
            *   Meter Status
            *   Ramp-meter control type
            •	Metering Rate
            •	occupancy
            *   speed
            •	volume
            •	links (could contain multiple link)
                •	link (id, detectorType, occupancy, speed, volume, LinkDatastatus)    
         */

        public TrafficRampReading(int rID, int mID, int ds, int ms, int rmct, int mr,
             int o, int s,  int v, string ids, string types, string os, string ss, string vs, string statuses)
        {
            rampId = rID;
            MSId = mID;
            device_status = ds;
            meter_status = ms;
            ramp_meter_control_type = rmct;
            meter_rate = mr;
            Occupancy = o;
            Speed = s;
            Volume = v;
            link_ids = ids;
            dector_types = types;
            occupancies = os;
            speeds = ss;
            volumes = vs;
            link_statuses = statuses;
            //int length = link_group.Length;

            //for (int i = 0; i + 5 < length; i += 6)
            //{
            //    RampLinkReading a = new RampLinkReading(link_group[i], link_group[i + 1], link_group[i + 2],
            //        link_group[i + 3], link_group[i + 4], link_group[i + 5]);
            //    links.Add(a);
            //}

        }

        public int rampId { get; set; }
        public int MSId { get; set; }
        public int device_status { get; set; }
        public int meter_status { get; set; }
        public int ramp_meter_control_type { get; set; }
        public int meter_rate { get; set; }
        public int Occupancy { get; set; }
        public int Speed { get; set; }
        public int Volume { get; set; }
        public string link_ids { get; set; }
        public string dector_types { get; set; }
        public string occupancies { get; set; }
        public string speeds { get; set; }
        public string volumes { get; set; }
        public string link_statuses { get; set; }
    }


    //StreamInsight does not support complicated data types
    public class RampLinkReading
    {
        public RampLinkReading()
        {
        }

        public RampLinkReading(int a, int b, int c, int d, int e, int f)
        {
            LinkDataStatus = a;
            detectorType = b;
            Occupancy = c;
            LinkId = d;
            Speed = e;
            Volume = f;
        }

        public int LinkId { get; set; }
        public int detectorType { get; set; }
        public int Occupancy { get; set; }
        public int Speed { get; set; }
        public int Volume { get; set; }
        public int LinkDataStatus { get; set; }
    }
}

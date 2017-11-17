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
    public class TrafficCmsReading
    {
        public TrafficCmsReading()
        {
        }
        /*  For each item
            •	Id (Id)
         */

        public TrafficCmsReading(int id, String deviceStatus, String state, String date, String time, String phase1Line1, String phase1Line2,
            String phase1Line3, String phase2Line1, String phase2Line2, String phase2Line3)
        {
            this.Id = id;
            this.DeviceStatus = deviceStatus;
            this.State = state;
            this.Date = date;
            this.Time = time;
            this.Phase1Line1 = phase1Line1;
            this.Phase1Line2 = phase1Line2;
            this.Phase1Line3 = phase1Line3;
            this.Phase2Line1 = phase2Line1;
            this.Phase2Line2 = phase2Line2;
            this.Phase2Line3 = phase2Line3;

        }

        public int Id { get; set; }
        public String DeviceStatus { get; set; }
        public String State { get; set; }
        public String Date { get; set; }
        public String Time { get; set; }
        public String Phase1Line1 { get; set; }
        public String Phase1Line2 { get; set; }
        public String Phase1Line3 { get; set; }
        public String Phase2Line1 { get; set; }
        public String Phase2Line2 { get; set; }
        public String Phase2Line3 { get; set; }

       
    }


}

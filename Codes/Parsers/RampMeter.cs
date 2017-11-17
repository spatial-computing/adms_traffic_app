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
    public class RampMeter
    {
        public RampMeter(int id, int msid, int type, String OST, int direction, 
            String FST, String TST, double lon, double lat, String city, double postmile, string agency)
        {
            ID = id;   
            msID = msid;
            rampType = type;
           
            OnStreet = OST;
            FromStreet = FST;
            ToStreet = TST;
            Direction = direction;
            latitude = lat;
            longitude = lon;
            City = city;
            PostMile = postmile;
            Agency = agency;
        }

        public RampMeter(string agency, List<string> record)
        {
            ID = Int32.Parse(record[0]);
            msID = Int32.Parse(record[1]);
            rampType = Int32.Parse(record[2]);
            OnStreet = record[3];
            FromStreet = record[4];
            ToStreet = record[5];
            int decimalPoints = 6;

            latitude = Double.Parse(record[6])/(Math.Pow(10, decimalPoints));
            longitude = Double.Parse(record[7])/(Math.Pow(10, decimalPoints));
            Direction = Int32.Parse(record[8]);
            
            City = record[9];
            PostMile = Double.Parse(record[10]);
            Agency = agency;
        }

        public bool AllFieldsEqual(RampMeter second)
        {
            bool result = false;

            result = ID == second.ID //&& OnStreet == second.OnStreet && FromStreet == second.FromStreet && ToStreet == second.ToStreet
                 && Agency == second.Agency && Direction == second.Direction && latitude == second.latitude &&
                longitude == second.longitude;//&& City == second.City;
            
            return result;
        }

        
        public int ID { get; set; }
        public int msID{ get; set; }
        public int rampType{ get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public String OnStreet { get; set; }
        public String FromStreet { get; set; }
        public String ToStreet { get; set; }
        public int Direction { get; set; }
        public String City { get; set; }
        public double PostMile { get; set; }
        public String Agency { get; set; }
    }
}

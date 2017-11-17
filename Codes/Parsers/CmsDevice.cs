using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;

namespace Parsers
{
    public class CmsDevice
    {
        public CmsDevice(string agency)
        {
            Agency = agency;
        }

        public CmsDevice(string agency, List<string> record)
        {
            Agency = agency;
            ID = Int32.Parse(record[0]);
            OnStreet = record[1];
            FromStreet = record[2];
            ToStreet = record[3];
            int decimalPoints = 6;

            double lat = Double.Parse(record[4])/(Math.Pow(10, decimalPoints));
            double lng = Double.Parse(record[5])/(Math.Pow(10, decimalPoints));
            Direction = Int32.Parse(record[6]);

            GeogLocation = SqlGeography.Point(lat, lng, SRID);
            City = record[7];
            PostMile = Double.Parse(record[8]);
            
        }

        public bool AllFieldsEqual(CmsDevice second)
        {
            bool result = false;

            result = ID == second.ID && Agency == second.Agency && Direction == second.Direction && //OnStreet == second.OnStreet && FromStreet == second.FromStreet && ToStreet == second.ToStreet && 
                (bool)(GeogLocation.Lat == second.GeogLocation.Lat) && (bool)(GeogLocation.Long == second.GeogLocation.Long);// &&
                //PostMile == second.PostMile; //&& this.City == second.City &&//this.AffectedLanes.Equals(second.AffectedLanes) //todo: affectedlanes should be tested.
            return result;
        }

        protected const int SRID = 4326;
        public int ID { get; set; }
        public SqlGeography GeogLocation { get; set; }
        public String OnStreet { get; set; }
        public int Direction { get; set; }
        public String FromStreet { get; set; }
        public String ToStreet { get; set; }
        public double PostMile { get; set; }
        public String City { get; set; }
        
       
        public String Agency { get; set; }
    }
    
  
}

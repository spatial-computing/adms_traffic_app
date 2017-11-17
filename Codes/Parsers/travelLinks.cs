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
    public class travelLinks
    {
        public travelLinks(int id, int rid, String direction, String type, int id1, int id2, double leng, 
            String s1, double lat1, double lon1, String s2, double lat2, double lon2, string agency)
        {
            ID = id;
            route = rid;
            Direction = direction;
            linkType = type;
            beginID = id1;
            endID = id2;
            length = leng;
            beginStreet = s1;
            beginLat = lat1;
            beginLon = lon1;
            endStreet = s2;
            endLat = lat2;
            endLon = lon2;
            Agency = agency;
        }

        public travelLinks(string agency, List<string> record)
        {
            ID = Int32.Parse(record[0]);
            route = Int32.Parse(record[1]);
            Direction = record[2];
            linkType = record[3];
            beginID = Int32.Parse(record[4]);
            endID = Int32.Parse(record[5]);
            length = Double.Parse(record[6]);
            int decimalPoints = 6;

            beginStreet = record[7];               
            beginLat = Double.Parse(record[8])/(Math.Pow(10, decimalPoints));
            beginLon = Double.Parse(record[9])/(Math.Pow(10, decimalPoints));

            endStreet = record[10];
            endLat = Double.Parse(record[11]) / (Math.Pow(10, decimalPoints));
            endLon = Double.Parse(record[12]) / (Math.Pow(10, decimalPoints));
            Agency = agency;

        }

        public bool AllFieldsEqual(travelLinks second)
        {
            bool result = false;

            result = ID == second.ID && route == second.route && linkType == second.linkType
                && beginID == second.beginID && Direction == second.Direction && endID == second.endID &&
                beginLat==second.beginLat && beginLon == second.beginLon && endLat == second.endLat &&
                endLon == second.endLon;// && beginStreet == second.beginStreet && endStreet == second.endStreet;
            
            return result;
        }

        
        public int ID { get; set; }
        public int route{ get; set; }
        public String Direction { get; set; }
        public String linkType { get; set; }
        public int beginID{ get; set; }
        public int endID{ get; set; }
        public double length { get; set; }

        public String beginStreet { get; set; }
        public double beginLat{ get; set; }
        public double beginLon { get; set; }

        public String endStreet { get; set; }     
        public double endLat{ get; set; }
        public double endLon { get; set; }
        public String Agency { get; set; }
        

    }
}

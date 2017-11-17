/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

// Comment by Jalal: This file can be deleted. Instead, proper GetField functions need to be implemented in the LookUpTable classes.
using System.Collections.Generic;

namespace DBServices
{
    public class FieldOrders
    {
        public static List<object> GetFreewayOutputSQLFieldOrders()
        {

           // return new List<Object> { "sensorID", "speed", "lat", "lng", "readDate", "direction", "onStreet", "fromStreet" };
          //  return new List<Object> { "configID", "sensorID", "speed", "readDate", "hovspeed", "ocupancy", "volume", "link_Status" };
            return new List<object> { "configID", "agency", "readDate", "link_id", "ocupancy", "speed", "volume", "hovspeed", "faulty_indicator" };
        }

        public static List<object> GetFreewayOutputXMLFieldOrders()
        {
            return new List<object> { "<sp>", "<lat>", "<lon>", "<ost>", "<dir>", "<fst>", "<hovsp>", "<ocu>", "<vol>", "<utm>" };
        }

        public static List<object> GetArterialOutputSQLFieldOrders()
        {

            // return new List<Object> { "sensorID", "speed", "lat", "lng", "readDate", "direction", "onStreet", "fromStreet" };
            //return new List<Object> { "configID", "sensorID", "speed", "readDate", "hovspeed", "ocupancy", "volume", "link_Status" };
            return new List<object> { "configID", "agency", "readDate", "link_id", "ocupancy", "speed", "volume", "hovspeed", "faulty_indicator" };
        }

        public static List<object> GetArterialConfigOutputSQLFieldOrders()
        {
            return new List<object> { "config_ID", "agency", "date_and_time", "link_id", "link_type","onstreet", "fromstreet","tostreet","start_lat_long","direction","postmile"};
        }

        public static List<object> GetFreewayConfigOutputSQLFieldOrders()
        {
            return new List<object> { "config_ID", "agency", "date_and_time", "link_id", "link_type", "onstreet", "fromstreet", "tostreet", "start_lat_long", "direction", "postmile" };
        }

        public static List<object> GetArterialOutputXMLFieldOrders()
        {
            return new List<object> { "<sp>", "<lat>", "<lon>", "<ost>", "<dir>", "<fst>", "<hovsp>", "<ocu>", "<vol>", "<utm>" };
        }
    }
}

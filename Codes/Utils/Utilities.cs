/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

//using Oracle.DataAccess.Client;

//using Oracle.DataAccess.Client;
using System;

namespace Utils
{
    public class Utilities
    {
        public static double KMH2MPH(double kmh)
        {
/*  commented by keivan 
           if (kmh > 0)
                return kmh / 1.609344;
            else
                return kmh; */
            if (kmh > 0)
                return Math.Round( kmh/1.609344, 0);
            else
                return Math.Round(kmh, 0);


        }

        /*         private void LoadLatLongs(String DBAddress) //TODO: make sure this function works
          {
              try
              {
                  //instance new oracle connection
                  var conn = new OracleConnection(DBAddress);
                  //open the connection
                  conn.Open();
                  String query =
                      "(select  g.link_id, t.X, t.Y, g.onstreet, g.direction, g.fromstreet  from (select start_lat_long as M, Onstreet, link_id , direction, fromstreet  from nbc.CONGESTION_CONFIG where config_id=(select MAX(config_id) from nbc.CONGESTION_CONFIG) )g, table(sdo_util.getvertices(g.M))t)";
                  //String query = "select g.link_id, t.X, t.Y from (select start_lat_long as M, link_id as link_id, direction as direction from CONGESTION_CONFIG where config_id=(select MAX(config_id) from CONGESTION_CONFIG) )g, table(sdo_util.getvertices(g.M))t";
                  //String query = "select g.link_id, t.X, t.Y from (select start_lat_long as M, link_id as link_id, direction as direction from MAINSTREET_CONGESTION_CONFIG where config_id=(select MAX(config_id) from MAINSTREET_CONGESTION_CONFIG) )g, table(sdo_util.getvertices(g.M))t";
                  var cmd = new OracleCommand(query);

                  cmd.Connection = conn;
                  cmd.CommandType = CommandType.Text;
                  OracleDataReader reader = cmd.ExecuteReader();

                  while (reader.Read())
                  {
                      /*Console.WriteLine(Convert.ToString(reader[0]));
                      Console.WriteLine(Convert.ToString(reader[1]));
                      Console.WriteLine(Convert.ToString(reader[2]));♥1♥
                      int key = Convert.ToInt32(reader[0]);
                      double lat = Convert.ToDouble(reader[1]);
                      double lon = Convert.ToDouble(reader[2]);
                      String ost = Convert.ToString(reader[3]);
                      int direction = Convert.ToInt32(reader[4]);
                      String fst = Convert.ToString(reader[5]);

                      linkLocDic[key] = new SensorInfo(lat, lon, ost, direction, fst);
                  }
              }
              catch (Exception e)
              {
                  throw e;
              }
          }
*/
    }
}
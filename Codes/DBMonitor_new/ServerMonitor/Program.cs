using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;

namespace ServerMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            string tag = "\'";
            string[] queryString = {
                                    "select max(date_and_time) as max_time from highway.highway_congestion_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D7 + tag,
                                    "select max(date_and_time) as max_time from highway.highway_congestion_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D8 + tag,
                                    "select max(date_and_time) as max_time from highway.highway_congestion_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D12 + tag,

                                    "select max(date_and_time) as max_time from arterial.arterial_congestion_data where agency = " + tag + RIITSArterialAgency.LADOT + tag,
                                    "select max(date_and_time) as max_time from arterial.arterial_congestion_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D7 + tag,
                                    //"select max(date_and_time) as max_time from arterial.arterial_congestion_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D8 + tag,
                                    //"select max(date_and_time) as max_time from arterial.arterial_congestion_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D12 + tag,

                                    "select max(date_and_time) as max_time from transit.metro_bus_data where agency = " + tag + RIITSBusAgency.MTA_Metro + tag,
                                    "select max(date_and_time) as max_time from transit.metro_bus_data where agency = " + tag + RIITSBusAgency.FHT + tag,
                                    "select max(date_and_time) as max_time from transit.metro_bus_data where agency = " + tag + RIITSBusAgency.LBT + tag,

                                    "select max(date_and_time) as max_time from transit.metro_rail_data",

                                    "select max(date_and_time) as max_time from transit.freeway_travel_time where agency = " + tag + RIITSFreewayAgency.Caltrans_D7 + tag,
                                    //"select max(date_and_time) as max_time from transit.freeway_travel_time where agency = " + tag + RIITSFreewayAgency.Caltrans_D8 + tag,
                                    //"select max(date_and_time) as max_time from transit.freeway_travel_time where agency = " + tag + RIITSFreewayAgency.Caltrans_D12 + tag,


                                    "select max(date_and_time) as max_time from transit.ramp_meter_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D7 + tag,
                                    "select max(date_and_time) as max_time from transit.ramp_meter_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D8 + tag,
                                    "select max(date_and_time) as max_time from transit.ramp_meter_data where agency = " + tag + RIITSFreewayAgency.Caltrans_D12 + tag,

                                    "select max(date_and_time) as max_time from event.event",

                                    "select max(date_and_time) as max_time from event.cms where agency = " + tag + RIITSFreewayAgency.Caltrans_D7 + tag,
                                    "select max(date_and_time) as max_time from event.cms where agency = " + tag + RIITSFreewayAgency.Caltrans_D8 + tag,
                                    "select max(date_and_time) as max_time from event.cms where agency = " + tag + RIITSFreewayAgency.Caltrans_D12 + tag
                                   };

            string[] datatypes = {"highway" + " from " + tag + RIITSFreewayAgency.Caltrans_D7 + tag,
                                     "highway" + " from " + tag + RIITSFreewayAgency.Caltrans_D8 + tag,
                                     "highway" + " from " + tag + RIITSFreewayAgency.Caltrans_D12 + tag,

                                    "arterial" + " from " + tag + RIITSArterialAgency.LADOT + tag,
                                    "arterial" + " from " + tag +RIITSFreewayAgency.Caltrans_D7 + tag,
                                    //"arterial" + " from " + tag +RIITSFreewayAgency.Caltrans_D8 + tag,
                                    //"arterial" + " from " + tag +RIITSFreewayAgency.Caltrans_D12 + tag,

                                    "bus" + " from " + tag + RIITSBusAgency.MTA_Metro + tag,
                                    "bus" + " from " + tag + RIITSBusAgency.FHT + tag,
                                    "bus" + " from " + tag + RIITSBusAgency.LBT + tag,

                                    "rail" + " from " + tag + RIITSBusAgency.MTA_Metro + tag,

                                    "travelTime" + " from " + tag +RIITSFreewayAgency.Caltrans_D7 + tag,
                                    //"travelTime" + " from " + tag +RIITSFreewayAgency.Caltrans_D8 + tag,
                                    //"travelTime" + " from " + tag +RIITSFreewayAgency.Caltrans_D12 + tag,

                                    "ramp" + " from " + tag +RIITSFreewayAgency.Caltrans_D7 + tag,
                                    "ramp" + " from " + tag +RIITSFreewayAgency.Caltrans_D8 + tag,
                                    "ramp" + " from " + tag +RIITSFreewayAgency.Caltrans_D12 + tag,

                                    "event", 
                                    "cms" + " from " + tag +RIITSFreewayAgency.Caltrans_D7 + tag,
                                    "cms" + " from " + tag +RIITSFreewayAgency.Caltrans_D8 + tag,
                                    "cms" + " from " + tag +RIITSFreewayAgency.Caltrans_D12 + tag
                                 };

            
            while(true)
            {

                DatabaseConnection con = new DatabaseConnection();
                con.OpenConnection();

                for (int i = 0; i < datatypes.Length; i++)
                {

                    ArrayList result7 = con.Query(queryString[i], "max_time");

                    if (result7.Count > 0)
                    {

                        string time = result7[0].ToString();

                        try
                        {
                            DateTime DBTime = DateTime.Parse(time);
                            long DBTimeStamp = DBTime.Hour * 60 + DBTime.Minute;
                            long CurrentStamp = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

                            long diff = CurrentStamp - DBTimeStamp;
                            if (diff >= 30)
                            {
                                string msg = "The latest updated time for data type: " + datatypes[i] + " is " + time;
                                msg += ", which is more than half an hour delay (Now: " + DateTime.Now.ToString() + ") \n";
                                msg += "The reason might be 1) Server is down or 2) RIITS data is not updated\n";
                                ExceptionEmailReporter myReporter = new ExceptionEmailReporter(msg, 0, "");
                                myReporter.SendEmailThread();
                                //System.Console.Write(time);
                            }
                        }
                        catch(Exception ex)
                        {
                            StreamWriter exception = new StreamWriter("exceptions.txt", true);
                            exception.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                            exception.WriteLine("Database time: " + time);
                            exception.Close();
                        }
                    }
                }

                con.CloseConnection();
                Thread.Sleep(30 * 60 * 1000);
            
            }
        }
    }
}

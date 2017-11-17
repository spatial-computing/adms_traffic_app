/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 06/09/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;

using Microsoft.ComplexEventProcessing;
using Microsoft.WindowsAzure.StorageClient;
using OutputTypes;
using Parsers;

namespace MyMemory
{
    public class RailGPSEntityTuple : TableServiceEntity
    {
        public RailGPSEntityTuple()
            : base()
        {
        }

        private const string DELIMITER = "-";

        public int ConfigID { get; set; }
        public string Agency { get; set; }
        public DateTime Datetime { get; set; }
        public int TrainId { get; set; }
        public String LineId { get; set; }
        public String RouteId { get; set; }
        public String RouteDes { get; set; }
        public String Destination { get; set; }
        public string OffRoute { get; set; }
        public int Direction { get; set; }
        public String Longitude { get; set; }
        public String Latitude { get; set; }
        public String Location_time { get; set; }
        public int Scheduled_dev { get; set; }
        public int Arrival_nextTP { get; set; }
        public String Next_location { get; set; }
        public int Timepoint { get; set; }

        public RailGPSEntityTuple(RailRoute sensor, RailGPSOutputElement ev)
        {

            ConfigID = BootUp.maxRailConfigID;
            Agency = sensor.Agency;
            Datetime = ev.StartTime.LocalDateTime;

            TrainId = ev.trainId;
            LineId = ev.lineId;
            RouteId = ev.routeId;
            RouteDes = ev.routeDes;
            Destination = ev.destination;
            OffRoute = ev.offRoute;
            Direction = ev.direction;
            Longitude = ev.Longitude;
            Latitude = ev.Latitude;
            Location_time = ev.Location_time;
            Scheduled_dev = ev.scheduled_dev;
            Arrival_nextTP = ev.arrival_nextTP;
            Next_location = ev.next_location;
            Timepoint = ev.timepoint;
            
            PartitionKey = CalcPartition();
            RowKey = CalcRowKey();
        }

        //private string CalcPartition()
        //{
        //    return RouteId + DELIMITER + Datetime.ToString("yyyyMMdd") + DELIMITER + Agency; 
        //}

        //private string CalcRowKey()
        //{
        //    return Datetime.ToString("HHmmss");
        //}
        private string CalcPartition()
        {
            return Datetime.ToString("yyyyMMdd") + DELIMITER + Agency;//+ Type;
        }

        public static string CalcPartition(DateTime datetime, string agency, string type)
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + agency + DELIMITER + type;
        }

        public static string CalcRowKey(int sensID, DateTime datetime)
        {
            return sensID + datetime.ToString("ddHHmmss");
        }

        private string CalcRowKey()
        {
            return TrainId + DELIMITER+ Datetime.ToString("HHmmss");
        }
    }
}

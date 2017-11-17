/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 06/19/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Parsers;

namespace MyMemory
{
    public class TravelTimeConfigEntityTuple : TableServiceEntity
    {
        private const string DELIMITER = "-";

        public int ID { get; set; }
        public int route { get; set; }
        public String Direction { get; set; }
        public String linkType { get; set; }
        public int beginID { get; set; }
        public int endID { get; set; }
        public double length { get; set; }

        public String beginStreet { get; set; }
        public double beginLat { get; set; }
        public double beginLon { get; set; }

        public String endStreet { get; set; }
        public double endLat { get; set; }
        public double endLon { get; set; }
        public String Agency { get; set; }
        public int ConfigID { get; set; }
        private DateTime datetime { get; set; }
        public long iDateTime { get; set; } // as Azure Table saves datetime in UTC format, we store date as a number to prevent any confusion.

        public TravelTimeConfigEntityTuple () : base() { }
        public TravelTimeConfigEntityTuple(travelLinks sensor, int configID, DateTime packetDateTime)
        {
            ConfigID = configID;
            ID = sensor.ID;
            route = sensor.route;
            Direction = sensor.Direction;
            linkType = sensor.linkType;
            beginID = sensor.beginID;
            endID = sensor.endID;
            length = sensor.length;
            beginStreet = sensor.beginStreet;
            beginLat = sensor.beginLat;
            beginLon = sensor.beginLon;
            endStreet = sensor.endStreet;
            endLat = sensor.endLat;
            endLon = sensor.endLon;
            Agency = sensor.Agency;
            
            datetime = packetDateTime;
            iDateTime = Int64.Parse(datetime.ToString("yyyyMMddHHmmss"));

            PartitionKey = CalcPartition();
            RowKey = CalcRowKey();
        }

        public string CalcPartition()
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + Agency; ;
        }
        public string CalcRowKey()
        {
            return ID + DELIMITER+ datetime.ToString("HHmmss");
        }

    }
}

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
    public class BusRouteEntityTuple : TableServiceEntity
    {
        private const string DELIMITER = "-";

        public int routeId { get; set; }
        public String routeDes { get; set; }
        public String zones { get; set; }
        public String Agency { get; set; }
        private DateTime datetime { get; set; }
        public long iDateTime { get; set; } // as Azure Table saves datetime in UTC format, we store date as a number to prevent any confusion.
        public int configID { get; set; }

        public BusRouteEntityTuple() : base() { }
        public BusRouteEntityTuple(BusRoute sensor, int configID, DateTime packetDateTime)
        {
            this.configID = configID;
            this.routeId= sensor.routeId;
            Agency = sensor.Agency;
            routeDes = sensor.routeDes;
            datetime = packetDateTime;
            iDateTime = Int64.Parse(datetime.ToString("yyyyMMddHHmmss"));
            zones = "";
            for (int i = 0; i < sensor.zones.Count(); i++)
                zones += sensor.zones[i] + ",";

            PartitionKey = CalcPartition();
            RowKey = CalcRowKey();
        }

        private string CalcPartition()
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + Agency; // + DELIMITER;//+ Type;
        }

        private string CalcRowKey()
        {
            return routeId+ datetime.ToString("HHmmss");
        }

    }
}

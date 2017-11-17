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
    public class RailRouteEntityTuple : TableServiceEntity
    {
        private const string DELIMITER = "-";
        
        public int ConfigID { get; set; }
        public int routeId { get; set; }
        public String routeDes { get; set; }
        public String Agency { get; set; }
        private DateTime datetime { get; set; }
        public long iDateTime { get; set; } // as Azure Table saves datetime in UTC format, we store date as a number to prevent any confusion.

        public RailRouteEntityTuple () : base() { }
        public RailRouteEntityTuple(RailRoute sensor, int configID, DateTime packetDateTime)
        {
            ConfigID = configID;
            routeId = sensor.routeId;
            Agency = sensor.Agency;
            routeDes = sensor.routeDes;
            datetime = packetDateTime;
            iDateTime = Int64.Parse(datetime.ToString("yyyyMMddHHmmss"));
           
            PartitionKey = CalcPartition(Agency);
            RowKey = CalcRowKey();
        }

        public string CalcPartition(string agency)
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + Agency; 
        }
        public  string CalcRowKey()
        {
            return routeId+ datetime.ToString("HHmmss");
        }

    }
}

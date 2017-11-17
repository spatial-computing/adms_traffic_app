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
    public class RampMeterEntityTuple : TableServiceEntity
    {
        private const string DELIMITER = "-";

        public int ConfigID { get; set; }
        public int ID { get; set; }
        public int msID { get; set; }
        public int rampType { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public String OnStreet { get; set; }
        public String FromStreet { get; set; }
        public String ToStreet { get; set; }
        public int Direction { get; set; }
        public String City { get; set; }
        public double PostMile { get; set; }
        public String Agency { get; set; }
        private DateTime datetime { get; set; }
        public long iDateTime { get; set; } // as Azure Table saves datetime in UTC format, we store date as a number to prevent any confusion.

        public RampMeterEntityTuple () : base() { }
        public RampMeterEntityTuple(RampMeter sensor, int configID, DateTime packetDateTime)
        {
            ConfigID = configID;
            ID = sensor.ID;
            msID = sensor.msID;
            rampType = sensor.rampType;
            latitude = sensor.latitude;
            longitude = sensor.longitude;
            OnStreet = sensor.OnStreet;
            FromStreet = sensor.FromStreet;
            ToStreet = sensor.ToStreet;
            Direction = sensor.Direction;
            City = sensor.City;
            PostMile = sensor.PostMile;
            Agency = sensor.Agency;

            datetime = packetDateTime;
            iDateTime = Int64.Parse(datetime.ToString("yyyyMMddHHmmss"));

            PartitionKey = CalcPartition(Agency);
            RowKey = CalcRowKey();
        }

        public string CalcPartition(string agency)
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + Agency; 
        }
        public string CalcRowKey()
        {
            return ID+ datetime.ToString("HHmmss");
        }

    }
}

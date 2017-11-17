using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Parsers;


namespace MyMemory
{
    public class CmsDeviceEntityTuple : TableServiceEntity
    {
        private const string DELIMITER = "-";

        public int ConfigID { get; set; }
        private DateTime datetime { get; set; }
        public string Agency { get; set; }
        public long iDateTime { get; set; } // as Azure Table saves datetime in UTC format, we store date as a number to prevent any confusion.

        public int ID { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public String OnStreet { get; set; }
        public int Direction { get; set; }
        public String FromStreet { get; set; }
        public String ToStreet { get; set; }
        public double PostMile { get; set; }
        public String City { get; set; }

        public CmsDeviceEntityTuple() : base() { }
        public CmsDeviceEntityTuple(CmsDevice device, int confID, DateTime packetDateTime)//, string link_type)
        {

            ConfigID = confID;
            Agency = device.Agency;
            
            datetime = packetDateTime;
            iDateTime = Int64.Parse(datetime.ToString("yyyyMMddHHmmss"));
            ID = device.ID;
            //  Link_type = link_type;
            OnStreet = device.OnStreet;
            FromStreet = device.FromStreet;
            ToStreet = device.ToStreet;
            Lat = device.GeogLocation.Lat.Value;
            Long = device.GeogLocation.Long.Value;
            Direction = device.Direction;
            PostMile = device.PostMile;
            City = device.City;

            PartitionKey = CalcPartition();
            RowKey = CalcRowKey();;
            
        }


        public string CalcPartition()
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + Agency; 
        }
        public string CalcRowKey()
        {
            return ID + datetime.ToString("HHmmss");
        }


    }
}

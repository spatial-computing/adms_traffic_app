﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Parsers;

namespace MyMemory
{
    public class SensorConfigEntityTuple : TableServiceEntity
    {

        private const string DELIMITER = "-";
        //protected abstract string Type { get; set; }
        public int ConfigID { get; set; }
        public string Agency { get; set; }
        private string City { get; set; }
        private DateTime datetime { get; set; }
        public long iDateTime { get; set; } // as Azure Table saves datetime in UTC format, we store date as a number to prevent any confusion.
        public string SensorID { get; set; }
       // public string Link_type { get; set; }
        public string OnStreet { get; set; }

        public string FromStreet { get; set; }
        public string ToStreet { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public int Direction { get; set; }
        public double PostMile { get; set; }
        public int AffectedNumOfLanes { get; set; }

        public SensorConfigEntityTuple() : base() { }
        public SensorConfigEntityTuple(Sensor sensor, int confID, DateTime packetDateTime)//, string link_type)
        {
            ConfigID = confID;
            Agency = sensor.Agency;
            City = sensor.City;
            datetime = packetDateTime;
            iDateTime = Int64.Parse(datetime.ToString("yyyyMMddHHmmss"));
            SensorID = sensor.ID;
          //  Link_type = link_type;
            OnStreet = sensor.OnStreet;
            FromStreet = sensor.FromStreet;
            ToStreet = sensor.ToStreet;
            Lat = sensor.GeogLocation.Lat.Value;
            Long = sensor.GeogLocation.Long.Value;
            Direction = sensor.Direction;
            PostMile = sensor.PostMile;
            AffectedNumOfLanes = sensor.AffectedLanes.Count;
            PartitionKey = CalcPartition();
            RowKey = CalcRowKey();
        }

        public string CalcPartition()
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + Agency; 
        }
        public string CalcRowKey()
        {
            return SensorID + datetime.ToString("HHmmss");
        }
    }
}

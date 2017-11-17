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
    public class TravelTimeEntityTuple : TableServiceEntity
    {
        public TravelTimeEntityTuple()
            : base()
        {
        }

        private const string DELIMITER = "-";
       
        public int ConfigID { get; set; }
        public string Agency { get; set; }
        private DateTime datetime { get; set; }
        public long iDateTime { get; set; }
        public int TravelId { get; set; }
        public double Speed { get; set; }
        public double TravelTime { get; set; }

        public TravelTimeEntityTuple(travelLinks sensor, TravelLinksOutputElement ev)
        {
            
            ConfigID = BootUp.maxTravelTimeConfigID;
            Agency = sensor.Agency;
            datetime = ev.StartTime.LocalDateTime;
            iDateTime = Int64.Parse(datetime.ToString("yyyyMMddHHmmss"));

            TravelId = ev.travelId;
            Speed = ev.speed;
            TravelTime = ev.travelTime;

            PartitionKey = CalcPartition();
            RowKey = CalcRowKey();
        }

        //private string CalcPartition()
        //{
        //    return TravelId + DELIMITER+ datetime.ToString("yyyyMMdd") + DELIMITER + Agency;
        //}

        //private string CalcRowKey()
        //{
        //    return datetime.ToString("HHmmss");
        //}
        private string CalcPartition()
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + Agency ;//+ Type;
        }

        public static string CalcPartition(DateTime datetime, string agency, string type)
        {
            return datetime.ToString("yyyyMMdd") + DELIMITER + agency + DELIMITER + type;
        }

        public static string CalcRowKey(int sensID, DateTime datetime)
        {
            return sensID + datetime.ToString("HHmmss");
        }

        private string CalcRowKey()
        {
            return TravelId +DELIMITER+ datetime.ToString("ddHHmmss");
        }

    }
}

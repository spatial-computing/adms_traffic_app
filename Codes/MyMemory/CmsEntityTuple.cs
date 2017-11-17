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
    public class CmsEntityTuple : TableServiceEntity
    {
        public CmsEntityTuple()
            : base()
        {
        }

        private const string DELIMITER = "-";
        public int ConfigID { get; set; }
        public String Agency { get; set; }
        public DateTime Datetime { get; set; }

        public int Id { get; set; }
        public String DeviceStatus { get; set; }
        public String State { get; set; }
        public String Date { get; set; }
        public String Time { get; set; }
        public String Phase1Line1 { get; set; }
        public String Phase1Line2 { get; set; }
        public String Phase1Line3 { get; set; }
        public String Phase2Line1 { get; set; }
        public String Phase2Line2 { get; set; }
        public String Phase2Line3 { get; set; }

        public CmsEntityTuple(CmsDevice device, CmsOutputElement ev)
        {

            ConfigID = BootUp.maxCmsConfigID;
            Agency = device.Agency;
            Datetime = ev.StartTime.LocalDateTime;

            Id = ev.Id;
            DeviceStatus = ev.DeviceStatus;
            State = ev.State;
            Date = ev.Date;
            Time = ev.Time;
            Phase1Line1 = ev.Phase1Line1;
            Phase1Line2 = ev.Phase1Line2;
            Phase1Line3 = ev.Phase1Line3;
            Phase2Line1 = ev.Phase2Line1;
            Phase2Line2 = ev.Phase2Line2;
            Phase2Line3 = ev.Phase2Line3;
            

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
            return Id + DELIMITER + Datetime.ToString("HHmmss");
        }
    }
}

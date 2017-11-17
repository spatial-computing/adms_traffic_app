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
    public class RampEntityTuple : TableServiceEntity
    {
        public RampEntityTuple ()
            : base()
        {
        }

        private const string DELIMITER = "-";
       
        public int ConfigID { get; set; }
        public string Agency { get; set; }
        private DateTime datetime { get; set; }
        public long iDateTime { get; set; }
        public int RampId { get; set; }
        public int MSId { get; set; }
        public int Device_status { get; set; }
        public int Meter_status { get; set; }
        public int Ramp_meter_control_type { get; set; }
        public int Meter_rate { get; set; }
        public int Occupancy { get; set; }
        public int Speed { get; set; }
        public int Volume { get; set; }
        public string Link_ids { get; set; }
        public string Dector_types { get; set; }
        public string Occupancies { get; set; }
        public string Speeds { get; set; }
        public string Volumes { get; set; }
        public string Link_statuses { get; set; }

        public RampEntityTuple(RampMeter sensor, RampOutputElement ev)
        {
            
            ConfigID = BootUp.maxRampMeterConfigID;
            Agency = sensor.Agency;
            datetime = ev.StartTime.LocalDateTime;
            iDateTime = Int64.Parse(datetime.ToString("yyyyMMddHHmmss"));

            RampId = ev.rampId;
            MSId = ev.MSId;
            Device_status = ev.device_status;
            Meter_status = ev.meter_status;
            Ramp_meter_control_type = ev.ramp_meter_control_type;
            Meter_rate = ev.meter_rate;
            Occupancy = ev.Occupancy;
            Speed = ev.Speed;
            Volume = ev.Volume;
            Link_ids = ev.link_ids;
            Dector_types = ev.dector_types;
            Occupancies = ev.occupancies;
            Speeds = ev.speeds;
            Volumes = ev.volumes;
            Link_statuses = ev.link_statuses;

            PartitionKey = CalcPartition();
            RowKey = CalcRowKey();
        }

        //private string CalcPartition()
        //{
        //    return RampId + DELIMITER + datetime.ToString("yyyyMMdd") + DELIMITER + Agency;
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
            return datetime.ToString("yyyyMM") + DELIMITER + agency + DELIMITER + type;
        }

        public static string CalcRowKey(int sensID, DateTime datetime)
        {
            return sensID + datetime.ToString("ddHHmmss");
        }

        private string CalcRowKey()
        {
            return RampId + DELIMITER+ datetime.ToString("HHmmss");
        }
    }
}

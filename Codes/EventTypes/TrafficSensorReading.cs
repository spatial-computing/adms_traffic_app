﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;

namespace EventTypes
{
    public class TrafficSensorReading
    {
        public TrafficSensorReading()
        {
        }

        public TrafficSensorReading(int hovSp, String linkStatus, int occ, string sensId, int sp, int vol)
        {
            HovSpeed = hovSp;
            LinkDataStatus = linkStatus;
            Occupancy = occ;
            SensorId = sensId;
            Speed = sp;
            Volume = vol;
        }

        public int HovSpeed { get; set; }
        public String LinkDataStatus { get; set; }
        public int Occupancy { get; set; }
        public string SensorId { get; set; }

        public int Speed { get; set; }
        public int Volume { get; set; }
    }

    // Freeway or Arterial inventory data.
    public class FreeterialInventorySensorReading
    {
        public FreeterialInventorySensorReading(string sensorId, string onStreet, string fromStreet, string toStreet, string lat, string lng, int direction, double postmile, int[] affectedLaneCount, string [] affectedLaneType)
        {
            SensorId = sensorId;
            OST = onStreet;
            FST = fromStreet;
            TST = toStreet;
            Lat = lat;
            Lng = lng;
            Direction = direction;
            Postmile = postmile;
            AffectedLaneCount = affectedLaneCount;
            AffectedLaneType = affectedLaneType;
        }

        public string SensorId { get; set; }
        public string OST { get; set; }
        public string FST { get; set; }
        public string TST { get; set; }
        public string Lat { get; set; } // string and not double. This is because the values written in XML file need to be processed to become valid doubles. So let's consider them as raw strings 
        public string Lng { get; set; }
        public int Direction { get; set; }
        public double Postmile { get; set; }
        public int [] AffectedLaneCount { get; set; }
        public string[] AffectedLaneType { get; set; }
    }   
    
    public class AverageSpeed
    {
        public AverageSpeed()
        {
        }

        public AverageSpeed(double speed, DateTimeOffset startTime)
        {
            Speed = speed;
            StartTime = startTime.UtcDateTime;
        }

        public double Speed { get; set; }
        public DateTime StartTime { get; set; }
    }

    public class doubleClass
    {
        public doubleClass()
        {
        }

        public doubleClass(double speed)
        {
            Speed = speed;
        }

        public double Speed { get; set; }
    }

    public class sensorIdAllAverageClass
    {
        public sensorIdAllAverageClass()
        {
        }

        public sensorIdAllAverageClass(int sensorID, double Aver)
        {
            SensorId = sensorID;
            Average = Aver;
        }

        public int SensorId { get; set; }
        public double Average { get; set; }
    }
}
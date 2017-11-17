/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using OutputTypes;
using Parsers;
using Utils;

namespace MyMemory
{
    public abstract class LookupArterial : LookupTable<ArterialSensorSpeedOutputElement>
    {
        private static BootUp memory = BootUp.GetInstance();
        //public override List<object> GetRecord(ArterialSensorSpeedOutputElement ev)
        public override Object GetRecord(ArterialSensorSpeedOutputElement ev, string agency)
        {
            Sensor sensor = new Sensor(agency, ev.SensorId, 0,0,"",-1,"");
            //Sensor sensor = memory.GetArterialSensInfo(ev.SensorId);
            //if (sensor == null)
            //    return null;

            var values = GetFields(sensor, ev);

            return values;
        }

        //protected abstract List<object> GetFields(Sensor sensor, ArterialSensorSpeedOutputElement ev);
        protected abstract Object GetFields(Sensor sensor, ArterialSensorSpeedOutputElement ev);
    }

    public class LookupArterialXML : LookupArterial
    {

        //protected override List<object> GetFields(Sensor sensor, ArterialSensorSpeedOutputElement ev)
        protected override Object GetFields(Sensor sensor, ArterialSensorSpeedOutputElement ev)
        {
            return new List<Object>
                                 {
                                     (int) Utilities.KMH2MPH(ev.Speed),
                                     sensor.GeogLocation.Lat,
                                     sensor.GeogLocation.Long,
                                     sensor.OnStreet,
                                     sensor.Direction,
                                     sensor.FromStreet,
                                     ev.Hovspeed,
                                     ev.Occupancy,
                                     ev.Volume,
                                     ev.StartTime.ToString("MMM-dd-yyyy HH:mm")
                                 };
        }
    }

    public class LookupArterialDB : LookupArterial
    {
        //protected override List<object> GetFields(Sensor sensor, ArterialSensorSpeedOutputElement ev)
        //{
        //    return new List<Object>
        //                         {
        //                             ev.SensorId,
        //                             (int) Utilities.KMH2MPH(ev.Speed),
        //                             sensor.GeogLocation.Lat,
        //                             sensor.GeogLocation.Long,
        //                             sensor.OnStreet,
        //                             sensor.Direction,
        //                             sensor.FromStreet,
        //                             ev.StartTime.Year+ev.StartTime.Month+ev.StartTime.Day+ " "+ ev.StartTime.Hour+ ":"+ ev.StartTime.Minute+ ":"+"00"
        //                         };
        //}

        //protected override List<object> GetFields(Sensor sensor, ArterialSensorSpeedOutputElement ev)
        protected override Object GetFields(Sensor sensor, ArterialSensorSpeedOutputElement ev)
        {
            return new List<Object>
                                 {
                                     BootUp.maxArterialConfigID,//ev.ConfigId,
                                     ev.SensorId,
                                     (int) Utilities.KMH2MPH(ev.Speed),
                                     //sensor.GeogLocation.Lat,
                                     //sensor.GeogLocation.Long,
                                    ev.StartTime.LocalDateTime.ToString("yyyyMMdd HH:mm:ss"),
                                    ev.Hovspeed,
                                    ev.Occupancy,
                                    ev.Volume,
                                    ev.Link_Status,
                                    //sensor.Direction, 
                                    //sensor.OnStreet,
                                    // sensor.FromStreet
                                    //sensor.Agency
                                 };
        }
    }

    public class LookUpArterialAzureTable : LookupArterial
    {
        protected override object GetFields(Sensor sensor, ArterialSensorSpeedOutputElement ev)
        {
            //return new ArterialSensorEntityTuple(sensor, ev);
            return new SensorEntityTuple(sensor, ev);
             }
        }
    }


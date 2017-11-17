/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */


/**
 * Updated by Bei (Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Add the GetFilterQueryTemplate for Bus, Rail, RMS, TravelTime
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;
using UDOs;

namespace SIServers
{
    internal class StreamUtils
    {
        public static CepStream<TrafficSensorReading> GetUnboundFilteredSelectedAreaFreewayStream(string inputName, DateTime alignment)
        {
            CepStream<TrafficSensorReading> sensorStreamForAvgSpeedQuery =
                //CepStream<TrafficSensorReading>.Create(inputName);
                GetFilteredSensorSpeedStream(inputName);
            CepStream<TrafficSensorReading> selectedArea =
                from win in
                    sensorStreamForAvgSpeedQuery.TumblingWindow(TimeSpan.FromMinutes(1), alignment,
                                                                WindowInputPolicy.ClipToWindow,
                                                                HoppingWindowOutputPolicy.ClipToWindowEnd)
                select win.InsideFreewayRect();
            return selectedArea;
        }
        public static CepStream<TrafficSensorReading> GetUnboundFilteredSelectedAreaArterialStream(string inputName, DateTime alignment)
        {
            CepStream<TrafficSensorReading> sensorStreamForAvgSpeedQuery =
                //CepStream<TrafficSensorReading>.Create(inputName);
                GetFilteredSensorSpeedStream(inputName);
            CepStream<TrafficSensorReading> selectedArea =
                from win in
                    sensorStreamForAvgSpeedQuery.TumblingWindow(TimeSpan.FromMinutes(1), alignment,
                                                                WindowInputPolicy.ClipToWindow,
                                                                HoppingWindowOutputPolicy.ClipToWindowEnd)
                //select win.InsideFreewayRect();
                select win.InsideArterialRect();
            return selectedArea;
        }

        public static QueryTemplate GetAvgSpeedForSltAreaQT(Application app, CepStream<TrafficSensorReading> selectedArea, String qtName, String qtDescription, DateTime alignment)
        {
            CepStream<doubleClass> AvgSpdForSltAreaEvent = from aa in selectedArea.TumblingWindow(TimeSpan.FromMinutes(1), alignment,
                                                                              WindowInputPolicy.ClipToWindow,
                                                                              HoppingWindowOutputPolicy.ClipToWindowEnd)
                                                           select
                                                               new doubleClass(aa.Avg(temp => temp.Speed));


            return app.CreateQueryTemplate(qtName, qtDescription, AvgSpdForSltAreaEvent);
        }

        public static CepStream<EventReading> GetEventStream(string inputName)
        {
            return CepStream<EventReading>.Create(inputName);
        }

        public static CepStream<TrafficSensorReading> GetFilteredSensorSpeedStream( string inputName)
        {
            CepStream<TrafficSensorReading> sensorStreamForSensorSpeedQuery =
                CepStream<TrafficSensorReading>.Create(inputName);
            var filteredSensorStreamForSensorSpeedQuery = from ev in sensorStreamForSensorSpeedQuery
                                                          //where ev.Speed > 0
                                                          select  (ev) ;

            

            

            //TODO: you might want to find the average value over the last minute.

            //var sensorSpeed =
            //    from oneMinReading in sensorStreamForSensorSpeedQuery.AlterEventDuration(e => TimeSpan.FromMinutes(1))
            //    group oneMinReading by oneMinReading.SensorId
            //        into oneGroup
            //        from eventWindow in
            //            oneGroup.SnapshotWindow(WindowInputPolicy.ClipToWindow, SnapshotWindowOutputPolicy.Clip)
            //        select new { speed = eventWindow.Avg(e => e.Speed), SensorId = oneGroup.Key };
            //return app.CreateQueryTemplate(QTName, "", sensorSpeed);)
            return filteredSensorStreamForSensorSpeedQuery;
        }

        public static QueryTemplate GetQT(Application app, CepStream<TrafficSensorReading> stream, string QTName, string QTDescription)
        {
            return app.CreateQueryTemplate(QTName,QTDescription , stream);
        }

        public static CepStream<TrafficBusGPSReading> GetFilteredBusStream(string inputName)
        {
            CepStream<TrafficBusGPSReading> sensorStreamForSensorSpeedQuery =
                CepStream<TrafficBusGPSReading>.Create(inputName);
            //var filteredSensorStreamForSensorSpeedQuery = from ev in sensorStreamForSensorSpeedQuery
              //                                            select ev;
            //return filteredSensorStreamForSensorSpeedQuery;
            return sensorStreamForSensorSpeedQuery;
        }

        public static CepStream<TrafficRailGPSReading> GetFilteredRailStream(string inputName)
        {
            CepStream<TrafficRailGPSReading> sensorStreamForSensorSpeedQuery =
                CepStream<TrafficRailGPSReading>.Create(inputName);
            var filteredSensorStreamForSensorSpeedQuery = from ev in sensorStreamForSensorSpeedQuery
                                                          select ev;
            return filteredSensorStreamForSensorSpeedQuery;
        }

        public static CepStream<TrafficRampReading> GetFilteredRampStream(string inputName)
        {
            CepStream<TrafficRampReading> sensorStreamForSensorSpeedQuery =
                CepStream<TrafficRampReading>.Create(inputName);
            var filteredSensorStreamForSensorSpeedQuery = from ev in sensorStreamForSensorSpeedQuery
                                                          //where ev.Speed > 0
                                                          select ev;
            return filteredSensorStreamForSensorSpeedQuery;
        }

        public static CepStream<TrafficTravelTimeReading> GetFilteredTravelTimeStream(string inputName)
        {
            CepStream<TrafficTravelTimeReading> sensorStreamForSensorSpeedQuery =
                CepStream<TrafficTravelTimeReading>.Create(inputName);
            var filteredSensorStreamForSensorSpeedQuery = from ev in sensorStreamForSensorSpeedQuery
                                                          //where ev.speed > 0
                                                          select ev;
            return filteredSensorStreamForSensorSpeedQuery;
        }

        public static CepStream<TrafficCmsReading> GetFilteredCmsStream(string inputName)
        {
            CepStream<TrafficCmsReading> sensorStreamForSensorSpeedQuery =
                CepStream<TrafficCmsReading>.Create(inputName);
            //var filteredSensorStreamForSensorSpeedQuery = from ev in sensorStreamForSensorSpeedQuery
            //                                            select ev;
            //return filteredSensorStreamForSensorSpeedQuery;
            return sensorStreamForSensorSpeedQuery;
        }
    }

    //internal class SensorEventComparer : IEqualityComparer<TrafficSensorReading>{
    //    public bool Equals(TrafficSensorReading x, TrafficSensorReading y)
    //    {
    //        if( x.SensorId == y.SensorId && x.)
    //    }

    //    public int GetHashCode(TrafficSensorReading obj)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

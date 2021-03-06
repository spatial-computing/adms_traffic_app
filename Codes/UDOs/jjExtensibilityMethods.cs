﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using ADMS;
using EventTypes;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Extensibility;
using Microsoft.ComplexEventProcessing.Linq;
using Microsoft.SqlServer.Types;
using MyMemory;
using Parsers;
using Utils;

namespace UDOs
{
    public class Rectangle
    {
        private readonly double lat1;
        private readonly double lat2;
        private readonly double long1;
        private readonly double long2;
        //private double x1;
        //private double x2;
        //private double y1;
        //private double y2;

        public Rectangle(double lat1, double long1, double lat2, double long2)
        {
            this.lat1 = lat1;
            this.lat2 = lat2;
            this.long1 = long1;
            this.long2 = long2;
            //UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, lat1, long1);
            //double x = UTMConverter.UTMEasting;
            //double y = UTMConverter.UTMNorthing;
            //y1 = y;
            //x1 = x;

            //UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, lat2, long2);
            //x = UTMConverter.UTMEasting;
            //y = UTMConverter.UTMNorthing;

            //y2 = y;
            //x2 = x;
        }

        public double Lat1
        {
            get { return lat1; }
        }

        public double Lat2
        {
            get { return lat2; }
        }

        public double Long1
        {
            get { return long1; }
        }

        public double Long2
        {
            get { return long2; }
        }


        public SqlGeography GetBoundingRectangle()
        {
            String polygonText = "POLYGON ((";
            //double miny = Math.Min(y1, y2);
            //double maxy = Math.Max(y1, y2);

            //double minx = Math.Min(x1, x2);
            //double maxx = Math.Max(x1, x2);

            //polygonText += minx + " " + miny + ", ";
            //polygonText += minx + " " + maxy + ", ";
            //polygonText += maxx + " " + maxy + ", ";
            //polygonText += maxx + " " + miny + ", ";
            //polygonText += minx + " " + miny + "))";

            double minlat = Math.Min(lat1, lat2);
            double maxlat = Math.Max(lat1, lat2);

            double minlong = Math.Min(long1, long2);
            double maxlong = Math.Max(long1, long2);

            polygonText += minlong + " " + minlat + ", ";
            polygonText += maxlong + " " + minlat + ", ";
            polygonText += maxlong + " " + maxlat + ", ";
            polygonText += minlong + " " + maxlat + ", ";
            polygonText += minlong + " " + minlat + "))";


            SqlGeography result = SqlGeography.Parse(polygonText);
            return result;
        }
    }


    public class InsideArterialRectangle : CepTimeSensitiveOperator<TrafficSensorReading, TrafficSensorReading>
    {
        public static Rectangle Rect = new Rectangle(34.05444043974997, -118.3459287368164, 34.01731225215423,
                                                     -118.263187953125);
        //public static Rectangle Rect = new Rectangle(34.02444043974997, -118.3459287368164, 34.01731225215423,
        //                                             -118.303187953125);
        //public UDOConfig config;
        //public InsideFreewayRectangle(UDOConfig config)
        //{
        //    this.config = config;
        //}

        public override IEnumerable<IntervalEvent<TrafficSensorReading>> GenerateOutput(
            IEnumerable<IntervalEvent<TrafficSensorReading>> events, WindowDescriptor windowDescriptor)
        {
            var result = new List<IntervalEvent<TrafficSensorReading>>();
            if (Rect == null)
                return result;
            SqlGeography rectangle = Rect.GetBoundingRectangle();
            //SqlGeography rectangle = Rect.GetBoundingRectangle();

            double minLat = Math.Min(Rect.Lat1, Rect.Lat2);
            double maxLat = Math.Max(Rect.Lat1, Rect.Lat2);

            double minLong = Math.Min(Rect.Long1, Rect.Long2);
            double maxLong = Math.Max(Rect.Long1, Rect.Long2);


            //IEnumerable<IntervalEvent<TrafficSensorReading>> result = new List<IntervalEvent<TrafficSensorReading>>();
            Dictionary<string, Sensor> allSensors = BootUp.ArterialLinkLocDic;
            

            foreach (var ev in events)
            {
                string sensorId = ev.Payload.SensorId;
                bool contains = allSensors.Keys.Contains(sensorId);
                SqlBoolean intersects = SqlBoolean.False;
                if (contains)
                {
                    SqlGeography loc = allSensors[sensorId].GeogLocation;
                    intersects = rectangle.STIntersects(loc);
                }

                if (contains && intersects.IsTrue) //todo: figure it out.
                {
                    result.Add(ev);
                }
                //if( contains)
                //{
                //    SqlGeography geogLoc = allSensors[sensorId].GeogLocation;

                //    if( Utilities.IsBetween(geogLoc.Lat.Value, minLat, maxLat) && Utilities.IsBetween(geogLoc.Long.Value, minLong, maxLong))
                //    result.Add(ev);
                //}
            }

            return result;
        }
    }


    public class InsideFreewayRectangle : CepTimeSensitiveOperator<TrafficSensorReading, TrafficSensorReading>
    {
        //public static Rectangle Rect{ get; set; }
            //todo: delete the following line for future. just written for demo purposes for Barak. then, uncomment the line above this.
        public static Rectangle Rect = new Rectangle(34.05444043974997, -118.3459287368164, 34.01731225215423,
                                                    -118.263187953125);
        //public UDOConfig config;
        //public InsideFreewayRectangle(UDOConfig config)
        //{
        //    this.config = config;
        //}

        public override IEnumerable<IntervalEvent<TrafficSensorReading>> GenerateOutput(
            IEnumerable<IntervalEvent<TrafficSensorReading>> events, WindowDescriptor windowDescriptor)
        {
            var result = new List<IntervalEvent<TrafficSensorReading>>();
            if (Rect == null)
                return result;
            SqlGeography rectangle = Rect.GetBoundingRectangle();
            //SqlGeography rectangle = Rect.GetBoundingRectangle();

            double minLat = Math.Min(Rect.Lat1, Rect.Lat2);
            double maxLat = Math.Max(Rect.Lat1, Rect.Lat2);

            double minLong = Math.Min(Rect.Long1, Rect.Long2);
            double maxLong = Math.Max(Rect.Long1, Rect.Long2);


            //IEnumerable<IntervalEvent<TrafficSensorReading>> result = new List<IntervalEvent<TrafficSensorReading>>();
            Dictionary<string, Sensor> allSensors = BootUp.FreewayLinkLocDic;
            //Dictionary<int, ArterialSensor> allSensors = BootUp.GetInstance().arterialLinkLocDic;

            foreach (var ev in events)
            {
                string sensorId = ev.Payload.SensorId;
                bool contains = allSensors.Keys.Contains(sensorId);
                SqlBoolean intersects = SqlBoolean.False;
                if (contains)
                {
                    SqlGeography loc = allSensors[sensorId].GeogLocation;
                    intersects = rectangle.STIntersects(loc);
                }

                if (contains && intersects.IsTrue) //todo: figure it out.
                {
                    result.Add(ev);
                }
                //if( contains)
                //{
                //    SqlGeography geogLoc = allSensors[sensorId].GeogLocation;

                //    if( Utilities.IsBetween(geogLoc.Lat.Value, minLat, maxLat) && Utilities.IsBetween(geogLoc.Long.Value, minLong, maxLong))
                //    result.Add(ev);
                //}
            }

            return result;
        }
    }

    //public class PCAPrediction : CepTimeSensitiveOperator<sensorIdAllAverageClass, AverageSpeed>
    public class PCAPrediction : CepTimeSensitiveOperator<doubleClass, AverageSpeed>
    {
        private static readonly Object lockThis = new Object();

        private static List<string> sensorIDs;

        internal static void SetList(List<string> newList)
        {
            lock (lockThis)
            {
                sensorIDs = newList;
            }
        }

        public override IEnumerable<IntervalEvent<AverageSpeed>> GenerateOutput(
            IEnumerable<IntervalEvent<doubleClass>> events, WindowDescriptor windowDescriptor)
        {
            var result = new List<IntervalEvent<AverageSpeed>>();
            if (events.Count() == 0)
                return result;
            PCA pcaInstance = PCA.GetInstance();
            int mostSimilarDayIndex = 0;// Commented by Penny pcaInstance.GetMostSimilar(sensorIDs, events.ToList());
            IntervalEvent<doubleClass> lastEvent = events.First();
            foreach (var intervalEvent in events)
            {
                if (intervalEvent.StartTime > lastEvent.StartTime)
                    lastEvent = intervalEvent;
            }
            for (int i = 0; i < 15; i++)
            {
                DateTime predictionTime = lastEvent.StartTime.AddMinutes(i).LocalDateTime;
                double tempAverage = pcaInstance.FindAverage(sensorIDs, mostSimilarDayIndex,
                                                             PCACoefLoader.GetTimeIndex(predictionTime));

                IntervalEvent<AverageSpeed> newEelement = CreateIntervalEvent();

                newEelement.Payload = new AverageSpeed(tempAverage, predictionTime);
                newEelement.StartTime = lastEvent.StartTime;
                newEelement.EndTime = lastEvent.EndTime;
                result.Add(newEelement);
            }

            //foreach (IntervalEvent<doubleClass> ev in events)
            //{
            //    IntervalEvent<AverageSpeed> first = CreateIntervalEvent();
            //    first.Payload = new AverageSpeed(10, new DateTime(1, 2, 3, 4, 5, 0));
            //    first.StartTime = ev.StartTime;
            //    first.EndTime = ev.EndTime;
            //    result.Add(first);
            //}


            return result;
        }
    }

    public class PCAAverage : CepTimeSensitiveAggregate<TrafficSensorReading, double>
    {
        public override double GenerateOutput(IEnumerable<IntervalEvent<TrafficSensorReading>> events,
                                              WindowDescriptor windowDescriptor)
        {
            DateTimeOffset time = events.ToList()[0].StartTime;
            var sensorIds = new List<string>();
            double result = 0;
            PCA pcaFinder = PCA.GetInstance();
            int count = 0;
            foreach (var ev in events)
            {
                double temp = pcaFinder.HistoricValueForSeptember21st(ev.Payload.SensorId, time.LocalDateTime);


                if (temp != PCACoefLoader.NO_VALUE)
                {
                    {
                        result += temp;
                        sensorIds.Add(ev.Payload.SensorId);
                        count++;
                    }
                }
            }
            PCAPrediction.SetList(sensorIds);
            if (count == 0)
                return 0;
            return result/count;
        }
    }

    public static class MyUserDefinedExtensionMethods
    {
        [CepUserDefinedOperator(typeof (InsideFreewayRectangle))]
        public static TrafficSensorReading InsideFreewayRect(this CepWindow<TrafficSensorReading> window)
        //public static TrafficSensorReading InsideFreewayRect(this CepWindow<TrafficSensorReading> window,UDOConfig sensorTypeConfig)
        {
            throw CepUtility.DoNotCall();
        }
        [CepUserDefinedOperator(typeof(InsideArterialRectangle))]
        public static TrafficSensorReading InsideArterialRect(this CepWindow<TrafficSensorReading> window)
        //public static TrafficSensorReading InsideFreewayRect(this CepWindow<TrafficSensorReading> window,UDOConfig sensorTypeConfig)
        {
            throw CepUtility.DoNotCall();
        }

        //[CepUserDefinedOperator(typeof(PCAPrediction))]
        //public static AverageSpeed Predict(this CepWindow<sensorIdAllAverageClass> window)
        //{
        //    throw CepUtility.DoNotCall();
        //}

        [CepUserDefinedOperator(typeof (PCAPrediction))]
        public static AverageSpeed Predict(this CepWindow<doubleClass> window)
        {
            throw CepUtility.DoNotCall();
        }

        [CepUserDefinedAggregate(typeof (PCAAverage))]
        public static double HistoricAverage(this CepWindow<TrafficSensorReading> window)
        {
            throw CepUtility.DoNotCall();
        }
    }

    public class WeightedAverage
    {
        private static double historicWeight;

        public static double HistoricWeight
        {
            get { return historicWeight; }
            set { if (value < 1 && value > 0) historicWeight = value; }
        }

        public static double ProcessedAvg(double historicAvg, double rawAvg)
        {
            return (historicWeight*historicAvg + (1 - historicWeight)*rawAvg);
        }
    }

    //public class ListOfSensors
    //{
    //    private static int[] myList = new[]
    //                                      {
    //                                          716028,
    //                                          759163,
    //                                          763646,
    //                                          759257,
    //                                          737229,
    //                                          737237,
    //                                          737246,
    //                                          737257,
    //                                          765506,
    //                                          737278,
    //                                          764054,
    //                                          718410,
    //                                          764125,
    //                                          764130,
    //                                          718412,
    //                                          763336,
    //                                          717008,
    //                                          717009,
    //                                          768584,
    //                                          717011,
    //                                          716036,
    //                                          717014,
    //                                          717016,
    //                                          717020,
    //                                          717022,
    //                                          718415
    //                                      };

    //    private static Random rand = new Random();
    //    private static ListOfSensors instance;

    //    private ListOfSensors(int[] list)
    //    {
    //        myList = list;
    //    }

    //    public static bool Includes(int element)
    //    {
    //        if (myList.Contains(element))
    //            return true;
    //        return false;
    //    }

    //    public ListOfSensors GetInstance()
    //    {
    //        return instance;
    //    }

    //    public static void SetList(int[] list)
    //    {
    //        myList = list;
    //    }
    //}
}
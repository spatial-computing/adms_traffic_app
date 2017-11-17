using System;
using System.Collections.Generic;
using BaseOutputAdapter;
using EventTypes;
using Memory;
using Microsoft.ComplexEventProcessing;
using Parsers;
using Utils;

namespace XMLOutputAdapter
{
    public class XmlPointArterialSensorSpeedOutput<T> : XmlPointSensorSpeedOutput<T> where T : IOutputType, new()
    {
        public XmlPointArterialSensorSpeedOutput(TrafficOutputConfig configInfo, CepEventType cepEventType)
            : base(configInfo, cepEventType)
        {
        }

        protected override String CreateOutputMessage()
        {
            BootUp memory = BootUp.GetInstance();
            String xmlStr = Config.Header //"<?xml version=\"1.0\"?>"
                            + Config.RootName; //"<mkrs>";

            foreach (T e in buffer)
            {
                ArterialSensor sensor = memory.GetArterialSensInfo((e.SensorId));

                if (sensor == null)
                    continue;
                DateTimeOffset temp = e.StartTime.LocalDateTime;
                //Question: I don't know how to solve the cultureinfo problem. LA time: -8 UTC
                var values = new List<Object>
                                 {
                                     (int) Utilities.KMH2MPH(e.Speed),
                                     sensor.GeogLocation.Lat,
                                     sensor.GeogLocation.Long,
                                     sensor.OnStreet,
                                     sensor.Direction,
                                     sensor.FromStreet,
                                     temp.ToString("MMM-dd-yyyy HH:mm")
                                 };

                xmlStr += Config.OtherTopStories;
                for (int i = 0; i < values.Count; i++)
                    xmlStr += Config.OutputFieldOrders[i] + values[i] + EndTags[i];

                xmlStr += Config.OtherTopStories.Insert(1, "/");
            }
            xmlStr += Config.RootName.Insert(1, "/");


            //TextWriter tw = new StreamWriter("C:/inetpub/wwwroot/WebApplication1/HighwayTraffic.xml");
            return xmlStr;
        }
    }
}
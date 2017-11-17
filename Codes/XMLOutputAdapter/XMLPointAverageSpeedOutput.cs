using System;
using System.Collections.Generic;
using System.IO;
using BaseOutputAdapter;
using Memory;
using Microsoft.ComplexEventProcessing;
using Utils;

namespace XMLOutputAdapter
{
    public class XmlPointAverageSpeedOutput : XmlPointOutput
    {
        
        public XmlPointAverageSpeedOutput(TrafficOutputConfig configInfo, CepEventType cepEventType)
            : base(configInfo, cepEventType)
        {
            
        }

        /// <summary>
        /// Returns true of a CTI event is observed. It also sets the allReceived variable.
        /// </summary>
        /// <param name="currEvent"></param>
        /// <returns></returns>
        protected override bool UpdateEndOfMinute(PointEvent currEvent)
        {
            bool result = false;
            if (EventKind.Cti == currEvent.EventKind)
            {
                if (buffer.Count > 0)
                {
                    EventArgs args = new EventArgs<String>(((AverageSpeedOutputElement) buffer[0]).StartTime.ToString());
                    result = true;
                    GenerateOutput();
                    buffer.Clear();
                    OnChanged(args);
                }
                else
                {
                    Console.WriteLine("No element in buffer");
                    // todo: jj added on 8/29/2010 
                    OnChanged(new EventArgs<String>("No element in buffer"));
                }
            }
            else
            {
                buffer.Add(new AverageSpeedOutputElement((Double) currEvent.GetField(0), currEvent.StartTime));
                Console.WriteLine("Average Speed: " + Utilities.KMH2MPH((double) currEvent.GetField(0)));

                //Console.WriteLine("Average speed is: " + currEvent.GetField(0));
            }
            return result;
        }

        protected override String CreateOutputMessage()
        {
            BootUp bootUp = BootUp.GetInstance();
            String xmlStr = Config.Header //"<?xml version=\"1.0\"?>"
                            + Config.RootName; //"<mkrs>";
            int count = 0;
            bool add = false;
            if (Config.OtherTopStories.Length > 0)
                add = true;
            foreach (AverageSpeedOutputElement e in buffer)
            {
                DateTimeOffset temp = e.StartTime.LocalDateTime;
                // -TimeSpan.FromHours(8); //Question: how should I solve this?
                var values = new List<Object>
                                 {
                                     temp.ToString("MMM-dd-yyyy"),
                                     temp.ToString(" HH:mm"),
                                     (int) Utilities.KMH2MPH(e.Speed)
                                 };
                xmlStr += Config.OtherTopStories;
                for (int i = 0; i < values.Count; i++)
                    xmlStr += Config.OutputFieldOrders[i] + values[i] + EndTags[i];

                if (add)
                    xmlStr += Config.OtherTopStories.Insert(1, "/");

                count++;
            }
            xmlStr += Config.RootName.Insert(1, "/");
            //xmlStr += config.Header.Insert(1, "/");
            //TextWriter tw = new StreamWriter("C:/Program Files/Apache Software Foundation/Tomcat 5.5/webapps/SI/" + config.OutputFileName);
            return xmlStr;
        }

        protected override void GenerateOutput()
        {
            lock (typeof (XmlPointAverageSpeedOutput))
            {
                //  Random rand = new Random();
                StreamWriter = new StreamWriter(Config.OutputFileName); //+ rand.Next());
                StreamWriter.Write(CreateOutputMessage());
                StreamWriter.Close();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using BaseOutputAdapter;
using Memory;
using Microsoft.ComplexEventProcessing;
using Utils;

namespace XMLOutputAdapter
{
    internal class XmlPointPredictionSpeedOutput : XmlPointAverageSpeedOutput
    {
        public XmlPointPredictionSpeedOutput(TrafficOutputConfig configInfo, CepEventType cepEventType)
            : base(configInfo, cepEventType)
        {
            buffer = new List<Object>();
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
                    EventArgs args =
                        new EventArgs<String>(((PredictionSpeedOutputElement) buffer[0]).StartTime.ToString());
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
                buffer.Add(new PredictionSpeedOutputElement((Double) currEvent.GetField(0),
                                                            (DateTime) currEvent.GetField(1)));
                Console.WriteLine("Prediction: " + Utilities.KMH2MPH((double) currEvent.GetField(0)));

                //Console.WriteLine("Average speed is: " + currEvent.GetField(0));
            }
            return result;
        }

        protected override String CreateOutputMessage()
        {
            BootUp memory = BootUp.GetInstance();
            String xmlStr = Config.Header //"<?xml version=\"1.0\"?>"
                            + Config.RootName; //"<mkrs>";
            int count = 0;
            bool add = false;
            if (Config.OtherTopStories.Length > 0)
                add = true;
            var comparer = new TimeComparer();
            buffer.Sort(comparer);
            foreach (PredictionSpeedOutputElement e in buffer)
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

        #region Nested type: TimeComparer

        private class TimeComparer : IComparer<Object>
        {
            #region IComparer<object> Members

            public int Compare(Object x, Object y)
            {
                return
                    ((PredictionSpeedOutputElement) x).StartTime.CompareTo(((PredictionSpeedOutputElement) y).StartTime);
            }

            #endregion
        }

        #endregion
    }
}
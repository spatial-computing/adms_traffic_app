using System;
using System.Collections.Generic;
using System.IO;
using BaseOutputAdapter;
using EventTypes;
using Microsoft.ComplexEventProcessing;

namespace XMLOutputAdapter
{
    public abstract class XmlPointSensorSpeedOutput<T> : XmlPointOutput<T> where T : IOutputType, new()
    {
        

        public XmlPointSensorSpeedOutput(TrafficOutputConfig configInfo, CepEventType cepEventType)
            : base(configInfo, cepEventType)
        {
        
        }

        /// <summary>
        /// Returns true if a CTI event is observed. It also sets the allReceived variable.
        /// </summary>
        /// <param name="currEvent"></param>
        /// <returns></returns>
        protected override bool UpdateEndOfMinute(PointEvent currEvent)
        {
            bool result = false;
            if (EventKind.Cti != currEvent.EventKind)
            {
                T newT= new T();
                newT.Initialize(currEvent);
                buffer.Add(newT);
            }
            else
            {
                if (buffer.Count > 0)
                {
                    //EventArgs args = new EventArgs<String>(buffer[0].StartTime.ToString());
                    result = true;
                    GenerateOutput();
                    buffer.Clear();
                    //OnChanged(args);
                }
                else
                {
                    Console.WriteLine("No element in buffer");
                }
            }
            return result;
        }


        //private static int fileNumber = 0;
        protected override void GenerateOutput()
        {
            TextWriter tw =
                new StreamWriter( //"C:/Users/Jalal/Documents/My Dropbox/C# projects/averageQuery/WebApplication1/" +
                    //      @"F:\Jalal\Arterial For Colin from 6 pm\" + fileNumber + ".xml");
                    Config.OutputFileName); //+ new Random().Next()); //  
            //     fileNumber++;
            tw.Write(CreateOutputMessage());

            tw.Close();
            Console.WriteLine("Query 1 wrote to file " + Config.OutputFileName);
            // Question: Instead of writing "Query 1 wro...", how can I find the query/application that ran this adapter? 
        }
    }
}
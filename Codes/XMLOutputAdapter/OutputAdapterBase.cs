/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using EventTypes;
using ExceptionReporter;
using MediaWriters;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using System.IO;
using OutputTypes;
using ExceptionReporter;

namespace XMLOutputAdapter
{
    public class OutputAdapterBase<T> : PointOutputAdapter where T : IOutputType, new()
    {
        protected List<T> buffer;
        protected String Delimiter = ",";
        protected CepEventType BindtimeEventType;
        protected TrafficOutputConfig Config;
        protected long EventsDequeued;
        
        //public delegate String CreateOutputMessage(List<T> buffer);
        //public delegate void GenerateOutput();


        IOutputMessage<T> myCreateOutputMessage;
        protected MediaWriter mediaWriter;
        
        public OutputAdapterBase(TrafficOutputConfig configInfo, CepEventType cepEventType, MediaWriter outputMedia, IOutputMessage<T> messageType)
        {
            Config = configInfo;
            BindtimeEventType = cepEventType;
            buffer = new List<T>();
            myCreateOutputMessage = messageType;
            mediaWriter = outputMedia;
        }


        public override void Start()
        {
            ConsumeEvents();
        }

        public override void Resume()
        {
            ConsumeEvents();
        }


        public void FileExpender(string fileName, string content)
        {
            try
            {
                //Added by Penny
                StreamWriter tempWriter = new StreamWriter(".\\logFolder\\" + fileName, true);
                tempWriter.WriteLine(DateTime.Now.ToString() + ":" + content);
                tempWriter.Close();
                tempWriter.Dispose();
            }
            catch (Exception)
            {
            }
        }


        protected void ConsumeEvents()
        {
            PointEvent currEvent = default(PointEvent);
            DequeueOperationResult result;

            try
            {
                while (true)
                {
                    if (AdapterState.Stopping == AdapterState)
                    {
                        result = Dequeue(out currEvent);
                        PrepareToStop(currEvent, result);
                        Stopped();
                        //  AdapterStopSignal.Set();

                        FileExpender("OutputAdpator_stopped" + Config.SourceDataType.ToString() + "_" + Config.OutputMediaType.ToString() + ".txt", "");

                        return;
                    }

                    result = Dequeue(out currEvent);
                    if (DequeueOperationResult.Empty == result)
                    {
                        PrepareToResume();

                        //FileExpender(Config.SourceDataType.ToString() + "_" + Config.OutputMediaType.ToString() + "OutputAdpator_resume1.txt", AdapterState.ToString());

                        Ready();

                        //FileExpender( "OutputAdpator_resume"+ Config.SourceDataType.ToString() + "_" + Config.OutputMediaType.ToString()+".txt", AdapterState.ToString());

                        return;
                    }
                    else
                    {
                        EventsDequeued++;
                     //   Line = CreateLogLineFromEvent(currEvent);
                        UpdateEndOfMinute(currEvent);
                        ReleaseEvent(ref currEvent);
                        
                        //FileExpender(
                        //    "OutputAdpator_dequeue" + Config.OutputMediaType.ToString() + "_" + Config.SourceDataType.ToString() + ".txt",
                        //    " " + EventsDequeued.ToString() + " Dequeue Finished");

                    }
                }
            }
            catch (AdapterException e)
            {
                Console.WriteLine("ConsumeEvents - " + e.Message + e.StackTrace);
                FileExpender("OutputAdapter_Exception" + Config.OutputMediaType.ToString() + "_" + Config.SourceDataType.ToString() + ".txt", e.Message);

                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 122, "OutputAdapterBase.cs");
                reporter.SendEmailThread();


                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(e.Message, 122, "OutputAdapterBase.cs", Config.SourceDataType.ToString(), "SI");
                reporter2.SendDatabaseExceptionThread();
            }
        }

        protected void PrepareToStop(PointEvent currEvent, DequeueOperationResult result)
        {
            if (DequeueOperationResult.Success == result)
            {
                ReleaseEvent(ref currEvent);
            }

            //Question: Should I close the streamwriter here or somewhere else?
            // _streamWriter.Close();
            //    _streamWriter.Dispose();
        }

        protected void PrepareToResume()
        {
            // The server can asynchronously call Resume at any point in time while the execution control
            // is in this routine. This routine can be used for any housekeeping before being resumed.
        }
      


        protected string CreateLogLineFromEvent(PointEvent currEvent)
        {
            string line = default(string);

            if (EventKind.Cti == currEvent.EventKind)
            {
                line += " CTI" + Delimiter + currEvent.StartTime;
            }
            else
            {
                line += " INSERT" + Delimiter + currEvent.StartTime + Delimiter;
                for (int ordinal = 0; ordinal < BindtimeEventType.FieldsByOrdinal.Count; ordinal++)
                {
                    CepEventTypeField evtField = BindtimeEventType.FieldsByOrdinal[ordinal];
                    object value = Convert.ChangeType(currEvent.GetField(ordinal), evtField.Type.ClrType,
                                                      CultureInfo.CurrentCulture);
                    line += (value != null) ? value.ToString() : "NULL";
                    line += Delimiter;
                }
            }

            return line;
        }
        /// <summary>
        /// Returns true of a CTI event is observed. It also sets the allReceived variable.
        /// </summary>
        /// <param name="currEvent"></param>
        /// <returns></returns>
        protected bool UpdateEndOfMinute(PointEvent currEvent)
        {
            bool result = false;
            if (EventKind.Cti != currEvent.EventKind)
            {
                T newT = new T();
                newT.Initialize(currEvent);
                buffer.Add(newT);

                //FileExpender(
                //            "OutputAdpator_buffersize" + Config.OutputMediaType.ToString() + "_" + Config.SourceDataType.ToString() + ".txt",
                //            " " + buffer.Count.ToString() );

            }
            else
            {
                if (buffer.Count > 0)
                {
                    //EventArgs args = new EventArgs<String>(buffer[0].StartTime.ToString());

                    DateTime startWritingTime = DateTime.Now;



                    /*if (Config.SourceDataType== "Bus")
                    {
                        int sum = 0;
                        HashSet<string> keys = new HashSet<string>();
                        for (int i = 0; i < buffer.Count; ++i)
                        {
                            BusGPSOutputElement tmp =
                                (buffer.ElementAt(i) as BusGPSOutputElement);

                            string key = tmp.busId.ToString();

                            if (!keys.Contains(key))
                                keys.Add(key);
                            else
                            {
                                sum++;
                            }
                        }

                        FileExpender("BusDataDuplicates.txt", buffer.Count.ToString()+" "+sum.ToString());
                    }*/

                    if (Config.SourceDataType== "TravelTime")
                    {
                        HashSet<string> keys = new HashSet<string>();
                        for (int i = 0; i < buffer.Count; ++i)
                            {
                                TravelLinksOutputElement tmp =
                                    (buffer.ElementAt(i) as TravelLinksOutputElement);

                                string key = tmp.travelId.ToString();

                                if (!keys.Contains(key))
                                    keys.Add(key);
                                else
                                {
                                    buffer.RemoveAt(i);
                                    --i;
                                }
                            }
                    }

                    #region filter out the duplicates  (Commented by Penny)
/*                  
                    HashSet<string> keys = new HashSet<string>();

                    switch(Config.SourceDataType){
                        case "Freeway":
                            for (int i = 0;  i < buffer.Count; ++i)
                            {
                                FreewaySensorSpeedOutputElement tmp =
                                    (buffer.ElementAt(i) as FreewaySensorSpeedOutputElement);

                                string key = tmp.SensorId.ToString();

                                if (!keys.Contains(key))
                                    keys.Add(key);
                                else
                                {
                                    buffer.RemoveAt(i);
                                    --i;
                                }
                            }
                            break;

                        case "Arterial":

                            for (int i = 0; i < buffer.Count; ++i)
                            {
                                ArterialSensorSpeedOutputElement tmp =
                                    (buffer.ElementAt(i) as ArterialSensorSpeedOutputElement);

                                string key = tmp.SensorId.ToString();

                                if (!keys.Contains(key))
                                    keys.Add(key);
                                else
                                {
                                    buffer.RemoveAt(i);
                                    --i;
                                }
                            }
                            break;

                        case "Bus":
                            for (int i = 0; i < buffer.Count; ++i)
                            {
                                BusGPSOutputElement tmp =
                                    (buffer.ElementAt(i) as BusGPSOutputElement);

                                string key = tmp.busId.ToString();

                                if (!keys.Contains(key))
                                    keys.Add(key);
                                else
                                {
                                    buffer.RemoveAt(i);
                                    --i;
                                }
                            }
                            break;

                        case "Rail":
                            for (int i = 0; i < buffer.Count; ++i)
                            {
                                RailGPSOutputElement tmp =
                                    (buffer.ElementAt(i) as RailGPSOutputElement);

                                string key = tmp.trainId.ToString();

                                if (!keys.Contains(key))
                                    keys.Add(key);
                                else
                                {
                                    buffer.RemoveAt(i);
                                    --i;
                                }
                            }
                            break;
                        case "TravelTime":
                            for (int i = 0; i < buffer.Count; ++i)
                            {
                                TravelLinksOutputElement tmp =
                                    (buffer.ElementAt(i) as TravelLinksOutputElement);

                                string key = tmp.travelId.ToString();

                                if (!keys.Contains(key))
                                    keys.Add(key);
                                else
                                {
                                    buffer.RemoveAt(i);
                                    --i;
                                }
                            }
                            break;
                        case "Ramp":
                            for (int i = 0; i < buffer.Count; ++i)
                            {
                                RampOutputElement tmp =
                                    (buffer.ElementAt(i) as RampOutputElement);

                                string key = tmp.rampId.ToString();

                                if (!keys.Contains(key))
                                    keys.Add(key);
                                else
                                {
                                    buffer.RemoveAt(i);
                                    --i;
                                }
                            }
                            break;
                        case "Event":
                            for (int i = 0; i < buffer.Count; ++i)
                            {
                                EventEntityTuple tmp =
                                    (buffer.ElementAt(i) as EventEntityTuple);

                                string key = tmp.EventId.ToString();

                                if (!keys.Contains(key))
                                    keys.Add(key);
                                else
                                {
                                    buffer.RemoveAt(i);
                                    --i;
                                }
                            }
                            break;
                    }
 */
                    #endregion

                    result = true;
                    mediaWriter.Write(myCreateOutputMessage.CreateMessage(buffer));
                    buffer.Clear();
                    //OnChanged(args);

                    // Consuming Time written by Penny
                    FileExpender(Config.SourceDataType.ToString() + "_" + Config.Agency + "_" + Config.OutputMediaType.ToString() + "_ConsumingTime.txt", 
                     " "+ EventsDequeued.ToString() + " " + startWritingTime.ToString());

                }
                else
                {
                    Console.WriteLine("No element in buffer: "+ Config.SourceDataType.ToString() + ", " + Config.OutputMediaType.ToString());
                }
            }
            return result;
        }

    }

  
}

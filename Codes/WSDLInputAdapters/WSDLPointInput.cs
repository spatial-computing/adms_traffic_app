/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Threading;
using BaseTrafficInputAdapters;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using System.IO;
using ExceptionReporter;

namespace WSDLInputAdapters
{
    public class WSDLPointInput : RIITSPointInputAdapter
    {
        private DateTime lastFetched= DateTime.MinValue;
        public WSDLPointInput(RIITSInputConfig configInfo, CepEventType cepEventType) : base(configInfo, cepEventType)
        {
        }
        private int WaitMore(int totalWaitInMilli)
        {
            int soFar = (int)(DateTime.Now - lastFetched).TotalMilliseconds;
            return Math.Max(0, totalWaitInMilli - soFar);
        }

        protected bool ListenForXML(PointEvent currEvent)
        {
            lastFetched = DateTime.Now;
            parser.Init();
            justReceived = true;

            if (!parser.XMLLoaded())
                return false; // document not found (SOAP Exception) ==> no xml file ==> do not create events.
            

            //packetDataTime = parser.SpringOverHeaders(_culture);
            //if (packetDataTime == default(DateTime))
            //    return false;
            DateTime newDateTime = parser.SpringOverHeaders(_culture);
            if (packetDataTime == newDateTime)
                return false;

            if (newDateTime == default(DateTime))
                return false;

            packetDataTime = newDateTime;
            return true;
        }

        protected override void ProduceEvents()
        {

            //FileExpender(parser.DataType + "WSDLPointInput_ProduceEvent.txt", "");

            PointEvent currEvent = default(PointEvent);
            EnqueueOperationResult result = EnqueueOperationResult.Full;
            try
            {
                while (true)
                {
                    // first check whether we need to stop the event sending loop
                    if (AdapterState.Stopping == AdapterState)
                    {
                        // resolve the event from the last failed Enqueue;
                        if (_pendingEvent != null)
                        {
                            currEvent = _pendingEvent;
                            _pendingEvent = null;
                        }

                        // do housekeeping
                        PrepareToStop(currEvent);

                        // declare the adapter as stopped
                        Stopped();


                        //Added by Penny
                        FileExpender("ProduceEventStops" + parser.DataType + ".txt", "");

                        // exit the worker thread
                        return;
                    }

                    // NOTE: at any point during execution of the code below, if the Adapter state
                    // changes to Stopping, the engine will resume the adapter (i.e. call Resume())
                    // one more time, and the stopping condition will be trapped at the check above.
                    //
                    // if a previous enqueue failed, enqueue the pending event first before creating a new event
                    // check for a pending payload event, if none,
                    // check for a pending Cti event, if none,
                    // create an event from the file
                    if (_pendingEvent != null)
                    {
                        // the last enqueue failed, so let's try the same event again
                        currEvent = _pendingEvent;
                        _pendingEvent = null;
                    }
                    else if (_pendingCtiTime != null)
                    {
                        // the check is important; but for this adapter, there is nothing to be done here.
                        // In general, you may want to take some action if the last CTI enqueue failed
                    }
                    else if (!parser.XMLLoaded()) // first call to this function
                    {
                        bool successful = false;
                        do
                        {
                            successful= ListenForXML(currEvent);


                            if (!successful)
                            {

                                Thread.Sleep(WaitMore(1000*fetchFrequency));
                            }
                        } while (!successful);
                    }
                    else // main stream.
                    {
                        //_line = _streamReader.ReadLine();
                        parser.ReadNode();
                        justReceived = false;
                        if (parser.IsNotEndElement()) // links tag finished
                            //if (null != _line)
                        {
                            currEvent = ConvertRecordToEvent(parser.ReadARecord()); //_bindtimeEventType.Fields.Count
                            _pendingEvent = null;
                        }
                        else
                        {
                            //insert a CTI and go for the new set of data.
                            var tempOfset = new DateTimeOffset(packetDataTime);

                            // Log written by Penny
                           FileExpender("WSDLPointInput_Enqueue_frq" + parser.DataType  + "_" + parser.agency + ".txt", _eventsEnqueued.ToString() + " " + AdapterState.ToString());


                            if (jjEnqueueCTI(tempOfset))
                                return;
                            


                            bool successful = false;
                            do
                            {
                                Thread.Sleep(WaitMore(1000 * fetchFrequency));
                                successful = ListenForXML(currEvent);
                            } while (!successful);

                        }
                    }

                    // Enqueue the Cti event (or) Current event with payload.
                    // If Enqueue is rejected, save the Cti time or the event itself in
                    // PrepareToResume, indicate Ready to resume, and exit worker thread
                    if (_pendingCtiTime != null)
                    {
                        if (jjEnqueueCTI(_pendingCtiTime.Value))
                        {
                            // if Full
                            return;
                        }
                        else
                        {
                            
                        }
                            
                    }
                    else if (!justReceived)
                    {
                        if (currEvent == null)
                        {
                            throw new AdapterException("EVENT ARGUMENT EXCEPTIO");
                        }
                        result = Enqueue(ref currEvent);

                        if (EnqueueOperationResult.Full == result)
                        {
                            PrepareToResume(currEvent);

                            FileExpender("WSDLPointInput_resume" + parser.DataType  + "_" + parser.agency + ".txt", _eventsEnqueued.ToString() + " " + AdapterState.ToString());

                            Ready();

                            //FileExpender(parser.StartTag + "WSDLPointInput_resume.txt", AdapterState.ToString());

                            return;
                        }
                        else
                        {
                            // if the enqueue was successful, set no pending events, and check for CTI injection
                            _eventsEnqueued++;
                            _pendingEvent = null;

                            // Log written by Penny
                            //FileExpender("WSDLPointInput_Enqueue_" + parser.DataType  + "_" + parser.agency + ".txt", _eventsEnqueued.ToString() + " " + AdapterState.ToString());

                        }
                    }
                    //else if( justReceived) //todo: delete. just for test purposes
                    //{
                    //    StreamWriter tempWriter = new StreamWriter("c:\\count.txt");
                    //    tempWriter.Write(_eventsEnqueued);
                    //    tempWriter.Close();
                    //}
                }
            }
            catch (AdapterException e)
            {
                Console.WriteLine("ProduceEvents - " + e.Message + e.StackTrace);

                FileExpender("WSDLPointInput_Exception_" + parser.StartTag + ".txt", e.Message);
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 200, "WSDLPointInput.cs");
                reporter.SendEmailThread();
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(e.Message, 200, "WSDLPointInput.cs", parser.DataType, "SI");
                reporter2.SendDatabaseExceptionThread();

            }
        }

        public void FileExpender(string fileName, string content)
        {
            //Added by Penny
            try
            {
                StreamWriter tempWriter = new StreamWriter(".\\enqueueLog\\" + fileName, true);
                tempWriter.WriteLine(DateTime.Now.ToString() + ":" + content);
                tempWriter.Close();
                tempWriter.Dispose();
            }
            catch (Exception)
            {
            }
            
        }
    }
}
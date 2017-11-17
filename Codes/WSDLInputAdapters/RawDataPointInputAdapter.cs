/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using WSDLInputAdapters;

namespace BaseTrafficInputAdapters
{
    internal class RawDataPointInputAdapter : WSDLPointInput
    {
        private string all;

        public RawDataPointInputAdapter(RIITSInputConfig configInfo, CepEventType cepEventType)
            : base(configInfo, cepEventType)
        {
        }

        protected bool ListenForXML(PointEvent currEvent)
        {
            parser.Init();
            justReceived = true;

            if (!parser.XMLLoaded())
                return false;

            packetDataTime = parser.SpringOverHeaders(_culture);
            if (packetDataTime == default(DateTime))
                return false;
            return true;
        }

        protected override void ProduceEvents()
        {
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
                        bool successful = ListenForXML(currEvent);

                        if (!successful)
                            return;
                    }
                    else // main stream.
                    {
                        //_line = _streamReader.ReadLine();
                        parser.ReadNode();
                        justReceived = false;
                        if (parser.IsNotEndElement()) // links tag finished
                            //if (null != _line)
                        {
                            currEvent = ConvertRecordToEvent(ReadARecord());
                            _pendingEvent = null;
                        }
                        else
                        {
                            //insert a CTI and go for the new set of data.
                            var tempOfset = new DateTimeOffset(packetDataTime);
                            if (jjEnqueueCTI(tempOfset))
                                return;
                            Thread.Sleep(1000*60);
                            bool successful = ListenForXML(currEvent);

                            if (!successful)
                                return;
                        }
                    }

                    // Enqueue the Cti event (or) Current event with payload.
                    // If Enqueue is rejected, save the Cti time or the event itself in
                    // PrepareToResume, indicate Ready to resume, and exit worker thread
                    if (_pendingCtiTime != null)
                    {
                        if (jjEnqueueCTI(_pendingCtiTime.Value))
                            return;
                    }
                    else if (!justReceived)
                    {
                        result = Enqueue(ref currEvent);

                        if (EnqueueOperationResult.Full == result)
                        {
                            PrepareToResume(currEvent);
                            Ready();
                            return;
                        }
                        else
                        {
                            // if the enqueue was successful, set no pending events, and check for CTI injection
                            _eventsEnqueued++;
                            _pendingEvent = null;
                        }
                    }
                }
            }
            catch (AdapterException e)
            {
                Console.WriteLine("ProduceEvents - " + e.Message + e.StackTrace);
            }
        }

        protected List<string> ReadARecord()
        {
            var result = new List<string>();
            result.Add(all);
            return result;
        }
    }
}
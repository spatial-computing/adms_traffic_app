/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BaseTrafficInputAdapters;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
//using ExceptionReporter;

namespace SocketInputAdapter
{
    public class SocketPointInput : RIITSPointInputAdapter
    {
        private readonly IPAddress ipAd;
        private readonly int portNum;
        private TcpListener sockListener;

        public SocketPointInput(SocketInputConfig configInfo, CepEventType cepEventType)
            : base(configInfo, cepEventType)
        {
            if (configInfo.ServerIP == null)
            {
                throw new ArgumentException("No server IP");
            }
            ipAd = IPAddress.Parse(configInfo.ServerIP);
            if (ipAd == null)
                throw new ArgumentException("Invalid IP address: " + configInfo.ServerIP);

            portNum = configInfo.ServerPort;

            StartSocketServer();
            //ListenForXML(null); we should not wait here because the query has not yet started. If we uncomment this line, then we  are postponing query start time and also processing the data for one minute before at each time.
        }

        public void CloseSocket()
        {
            sockListener.Stop();
        }

        private void startSocket()
        {
            sockListener =
                new TcpListener(ipAd, portNum);

            sockListener.Start();

            Console.WriteLine("Started Socket Server at port number " + portNum + "...");
            Console.WriteLine("The local End point is  :" + sockListener.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");
        }

        private void StartSocketServer()
        {
            if (ipAd != null)
            {
                var jj = new Thread(startSocket);
                jj.Start();
                //    sockListener =
                //new TcpListener(ipAd, portNum);
                //    sockListener.Start();
                //    Console.WriteLine("Started Socket Server at port number " + portNum + "...");
                //    Console.WriteLine("The local End point is  :" + sockListener.LocalEndpoint);
                //    Console.WriteLine("Waiting for a connection.....");
            }
            else
            {
                throw new ArgumentException("Invalid IP address: " + ipAd);
            }
        }

        private bool ListenForXML(PointEvent currEvent)
        {
            Socket s = sockListener.AcceptSocket();
            Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);
            var reader = new StreamReader(new NetworkStream(s));
            String all = reader.ReadToEnd();
            reader.Close();
            justReceived = true;

            if (!all.StartsWith("Ready"))
            {
                Console.Error.WriteLine("Invalid Input Received from" + s.RemoteEndPoint);
                // if end of file, prepare to stop, indicate Stopped and exit from worker thread
                EnqueueCtiEvent(DateTimeOffset.MaxValue);
                //Console.WriteLine(_eventsEnqueued);
                PrepareToStop(currEvent);
                Stopped();

                return false;
            }
            else

            {
                parser.Init();
                //    all = all.Substring(6);
                //    int threadID = Convert.ToInt32(all.Substring(0, 1));
                //    Console.WriteLine("Read Thread#: " + threadID);
                //    all = all.Substring(2);

                if (!parser.XMLLoaded())
                    return false;

                packetDataTime = parser.SpringOverHeaders(_culture);
                if (packetDataTime == default(DateTime))
                    return false;

                return true;
            }
        }

        protected void PrepareToStop(PointEvent currEvent)
        {
            CloseSocket();
            base.PrepareToStop(currEvent);
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
                            currEvent = ConvertRecordToEvent(parser.ReadARecord());
                            _pendingEvent = null;
                        }
                        else
                        {
                            //insert a CTI and listen on the port
                            var tempOfset = new DateTimeOffset(packetDataTime);
                            if (jjEnqueueCTI(tempOfset))
                                return;
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
                //ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 228, "SocketPointInput.cs");
                //reporter.SendEmailThread();
            }
        }


        protected override void Dispose(bool disposing)
        {
            // _streamReader.Dispose();
            parser.CloseFile();
            sockListener.Stop();

            base.Dispose(disposing);
        }
    }
}
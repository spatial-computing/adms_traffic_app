using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using EventTypes;

namespace XMLOutputAdapter
{
    //public delegate void ChangedEventHandler(object sender, EventArgs e);

    public abstract class XmlPointOutput<T> : OutputAdapterBase<T> where T : IOutputType, new()
    {
        //protected EventWaitHandle AdapterStopSignal;

        
        protected List<String> EndTags;
    
        protected string Line;
        protected StreamWriter StreamWriter;

        protected XmlPointOutput(TrafficOutputConfig configInfo, CepEventType cepEventType): base(configInfo, cepEventType)
        {
            EndTags = CreateEndTags(configInfo);
            
            //  AdapterStopSignal = EventWaitHandle.OpenExisting(configInfo.AdapterStopSignal);
        }

        //public event ChangedEventHandler Changed;

        //protected virtual void OnChanged(EventArgs e)
        //{
        //    if (Changed != null)
        //        Changed(this, e);
        //}

        
        protected List<String> CreateEndTags(TrafficOutputConfig configInfo)
        {
            var result = new List<string>();
            foreach (String elemTag in configInfo.OutputFieldOrders)
                result.Add(elemTag.Insert(1, "/"));
            return result;
        }

        
        public static string GetDefaultHeader()
        {
            return "<?xml version=\"1.0\"?>";
        }
    }

    public class EventArgs<T> : EventArgs
    {
        private readonly T m_value;

        public EventArgs(T value)
        {
            m_value = value;
        }

        public T Value
        {
            get { return m_value; }
        }
    }
}
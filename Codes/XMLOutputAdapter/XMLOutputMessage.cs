﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using MyMemory;
using OutputTypes;

namespace XMLOutputAdapter
{
    public class XMLOutputMessage<T> : IOutputMessage<T> where T: IOutputType
    {
        private XMLMessageConfig config;
        private LookupTable<T> lookupTable;
        private List<String> EndTags;

        public XMLOutputMessage(XMLMessageConfig configInfo, LookupTable<T> table)
        {
            config= configInfo;
            lookupTable = table;
            EndTags = CreateEndTags(config);
        }

        protected List<String> CreateEndTags(XMLMessageConfig configInfo)
        {
            var result = new List<string>();
            foreach (String elemTag in configInfo.OutputFieldOrders)
                result.Add(elemTag.Insert(1, "/"));
            return result;
        }

        public Object CreateMessage(List<T> buffer)
        {  
            String xmlStr = config.Header //"<?xml version=\"1.0\"?>"
                            + config.RootName; //"<mkrs>";
            int count = 0;
            foreach (T e in buffer)
            {

                var values = (List<object>) lookupTable.GetRecord(e, config.Agency);
                if (values == null) // sensor information not found in our databases.
                    continue;
                        
                xmlStr += config.OtherTopStories;
                for (int i = 0; i < values.Count; i++)
                    xmlStr += config.OutputFieldOrders[i].ToString() + values[i] + EndTags[i];

                xmlStr += config.OtherTopStories.Insert(1, "/");

                count++;
            }
            xmlStr += config.RootName.Insert(1, "/");


            //TextWriter tw = new StreamWriter("C:/inetpub/wwwroot/WebApplication1/HighwayTraffic.xml");
            return xmlStr;
        }
    }
}

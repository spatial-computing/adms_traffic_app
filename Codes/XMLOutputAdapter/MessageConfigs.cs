﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLOutputAdapter
{
    public class OutputMessageConfig
    {
        public OutputMessageConfig(List<Object> outputFieldOrders)
        {
            OutputFieldOrders = outputFieldOrders;
            
        }
        
        public List<Object> OutputFieldOrders { get; set; }

    }
    public class SQLMessageConfig : OutputMessageConfig
    {
        public string TableName { get; set; }
        public string Agency { get; set; }
        public SQLMessageConfig(string tableName, List<Object> outputFieldOrders, string agency)
            : base(outputFieldOrders)
        {
            TableName = tableName;
            Agency = agency;
        }
    }
    public class XMLMessageConfig : OutputMessageConfig
    {
        public XMLMessageConfig(List<Object> outputFieldOrders, string agency)
            : base(outputFieldOrders)
        {
            Header = "<?xml version=\"1.0\"?>";
            RootName = "<mkrs>";
            OtherTopStories = "<mkr>";
            Agency = agency;
        }

        public string Header { get; set; }
        public string RootName { get; set; }
        public string OtherTopStories { get; set; }
        public string Agency { get; set; }
    }
}

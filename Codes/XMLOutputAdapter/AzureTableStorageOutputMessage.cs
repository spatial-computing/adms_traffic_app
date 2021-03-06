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
    public class AzureTableStorageOutputMessage<T> : IOutputMessage<T> where T : IOutputType
    {
        private LookupTable<T> lookupTable;
        public SQLMessageConfig config;
        public AzureTableStorageOutputMessage(LookupTable<T> table, SQLMessageConfig configInfo)
        {
            lookupTable = table;
            config = configInfo;
        }
        
        public object CreateMessage(List<T> buffer)
        {
            List<Object> result = new List<Object>();
            foreach (T e in buffer)
            {

                var obj = lookupTable.GetRecord(e, config.Agency);
                if (obj== null) // sensor information not found in our databases.
                    continue;

                result.Add(obj);
            }

            return result;
        }
    }
}

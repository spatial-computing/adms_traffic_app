/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OutputTypes;
using Parsers;
using EventTypes;
using Utils;

namespace MyMemory
{
    public abstract class LookupTable<TOutputType> where TOutputType: IOutputType
    {
        //note: if no such sensor exists in our sensor database, then return null as the list.
        //public abstract List<object> GetRecord(TOutputType ev);
        public abstract Object GetRecord(TOutputType ev, string agency);
    }

}

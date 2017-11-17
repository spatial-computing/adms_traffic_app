/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using OutputTypes;

namespace XMLOutputAdapter
{
    public interface IOutputMessage<T> where T : IOutputType
    {
        Object CreateMessage(List<T> buffer);
    }
}

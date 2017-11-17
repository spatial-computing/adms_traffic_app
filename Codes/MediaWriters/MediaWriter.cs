/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;

namespace MediaWriters
{
    public interface MediaWriter
    {
        bool Write(Object message);
    }
}

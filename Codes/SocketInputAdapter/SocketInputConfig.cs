/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using BaseTrafficInputAdapters;

namespace SocketInputAdapter
{
    public class SocketInputConfig : RIITSInputConfig
    {
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
    }
}
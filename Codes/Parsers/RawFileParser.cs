/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System.Collections.Generic;

namespace Parsers
{
    public class RawFileParser : BaseFileParser
    {
        public RawFileParser(DataFetcher fetcher, string agency):base(agency, "depricated","")
        {
            del = fetcher;
        }

        public override List<string> ReadARecord()
        {
            var result = new List<string>();
            result.Add(all);
            return result;
        }
    }
}
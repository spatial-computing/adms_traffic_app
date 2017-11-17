/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Parser;
using EventTypes;

namespace Parsers
{
    public class TravelTimeParser: BaseFileParser
    {
        public TravelTimeParser(string agency)
            : base(agency, "travelLinks", SourceDataType.TravelTime.ToString())
        {
            expectedFieldNum = 3;
            del = FetchData;
        }

       

        public override List<string> ReadARecord()
        {
            var result = new List<string>();
            for (int i = 0; i < expectedFieldNum; i++)
            {
                do
                {
                    textReader.Read();
                } while (textReader.NodeType == XmlNodeType.Element || textReader.NodeType == XmlNodeType.EndElement);

                if (textReader.NodeType == XmlNodeType.Text)
                    result.Add(textReader.Value);
            }
            textReader.Read();
            textReader.Read();
            return result;
        }

        public string FetchData()
        {
            // return FreewayDataReplayer(); //todo: comment later
            return WSDLConnector("travelTimes", agency, "real-time");
        }
    
    }
}

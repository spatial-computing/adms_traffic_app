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
    class RailParser : BaseFileParser
    {
        public RailParser(string agency)
            : base(agency, "trains", SourceDataType.Rail.ToString())
        {
            expectedFieldNum = 14;
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
            textReader.Read(); // /timepointTime
            textReader.Read(); // /train

            return result;
        }

        public string FetchData()
        {
            // return FreewayDataReplayer(); //todo: comment later
            return WSDLConnector("rail", agency, "real-time");
        }
    
    }
}

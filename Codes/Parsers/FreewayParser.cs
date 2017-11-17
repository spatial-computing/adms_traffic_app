/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Xml;
using Parser;
using Parsers.CongestionService1;
using EventTypes;

namespace Parsers
{
    public class FreewayParser : BaseFileParser
    {
        public static string startTag = "links";
        public FreewayParser(string agency): base(agency, "links", SourceDataType.Freeway.ToString())
        {
            expectedFieldNum = 6;
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
                {
                    
                        result.Add(textReader.Value);
                }
            }
            textReader.Read(); // /linkDataStatus
            textReader.Read(); // /localLinkTrafficInformation
            textReader.Read(); // /link
            return result;
        }

        public string FetchData()
        {
           // return FreewayDataReplayer(); //todo: comment later
            return WSDLConnector("congestionFreeway", agency, "real-time");
        }
    
    }
}
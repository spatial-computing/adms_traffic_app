/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using EventTypes;

namespace Parsers
{
    public class TravelTimeInventoryParser: BaseFileParser
    {
        public TravelTimeInventoryParser(string agency)
            : base(agency, "travelLinks", SourceDataType.TravelTime.ToString())
        {
            expectedFieldNum = 13;
            del = FetchData;
        }

        public travelLinks ReadATravelLinkInfo()
        {
            return new travelLinks(agency, ReadARecord());
        }

       

        public override List<string> ReadARecord()
        {
            var result = new List<string>(13);

            bool goOn = true;
            while (goOn && textReader.Read())
            {
                switch (textReader.Name)
                {
                    case "travelLink":
                        if (textReader.NodeType == XmlNodeType.EndElement)
                            goOn = false;
                        break;
                    case "id":
                    case "route":
                    case "direction":
                    case "linkType":
                    case "beginNodeId":
                    case "endNodeId":
                    case "length":
                    case "beginCrossStreet":
                    case "beginLatitude":
                    case "beginLongitude":
                    case "endCrossStreet":
                    case "endLatitude":
                    case "endLongitude":
                        textReader.Read();
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag
                        break;
                }
            }
            return result;
        }

        public string FetchData()
        {
            return WSDLConnector("travelTimes", agency, "inventory");
        }
    }
}

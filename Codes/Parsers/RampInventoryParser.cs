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
    public class RampInventoryParser: BaseFileParser
    {
        public RampInventoryParser(string agency)
            : base(agency, "rampMeterListInventory", SourceDataType.Ramp.ToString())
        {
            expectedFieldNum = 10;
            del = FetchData;
        }

        public RampMeter ReadARampMeterInfo()
        {
            return new RampMeter(agency, ReadARecord());
        }

        public override List<string> ReadARecord()
        {
            var result = new List<string>(3);

            bool goOn = true;
            while (goOn && textReader.Read())
            {
                switch (textReader.Name)
                {
                    case "rampMeterInventory":
                        if (textReader.NodeType == XmlNodeType.EndElement)
                            goOn = false;
                        break;
                    case "id":
                    case "rampType":
                    case "latitude":
                    case "longitude":
                    case "direction":
                    case "city":
                    case "postmile":
                        textReader.Read();
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag
                        break;
                    case "msid":
                        textReader.Read();
                        if (textReader.Value.Length < 1)                //if there is no msid
                            result.Add("-1");
                        else 
                            result.Add(textReader.Value);

                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag
                        break;
                    case "onStreetInfo":
                    case "fromStreetInfo":
                    case "toStreetInfo":
                        textReader.Read();  //streets
                        textReader.Read();  //names
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag      
                        textReader.Read();
                        break;
                }
            }
            return result;
        }

        public string FetchData()
        {
            return WSDLConnector("rms", agency, "inventory");
        }
    }
}


using System;
using System.Collections.Generic;
using System.Xml;
using Parser;
using EventTypes;

namespace Parsers
{
    public class CmsDeviceParser: BaseFileParser
    {

        public CmsDeviceParser(string agency)
            : base(agency, "dmsListInventory", SourceDataType.Cms.ToString())
        {
            expectedFieldNum = 9;
            del = FetchData;
            
        }

        public CmsDevice ReadADeviceInfo()
        {
            return new CmsDevice(agency, ReadARecord());
        }


        public override List<string> ReadARecord()
        {
            var result = new List<string>(11);

            bool goOn = true;
            while (goOn && textReader.Read())
            {
                switch (textReader.Name)
                {
                    case "dmsInventory":
                        if (textReader.NodeType == XmlNodeType.EndElement)
                            goOn = false;
                        break;
                    case "id":
                        textReader.Read();
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag
                        break;
                    case "onStreetInfo":
                        textReader.Read(); // onstreet
                        textReader.Read(); //name
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /onstreet
                        break;
                    case "fromStreetInfo":
                        textReader.Read(); // fromstreet
                        textReader.Read(); //name
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /fromstreet
                        break;

                    case "toStreetInfo":

                        textReader.Read(); // tostreet
                        textReader.Read(); //name
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /name
                        textReader.Read(); // /toStreet
                        break;
                    case "latitude":
                        textReader.Read(); // latitude
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /latitude
                        break;
                    case "longitude":
                        textReader.Read(); // longitude
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /longitude
                        break;

                    case "direction":
                        textReader.Read(); // direction
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /direction
                        break;

                    case "city":
                        textReader.Read(); // city
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /city

                        break;
                    case "postmile":
                        textReader.Read(); // postmile
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); // /postmile
                        break;

                    
                }
            }


            return result;
        }

        public string FetchData()
        {
            return WSDLConnector("cms", agency, "inventory");
        }
    }
}
